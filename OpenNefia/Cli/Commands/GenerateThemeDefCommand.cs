using CommandLine;
using FluentResults;
using OpenNefia.Core.Data;
using OpenNefia.Core.Data.Patch;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Extensions;
using OpenNefia.Core.Rendering;
using OpenNefia.Mod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OpenNefia.Cli.Commands
{
    [Verb(name: "generateThemeDef", HelpText = "Generate a ThemeDef XML file from a directory containing Elona asset files.")]
    internal class GenerateThemeDefOptions : BaseOptions
    {
        [Option(longName: nameof(AssetDirectoryPrefix), shortName: 'p', HelpText = "Prefix that will hold the assets.")]
        public string AssetDirectoryPrefix { get; set; } = "Assets";

        [Value(0, MetaName = nameof(InputDirectory), Required = true, HelpText = "Source directory containing graphic/, sound/, etc. folders")]
        public string InputDirectory { get; set; } = string.Empty;

        [Value(1, MetaName = nameof(OutputDirectory), Required = true, HelpText = "Output directory to put the Theme.xml file")]
        public string OutputDirectory { get; set; } = string.Empty;

        [Value(2, MetaName = nameof(ThemeDefId), Required = true, HelpText = "ThemeDef ID to use for the newly created theme")]
        public string ThemeDefId { get; set; } = string.Empty;
    }

    /// <summary>
    /// TODO: this is bad.
    /// just write a custom <see cref="PatchOperation"/> that hides the massive wad of XML this produces
    /// </summary>
    internal class GenerateThemeDefCommand : ICommand
    {
        private HashSet<string> SourcePaths = new HashSet<string>();

        private GenerateThemeDefOptions Options;

        public GenerateThemeDefCommand(GenerateThemeDefOptions options)
        {
            this.Options = options;
        }

        // Hackish, since it assumes the same directory structure as 1.22 under Assets/ (but this is abided by)
        private string ToElonaPath(ModLocalPath path) => ToElonaPath(path.Subpath);
        private string ToElonaPath(IResourcePath path) => ToElonaPath((path as ModLocalPath)!);
        private string ToElonaPath(string path) => path.Replace("Assets/", "").Replace("\\", "/").ToLowerInvariant();

        public async Task<Result> Execute()
        {
            if (!Directory.Exists(Options.InputDirectory))
            {
                return Result.Fail($"Input directory {Options.InputDirectory} does not exist.");
            }
            if (!Directory.Exists(Options.OutputDirectory))
            {
                return Result.Fail($"Output directory {Options.OutputDirectory} does not exist.");
            }

            var newOverrides = new Dictionary<string, string>();

            var enumerationOpts = new EnumerationOptions() { RecurseSubdirectories = true };
            foreach (var file in Directory.EnumerateFiles(Options.InputDirectory, "*", enumerationOpts))
            {
                SourcePaths.Add(Path.GetRelativePath(Options.InputDirectory, file).Replace("\\", "/"));
            }
            var operationsElement = new XElement("Operations");

            // We will support a few select Def types for now. These are the ones that have
            // direct relevance to assets that are a part of vanilla Elona.

            foreach (var def in DefStore<AssetDef>.Enumerate())
            {
                if (CanPatch(def.Image))
                {
                    AddThemePatch(def.OriginalXml, operationsElement);
                }
            }
            foreach (var def in DefStore<ChipDef>.Enumerate())
            {
                if (CanPatch(def.Image))
                {
                    AddThemePatch(def.OriginalXml, operationsElement);
                }
            }
            foreach (var def in DefStore<TileDef>.Enumerate())
            {
                if (CanPatch(def.Image) || (def.WallImage != null && CanPatch(def.WallImage)))
                {
                    AddThemePatch(def.OriginalXml, operationsElement);
                }
            }

            var themeElement = new XElement("ThemeDef");
            themeElement.SetAttributeValue("Id", Options.ThemeDefId);
            themeElement.Add(operationsElement);

            var defsElement = new XElement("Defs");
            defsElement.Add(themeElement);
            var themeDefXml = new XDocument(defsElement);

            var outputFile = Path.Join(Options.OutputDirectory, $"Theme_{Options.ThemeDefId}.xml");
            await File.WriteAllTextAsync(outputFile, themeDefXml.ToString());

            Console.WriteLine($"Wrote ThemeDef XML to {outputFile}.");

            return Result.Ok();
        }

        private void AddThemePatch(XElement originalXml, XElement operationsElement)
        {
            foreach (var element in originalXml.Elements())
            {
                var newElement = new XElement(element);
                var changed = false;

                Action<XElement> check = (e) =>
                {
                    var elonaPathSuffix = ToElonaPath(e.Value);
                    if (SourcePaths.Contains(elonaPathSuffix))
                    {
                        // "Assets" + "graphic/character.bmp"
                        e.Value = $"{Options.AssetDirectoryPrefix}/{elonaPathSuffix}";
                        changed = true;
                    }

                    foreach (var descAttribute in e.Attributes())
                    {
                        elonaPathSuffix = ToElonaPath(descAttribute.Value);
                        if (SourcePaths.Contains(elonaPathSuffix))
                        {
                            descAttribute.Value = $"{Options.AssetDirectoryPrefix}/{elonaPathSuffix}";
                            changed = true;
                        }
                    }
                };

                check(newElement);
                foreach (var desc in newElement.Descendants())
                {
                    check(desc);
                }

                if (changed)
                {
                    var replaceOpElement = new XElement("Operation");
                    replaceOpElement.SetAttributeValue("Class", typeof(PatchOperationReplace).FullName);
                    replaceOpElement.Add(new XElement("XPath", element.GetAbsoluteXPathDefAware()));
                    replaceOpElement.Add(new XElement("Value", newElement));
                    operationsElement.Add(replaceOpElement);
                }
            }
        }

        private bool CanPatch(AssetSpec image)
        {
            if (image.ImageRegion != null)
            {
                return SourcePaths.Contains(ToElonaPath(image.ImageRegion.SourceImagePath));
            }
            else
            {
                return SourcePaths.Contains(ToElonaPath(image.ImagePath!));
            }
        }

        private bool CanPatch(TileSpec tile)
        {
            if (tile.ImageRegion != null)
            {
                return SourcePaths.Contains(ToElonaPath(tile.ImageRegion.SourceImagePath));
            }
            else
            {
                return SourcePaths.Contains(ToElonaPath(tile.ImagePath!));
            }
        }
    }
}

using FluentResults;
using OpenNefia.Core.Data.Patch;
using OpenNefia.Core.Data.Serial.CrossRefs;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Extensions;
using OpenNefia.Game;
using OpenNefia.Mod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OpenNefia.Core.Data.Serial
{
    internal class LoadedDefElement
    {
        public DefIdentifier DefIdentifier;
        public Def? Def;
        public XElement? SourceXml;
        public ModInfo AddingMod;
        public bool NeedsMerge;

        public LoadedDefElement(DefIdentifier defIdentifier, ModInfo addingMod)
        {
            DefIdentifier = defIdentifier;
            AddingMod = addingMod;
            NeedsMerge = false;
        }
    }

    internal static class DefLoader
    {
        private static readonly Dictionary<DefIdentifier, LoadedDefElement> AllDefs = new Dictionary<DefIdentifier, LoadedDefElement>();
        internal static XDocument MasterDefXML;
        private static XElement MasterDefsElement;

        static DefLoader()
        {
            MasterDefXML = new XDocument(new XElement("Defs"));
            MasterDefsElement = MasterDefXML.Element("Defs")!;
        }

        internal static Type? GetDirectDefType(Type type)
        {
            if (!typeof(Def).IsAssignableFrom(type))
            {
                return null;
            }

            while (type.BaseType != null && type.BaseType != typeof(object))
            {
                if (type.BaseType == typeof(Def) || type.BaseType.IsAbstract)
                {
                    return type;
                }
                type = type.BaseType!;
            }

            return null;
        }

        private static bool HasNamespacedDefTypeName(XElement ownedNode)
        {
            return ownedNode.Name.LocalName.Contains(".");
        }

        internal static void AppendDefXml(string filepath, ModInfo modInfo, DefDeserializer deserializer)
        {
            var xmlDocument = XDocument.Load(filepath);

            var root = xmlDocument.Root;
            if (root?.Name != "Defs")
            {
                return;
            }

            foreach (var elem in root.Elements())
            {
                var defIdentifier = DefDeserializer.GetDefIdAndTypeFromElement(elem);
                if (defIdentifier.IsSuccess)
                {
                    // <AssetDef/> -> <Core.AssetDef/>
                    if (!HasNamespacedDefTypeName(elem))
                    {
                        elem.Name = $"{modInfo.Metadata.Name}.{elem.Name}";
                    }
                    MasterDefsElement.Add(elem);
                    AllDefs.Add(defIdentifier.Value, new LoadedDefElement(defIdentifier.Value, modInfo));
                }
                else
                {
                    Console.WriteLine($"Skipping invalid def node {elem.Name}");
                }
            }
        }

        internal static Def? GetDef(DefIdentifier identifier) => GetDef(identifier.DefType, identifier.DefId);
        
        internal static Def? GetDef(Type defType, string defId)
        {
            var args = new object[] { defId };
            var store = typeof(DefStore<>)!.MakeGenericType(defType)!;

            var contains = store.GetMethod("ContainsDefId")!;
            if (!(bool)contains.Invoke(null, args)!)
                return null;

            var get = store.GetMethod("Get")!;
            return (Def?)get.Invoke(null, args);
        }

        internal static void LoadAll()
        {
            Logger.Info($"[DefLoader] Loading all defs...");

            DefTypes.ScanAllTypes();

            var deserializer = new DefDeserializer();

            // Build an XML document holding all defs.
            // The reason it's kept in one place is for running XPath operations on everything later.
            BuildMasterXMLDocument(deserializer);

            // Load all defs in the master XML def document.
            LoadDefs(deserializer);

            // Add all defs to the database.
            AddDefs();

            // Make sure DefOfEntries classes are populated so they can be used during crossref resolution.
            PopulateStaticEntries();

            // Resolve dependencies between defs.
            ResolveCrossRefs(deserializer);

            LocalizeDefs();

            Logger.Info($"[DefLoader] Finished loading.");
        }

        private static void BuildMasterXMLDocument(DefDeserializer deserializer)
        {
            foreach (var modInfo in Engine.ModLoader.LoadedMods)
            {
                var path = new ModLocalPath(modInfo, "Defs");
                var resolved = path.Resolve();
                if (Directory.Exists(resolved))
                {
                    foreach (var defSetFile in Directory.EnumerateFiles(resolved, "*.xml"))
                    {
                        AppendDefXml(defSetFile, modInfo, deserializer);
                    }
                }
            }

            CheckErrors(deserializer.Errors, "Errors loading defs");
        }

        private static void LoadDefs(DefDeserializer deserializer)
        {
            Logger.Info($"[DefLoader] LoadDefs");

            foreach (var element in MasterDefsElement.Elements())
            {
                var defIdentifier = DefDeserializer.GetDefIdAndTypeFromElement(element)!.Value;
                var loadedDef = AllDefs[defIdentifier];
                loadedDef.SourceXml = element;
                var containingMod = loadedDef.AddingMod;
                var result = deserializer.DeserializeDef(element, containingMod, DefDeserializeMode.FromDisk);

                if (result.IsSuccess)
                {
                    var defInstance = result.Value;
                    loadedDef.Def = defInstance;
                    loadedDef.SourceXml.Changed += (_, _) => loadedDef.NeedsMerge = true;
                }
            }
        }

        internal static void ApplyActiveThemes(List<ThemeDef> themes)
        {
            // TODO reload any previously modified defs from
            // their original XML.

            var deserializer = new DefDeserializer();

            var finalResult = new PatchResult();

            foreach (var theme in themes)
            {
                foreach (var patch in theme.Operations)
                {
                    var result = patch.Apply(MasterDefXML);
                    if (result.IsSuccess)
                    {
                        finalResult.Merge(result.Value);
                    }
                    else
                    {
                        Console.WriteLine($"Patch failure: {result}");
                    }
                }
            }

            AllDefs.Values
                .Where(loadedDef => loadedDef.NeedsMerge)
                .ForEach(loadedDef => ReloadDef(loadedDef!, deserializer));

            ResolveCrossRefs(deserializer);
        }

        private static void ReloadDef(LoadedDefElement loadedDef, DefDeserializer deserializer)
        {
            loadedDef.NeedsMerge = false;

            var originalDef = loadedDef.Def;
            if (originalDef == null)
            {
                throw new Exception($"Def {loadedDef.DefIdentifier} was marked as loaded, but its instance was null");
            }

            var element = originalDef.OriginalXml!;
            var containingMod = originalDef.Mod!;
            var result = deserializer.DeserializeDef(element, containingMod, DefDeserializeMode.AlreadyLoaded);

            if (result.IsSuccess)
            {
                var mergingDef = result.Value;
                var mergeResult = DefMerger.Merge(originalDef, mergingDef);
                if (mergeResult.IsFailed)
                {
                    throw new Exception($"Failed to patch def instance: {mergeResult}");
                }
            }
            else
            {
                throw new Exception($"Failed to patch def node: {result}");
            }
        }

        private static void AddDefs()
        {
            Logger.Info($"[DefLoader] AddDefs");

            List<string> errors = new List<string>();

            foreach (var def in AllDefs.Values.Select(ld => ld.Def!))
            {
                var store = typeof(DefStore<>)!.MakeGenericType(def.GetDirectDefType())!;
                var contains = store.GetMethod("ContainsDefId")!;
                if ((bool)contains.Invoke(null, new object[] { def.Id })!)
                {
                    errors.Add($"Def with same ID already exists: {def}");
                }
                else
                {
                    var add = store.GetMethod("AddDef", BindingFlags.NonPublic | BindingFlags.Static)!;
                    add.Invoke(store, new object[] { def });
                }
            }

            CheckErrors(errors, "Errors adding defs");
        }

        private static void ResolveCrossRefs(DefDeserializer defDeserializer)
        {
            Logger.Info($"[DefLoader] ResolveCrossRefs");

            List<string> errors = new List<string>();

            foreach (var crossRef in defDeserializer.CrossRefs)
            {
                var defsResult = GetDefsFromCrossRef(crossRef);
                if (defsResult.IsSuccess)
                {
                    crossRef.Resolve(defsResult.Value);
                }
                else
                {
                    errors.Add(defsResult.Errors.First().Message);
                }
            }

            foreach (var loadedDef in AllDefs.Values)
            {
                loadedDef.Def!.OnResolveReferences();
                loadedDef.Def!.ValidateDefField(errors);
            }

            CheckErrors(errors, $"Errors resolving crossreferences between defs");

            defDeserializer.CrossRefs.Clear();
        }

        internal static void LocalizeDefs()
        {
            foreach (var loadedDef in AllDefs.Values)
            {
                var ident = loadedDef.Def!.Identifier;
                var key = $"{ident.DefType.FullName}.{ident.DefId}";
                loadedDef.Def!.Localize(key);
            }
        }

        private static Result<List<Def>> GetDefsFromCrossRef(IDefCrossRef crossRef)
        {
            var defs = new List<Def>();

            foreach (var defIdent in crossRef.GetWantedCrossRefs())
            {
                var def = DefLoader.GetDef(defIdent);
                if (def == null)
                {
                    return Result.Fail($"Could not find def crossreference '{defIdent}'");
                }
                else
                {
                    defs.Add(def);
                }
            }

            return Result.Ok(defs);
        }

        private static bool IsEntriesType(Type arg)
        {
            return arg.GetCustomAttribute<DefOfEntriesAttribute>() != null;
        }

        private static ModInfo? FindContainingMod(Assembly assembly, Type ty)
        {
            var attrib = ty.GetCustomAttribute<DefOfEntriesAttribute>()!;
            var containingModType = attrib.ContainingMod;
            if (containingModType != null)
                return Engine.ModLoader.GetModFromType(containingModType);
            return Engine.ModLoader.GetModFromAssembly(assembly);
        }

        internal static void PopulateStaticEntries()
        {
            Logger.Info($"[DefLoader] Populating DefOfEntries classes.");

            List<string> errors = new List<string>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var ty in assembly.GetTypes().Where(IsEntriesType))
                {
                    var containingMod = FindContainingMod(assembly, ty);
                    if (containingMod == null)
                    {
                        errors.Add($"Cannot determine containing mod for def entries class {ty}");
                    }
                    else
                    {
                        foreach (var field in ty.GetFields(BindingFlags.Static | BindingFlags.Public))
                        {
                            var defType = GetDirectDefType(field.FieldType);
                            if (defType == null)
                            {
                                errors.Add($"Type {defType} is not a descendent of type that inherits from Def");
                            }
                            else
                            {
                                var defId = $"{containingMod.Metadata.Name}.{field.Name}";

                                var def = GetDef(defType, defId);
                                if (def == null)
                                {
                                    errors.Add($"{ty.FullName}: Could not find def '{defType.Name}.{defId}'");
                                }
                                else
                                {
                                    field.SetValue(null, def);
                                }
                            }
                        }
                    }

                }
            }

            CheckErrors(errors, "Errors initializing DefEntriesOf classes");
        }

        private static void CheckErrors(List<string> errors, string message)
        {
            if (errors.Count > 0)
            {
                var i = 0;
                var errorMessage = "";
                foreach (var error in errors)
                {
                    errorMessage += $"{error}\n";
                    i++;
                    if (i > 10)
                    {
                        errorMessage += "...";
                        break;
                    }
                }
                throw new Exception($"{message}:\n{errorMessage}");
            }
        }
    }
}

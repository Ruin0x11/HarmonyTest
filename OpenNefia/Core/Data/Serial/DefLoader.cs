using OpenNefia.Core.Data.Patch;
using OpenNefia.Core.Data.Types.DefOf;
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
using System.Xml;

namespace OpenNefia.Core.Data.Serial
{
    internal static class DefLoader
    {
        private static readonly Dictionary<DefIdentifier, Def> AllDefs = new Dictionary<DefIdentifier, Def>();
        private static readonly Dictionary<XmlNode, ModInfo> NodeToAddingMod = new Dictionary<XmlNode, ModInfo>();
        internal static XmlDocument MasterDefXML;
        private static XmlElement MasterDefsElement;

        static DefLoader()
        {
            MasterDefXML = new XmlDocument();
            MasterDefsElement = MasterDefXML.CreateElement("Defs");
            MasterDefXML.AppendChild(MasterDefsElement);
        }

        internal static Type? GetDirectDefType(Type type)
        {
            if (type.BaseType == typeof(Def))
            {
                return type;
            }
            else if (type.BaseType == null || type.BaseType == typeof(object))
            {
                return null;
            }
            return GetDirectDefType(type.BaseType!);
        }

        internal static void AppendDefXml(string filepath, ModInfo modInfo, DefDeserializer deserializer)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(filepath);

            var root = xmlDocument.DocumentElement;
            if (root?.Name != "Defs")
            {
                return;
            }

            foreach (var node in root.ChildNodes.Cast<XmlNode>())
            {
                if (DefDeserializer.IsValidDefNode(node))
                {
                    var ownedNode = MasterDefsElement.OwnerDocument.ImportNode(node, true);
                    MasterDefsElement.AppendChild(ownedNode);
                    NodeToAddingMod.Add(ownedNode, modInfo);
                }
                else
                {
                    Console.WriteLine($"Skipping invalid def node {node.Name}");
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

        private static void ResolveCrossRefs(DefDeserializer defDeserializer)
        {
            Logger.Info($"[DefLoader] ResolveCrossRefs");

            List<string> errors = new List<string>();

            foreach (var crossRef in defDeserializer.CrossRefs)
            {
                crossRef.Resolve(errors);
            }

            foreach (var def in AllDefs.Values)
            {
                def.OnResolveReferences();
                def.ValidateDefField(errors);
            }

            CheckErrors(errors, $"Errors resolving crossreferences between defs");

            defDeserializer.CrossRefs.Clear();
        }

        private static void BuildMasterXMLDocument(DefDeserializer deserializer)
        {
            foreach (var modInfo in GameWrapper.Instance.ModLoader.LoadedMods)
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

            // Apply the active ThemeDefs and merge them into the affected defs.
            ApplyActiveThemes(deserializer);

            AllDefs.Clear();

            Logger.Info($"[DefLoader] Finished loading.");
        }

        private static void LoadDefs(DefDeserializer deserializer)
        {
            Logger.Info($"[DefLoader] LoadDefs");

            foreach (var node in MasterDefXML!.DocumentElement!.ChildNodes.Cast<XmlNode>())
            {
                var containingMod = NodeToAddingMod[node]!;
                var result = deserializer.DeserializeDef(node, containingMod);

                if (result.IsSuccess)
                {
                    var defInstance = result.Value;
                    AllDefs.Add(defInstance.Identifier, defInstance);
                }
            }
        }

        private static void ApplyActiveThemes(DefDeserializer deserializer)
        {
            var theme = ThemeDefOf.TestTheme;

            var finalResult = new PatchResult();

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

            finalResult.AffectedDefs
                .Select(ident => DefLoader.GetDef(ident))
                .WhereNotNull()
                .ForEach(def => ReloadDef(def, deserializer));

            ResolveCrossRefs(deserializer);
        }

        private static void ReloadDef(Def originalDef, DefDeserializer deserializer)
        {
            var node = originalDef.OriginalXml!;
            var containingMod = NodeToAddingMod[node]!;
            var result = deserializer.DeserializeDef(node, containingMod);

            if (result.IsSuccess)
            {
                var mergingDef = result.Value;
                MergeDefs(originalDef, mergingDef);
            }
            else
            {
                throw new Exception($"Failed to patch def node: {result}");
            }
        }

        private static void MergeDefs(Def originalDef, Def mergingDef)
        {
            var t1 = originalDef.GetDirectDefType();
            var t2 = mergingDef.GetDirectDefType();
            if (t1 != t2)
            {
                throw new Exception($"Cannot merge defs of type {t1} and {t2}.");
            }
            Console.WriteLine($"Merge {originalDef} {mergingDef}");
        }

        private static void AddDefs()
        {
            Logger.Info($"[DefLoader] AddDefs");

            List<string> errors = new List<string>();

            foreach (var def in AllDefs.Values)
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

        private static bool IsEntriesType(Type arg)
        {
            return arg.GetCustomAttribute<DefOfEntriesAttribute>() != null;
        }

        private static ModInfo? FindContainingMod(Assembly assembly, Type ty)
        {
            var attrib = ty.GetCustomAttribute<DefOfEntriesAttribute>()!;
            var containingModType = attrib.ContainingMod;
            if (containingModType != null)
                return GameWrapper.Instance.ModLoader.GetModFromType(containingModType);
            return GameWrapper.Instance.ModLoader.GetModFromAssembly(assembly);
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

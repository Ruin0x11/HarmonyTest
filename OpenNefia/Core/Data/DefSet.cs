using OpenNefia.Core.Data.Serial;
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

namespace OpenNefia.Core.Data
{
    internal class DefSet
    {
        private string Filepath;
        public List<Def> Defs { get; internal set; }
        public List<DefCrossRef> CrossRefs { get; internal set; }
        public BaseMod ContainingMod { get; }

        public DefSet(string filepath, BaseMod containingModType, DefDeserializer deserializer)
        {
            this.Filepath = filepath;
            this.Defs = new List<Def>();
            this.CrossRefs = new List<DefCrossRef>();
            this.ContainingMod = containingModType;
            this.Load(deserializer);
        }

        private void Load(DefDeserializer deserializer)
        {
            if (!File.Exists(this.Filepath))
                throw new FileNotFoundException($"Def set file {this.Filepath} does not exist.");

            var doc = new XmlDocument();
            doc.Load(this.Filepath);

            var root = doc.DocumentElement!;
            if (root.Name != "Defs")
            {
                throw new DefLoadException("'Defs' element not found in root");
            }

            foreach (var elem in root.ChildNodes)
            {
                var node = (XmlNode)elem;
                var name = node.Name;

                var defType = DefTypes.GetDefTypeFromName(name);
                if (defType == null)
                    throw new DefLoadException($"Def type '{name}' not found.");

                var defInstance = deserializer.DeserializeDef(defType, node, ContainingMod);

                if (defInstance != null)
                {
                    Defs.Add(defInstance);
                    CrossRefs.AddRange(deserializer.CrossRefs);
                }
            }
        }
    }
}

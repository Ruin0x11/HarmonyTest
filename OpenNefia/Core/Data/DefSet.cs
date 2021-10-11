using OpenNefia.Core.Data.Serial;
using OpenNefia.Game;
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
    public class DefSet
    {
        private string Filepath;
        public List<Def> Defs { get; internal set; }
        public Type ContainingMod { get; }

        public DefSet(string filepath, Type containingModType)
        {
            this.Filepath = filepath;
            this.Defs = new List<Def>();
            this.ContainingMod = containingModType;
            this.Load();
        }

        private void Load()
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

            var deserializer = new DefDeserializer();

            foreach (var elem in root.ChildNodes)
            {
                var node = (XmlNode)elem;
                var name = node.Name;

                var defType = DefTypes.GetDefTypeFromName(name);
                if (defType == null)
                    throw new DefLoadException($"Def type '{name}' not found.");

                var defInstance = deserializer.DeserializeDef(defType, node, ContainingMod);
                Defs.Add(defInstance);
            }
        }
    }
}

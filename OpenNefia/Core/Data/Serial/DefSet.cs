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
using System.Xml.Linq;

namespace OpenNefia.Core.Data.Serial
{
    internal class DefSet
    {
        public string Filepath { get; }
        public XDocument XmlDocument { get; }
        public Dictionary<DefIdentifier, Def> Defs { get; }
        public ModInfo ContainingMod { get; }

        public DefSet(string filepath, ModInfo containingMod, DefDeserializer deserializer)
        {
            this.Filepath = filepath;
            this.XmlDocument = new XDocument(this.Filepath);
            this.Defs = new Dictionary<DefIdentifier, Def>();
            this.ContainingMod = containingMod;
            this.Load(deserializer);
        }

        private void Load(DefDeserializer deserializer)
        {
            if (!File.Exists(this.Filepath))
                throw new FileNotFoundException($"Def set file {this.Filepath} does not exist.");

            var root = XmlDocument.Root!;
            if (root.Name != "Defs")
            {
                throw new DefLoadException("'Defs' element not found in root");
            }

            foreach (var elem in root.Elements())
            {
                var result = deserializer.DeserializeDef(elem, ContainingMod);

                if (result.IsSuccess)
                {
                    var defInstance = result.Value;
                    Defs.Add(defInstance.Identifier, defInstance);
                }
            }
        }
    }
}

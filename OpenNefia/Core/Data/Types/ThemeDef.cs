using OpenNefia.Core.Data.Patch;
using OpenNefia.Core.Data.Serial;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace OpenNefia.Core.Data.Types
{
    public class ThemeOverrides : IDefSerializable, IEnumerable<PatchOperation>
    {
        public List<PatchOperation> Operations = new List<PatchOperation>();

        public void DeserializeDefField(IDefDeserializer deserializer, XElement node, Type containingModType)
        {
            foreach (var childNode in node.XPathSelectElements("Operation"))
            {
                var result = deserializer.DeserializeObject(childNode, typeof(PatchOperation), containingModType);
                if (result.IsFailed)
                {
                    throw new Exception($"Cannot deserialize {childNode}");
                }
                else
                {
                    Operations.Add((PatchOperation)result.Value);
                }
            }
        }

        public IEnumerator<PatchOperation> GetEnumerator() => Operations.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Operations.GetEnumerator();
    }

    public class ThemeDef : Def
    {
        public ThemeDef(string id) : base(id)
        {
            Operations = new ThemeOverrides();
        }

        [DefRequired]
        public ThemeOverrides Operations;
    }
}

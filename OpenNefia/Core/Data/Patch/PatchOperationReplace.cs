using FluentResults;
using OpenNefia.Core.Data.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenNefia.Core.Data.Patch
{
    public class PatchOperationReplace : PatchOperation
    {
        [DefRequired]
        public string XPath = string.Empty;

        [DefRequired]
        public XmlNode Value = null!;

        public override Result<PatchResult> Apply(XmlDocument document)
        {
            var nav = document.CreateNavigator()!;
            var node = nav.SelectSingleNode(this.XPath);

            if (node == null)
            {
                return Result.Fail("XPath not found.");
            }

            node.ReplaceSelf(Value.OuterXml);

            return Result.Ok(PatchOperation.NodeToAffectedDefs(node));
        }
    }
}

using FluentResults;
using OpenNefia.Core.Data.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace OpenNefia.Core.Data.Patch
{
    public class PatchOperationReplace : PatchOperation
    {
        [DefRequired]
        public string XPath = string.Empty;

        [DefRequired]
        public XElement Value = null!;

        public override Result<PatchResult> Apply(XDocument document)
        {
            var element = document.XPathSelectElement(this.XPath);

            if (element == null)
            {
                return Result.Fail("XPath not found.");
            }

            element.ReplaceWith(Value);

            return Result.Ok(new PatchResult());
        }
    }
}

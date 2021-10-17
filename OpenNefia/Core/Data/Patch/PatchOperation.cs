using FluentResults;
using OpenNefia.Core.Data.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace OpenNefia.Core.Data.Patch
{
    public class PatchResult
    {
        public HashSet<DefIdentifier> AffectedDefs = new HashSet<DefIdentifier>();

        public void Merge(PatchResult other)
        {
            foreach (var defIdentifier in other.AffectedDefs)
            {
                this.AffectedDefs.Add(defIdentifier);
            }
        }
    }

    public abstract class PatchOperation : IDefSerializable
    {
        public abstract Result<PatchResult> Apply(XDocument document);

        public virtual void DeserializeDefField(IDefDeserializer deserializer, XElement node, Type containingModType)
        {
            deserializer.PopulateAllFields(node, this, containingModType);
        }

        public static PatchResult NodeToAffectedDefs(XElement? element)
        {
            var patchResult = new PatchResult();

            if (element == null)
            {
                return patchResult;
            }

            var parentElement = element.Parent;
            while (parentElement != null)
            {
                if (parentElement.Name == "Defs" && parentElement.Parent == null)
                {
                    var defIdentResult = DefDeserializer.GetDefIdAndTypeFromElement(element);
                    if (defIdentResult.IsSuccess)
                    {
                        patchResult.AffectedDefs.Add(defIdentResult.Value);
                    }
                    return patchResult;
                }

                element = parentElement;
                parentElement = parentElement.Parent;
            }

            // No defs affected.
            return patchResult;
        }
    }
}

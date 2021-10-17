using FluentResults;
using OpenNefia.Core.Data.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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
        public abstract Result<PatchResult> Apply(XmlDocument document);

        public virtual void DeserializeDefField(IDefDeserializer deserializer, XmlNode node, Type containingModType)
        {
            deserializer.PopulateAllFields(node, this, containingModType);
        }

        public static PatchResult NodeToAffectedDefs(XPathNavigator? nav)
        {
            var patchResult = new PatchResult();

            if (nav == null)
            {
                return patchResult;
            }

            var defsNav = nav.Clone();
            defsNav.MoveToRoot();

            if (!defsNav.MoveToChild("Defs", string.Empty))
            {
                return patchResult;
            }

            var prevNav = nav.Clone();
            while (nav.MoveToParent())
            {
                if (nav.IsSamePosition(defsNav))
                {
                    var defIdentResult = DefDeserializer.GetDefIdAndTypeFromNode(prevNav);
                    if (defIdentResult.IsSuccess)
                    {
                        patchResult.AffectedDefs.Add(defIdentResult.Value);
                    }
                    return patchResult;
                }

                prevNav.MoveToParent();
            }

            // No defs affected.
            return patchResult;
        }
    }
}

using FluentResults;
using OpenNefia.Core.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenNefia.Core.Data.Serial
{
    internal class DefThemeMerger
    {
        public class MergeResult
        {
            public List<Def> Defs;
            public XmlDocument MergedDocument;

            public MergeResult(List<Def> defs, XmlDocument mergedDocument)
            {
                Defs = defs;
                MergedDocument = mergedDocument;
            }
        }

        public Result<MergeResult> Merge(ThemeDef theme)
        {
            var defs = new List<Def>();
            var errors = new List<Error>();
            var defDeserializer = new DefDeserializer();

            var mergedDocument = new XmlDocument();
            var defsElement = mergedDocument.CreateElement("Defs", null);

            foreach (var patch in theme.Operations)
            {
                
            }
            
            foreach (var errorMessage in defDeserializer.Errors)
            {
                errors.Add(new Error(errorMessage));
            }

            if (errors.Count > 0)
            {
                return Result.Fail($"{errors.Count} error(s) during merging.");
            }
            
            return Result.Ok(new MergeResult(defs, mergedDocument));
        }

        private static void MergeDefXml(XmlNode originalXml, XmlNode overrideXml, in XmlElement mergedDefElement)
        {
            // "Class" is the only element treated specially at the top level, so far
            // Everything else in child nodes will be merged.
        }
    }
}

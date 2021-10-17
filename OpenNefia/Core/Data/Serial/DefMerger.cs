using FluentResults;
using OpenNefia.Core.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenNefia.Core.Data.Serial
{
    internal static class DefMerger
    {
        /// <summary>
        /// Merges two defs in-place.
        /// 
        /// The rules are as follows:
        /// 
        /// 1. The two defs must be of the same concrete class. Generally this should not be an issue
        ///    because this system is for merging simple defs like <see cref="FontDef"/> that are unlikely 
        ///    to need subclassing. In the case of <see cref="AssetDef"/> which does need subclassing 
        ///    occasionally, I'm thinking the asset management system will be able to retrieve the new asset 
        ///    instance from the store when it detects a hotload instead of needing to rely on this merging
        ///    system in the first place.
        /// 2. Only public or private fields of the class will be merged. Properties will be ignored.
        /// 3. After the merge completes, <see cref="Def.OnMerge()"/> is called so the def can clean up
        ///    any properties or other data it needs to, like cached LÖVE objects.
        /// 
        /// </summary>
        /// <param name="mergingInto"></param>
        /// <param name="sourcingFrom"></param>
        /// <returns></returns>
        public static Result Merge(Def mergingInto, Def sourcingFrom)
        {
            var errors = new List<Error>();

            var t1 = mergingInto.GetType();
            var t2 = sourcingFrom.GetType();
            if (t1 != t2)
            {
                return Result.Fail($"Cannot merge defs of type {t1} and {t2}.");
            }
            Console.WriteLine($"Merge {sourcingFrom} into {mergingInto}");

            foreach (var field in t1.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                try
                {
                    var sourcingFromValue = field.GetValue(sourcingFrom);
                    field.SetValue(mergingInto, sourcingFromValue);
                }
                catch (Exception ex)
                {
                    errors.Add(new Error($"Could not merge field {t1.Name}.{field.Name}").CausedBy(ex));
                }
            }

            if (errors.Count > 0)
            {
                return Result.Fail("Errors occurred during def merging.").WithErrors(errors);
            }

            try
            {
                mergingInto.OnMerge();
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error($"Error during {t1.Name}.OnMerge() callback").CausedBy(ex));
            }

            return Result.Ok();
        }
    }
}

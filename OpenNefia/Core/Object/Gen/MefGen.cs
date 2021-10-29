using FluentResults;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Object
{
    public class MefGenOpts : MapObjectGenOpts
    {
    }

    public static class MefGen
    {
        public static Result<Mef> Create(MefDef def, MefGenOpts? opts = null)
        {
            return MapObjectGen.Create(def, opts).ToResult(v => (Mef)v);
        }

        public static Result<Mef> Create(MefDef def, IMapObjectHolder holder, MefGenOpts? opts = null)
        {
            return MapObjectGen.Create(def, holder, opts).ToResult(v => (Mef)v);
        }

        public static Result<Mef> Create(MefDef def, TilePos pos, MefGenOpts? opts = null)
        {
            return MapObjectGen.Create(def, pos, opts).ToResult(v => (Mef)v);
        }
    }
}

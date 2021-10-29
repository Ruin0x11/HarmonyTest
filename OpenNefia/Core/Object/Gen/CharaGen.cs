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
    public class CharaGenOpts : MapObjectGenOpts
    {
    }

    public static class CharaGen
    {
        public static Result<Chara> Create(CharaDef def, CharaGenOpts? opts = null)
        {
            return MapObjectGen.Create(def, opts).ToResult(v => (Chara)v);
        }

        public static Result<Chara> Create(CharaDef def, IMapObjectHolder holder, CharaGenOpts? opts = null)
        {
            return MapObjectGen.Create(def, holder, opts).ToResult(v => (Chara)v);
        }

        public static Result<Chara> Create(CharaDef def, TilePos pos, CharaGenOpts? opts = null)
        {
            return MapObjectGen.Create(def, pos, opts).ToResult(v => (Chara)v);
        }
    }
}

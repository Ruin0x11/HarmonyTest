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
    public class ItemGenOpts : MapObjectGenOpts
    {
    }

    public static class ItemGen
    {
        public static Result<Item> Create(ItemDef def, ItemGenOpts? opts = null)
        {
            return MapObjectGen.Create(def, opts).ToResult(v => (Item)v);
        }

        public static Result<Item> Create(ItemDef def, IMapObjectHolder holder, ItemGenOpts? opts = null)
        {
            return MapObjectGen.Create(def, holder, opts).ToResult(v => (Item)v);
        }

        public static Result<Item> Create(ItemDef def, TilePos pos, ItemGenOpts? opts = null)
        {
            return MapObjectGen.Create(def, pos, opts).ToResult(v => (Item)v);
        }
    }
}

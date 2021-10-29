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
    public static class ItemGen
    {
        public static Result<Item> Create(ItemDef def)
        {
            return MapObjectGen.Create(def).ToResult(v => (Item)v);
        }

        public static Result<Item> Create(ItemDef def, IMapObjectHolder holder)
        {
            return MapObjectGen.Create(def, holder).ToResult(v => (Item)v);
        }

        public static Result<Item> Create(ItemDef def, TilePos pos)
        {
            return MapObjectGen.Create(def, pos).ToResult(v => (Item)v);
        }
    }
}

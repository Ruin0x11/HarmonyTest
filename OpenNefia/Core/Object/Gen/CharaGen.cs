﻿using FluentResults;
using OpenNefia.Core.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Object
{
    public static class CharaGen
    {
        public static Result<Chara> Create(CharaDef def)
        {
            return MapObjectGen.Create(def).ToResult(v => (Chara)v);
        }

        public static Result<Chara> Create(CharaDef def, IMapObjectHolder holder)
        {
            return MapObjectGen.Create(def, holder).ToResult(v => (Chara)v);
        }

        public static Result<Chara> Create(CharaDef def, TilePos pos)
        {
            return MapObjectGen.Create(def, pos).ToResult(v => (Chara)v);
        }
    }
}

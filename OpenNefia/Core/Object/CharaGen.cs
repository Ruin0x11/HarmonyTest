using FluentResults;
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
        public static Result<Chara> Create()
        {
            var chara = new Chara(ChipDefOf.CharaRaceSlime);

            return Result.Ok(chara);
        }
    }
}

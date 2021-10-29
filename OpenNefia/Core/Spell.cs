using FluentResults;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Effect;
using OpenNefia.Core.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core
{
    public static class Spell
    {
        public static Result<EffectResult> CastSpell(SpellDef spellDef, 
            Chara target,
            MapObject? source = null,
            int power = 0,
            int? tileX = null,
            int? tileY = null,
            int? tileRange = null,
            CurseState curseState = CurseState.Normal,
            TriggeredBy triggeredBy = TriggeredBy.Unknown)
        {
            if (!tileRange.HasValue)
                tileRange = spellDef.TileRange;

            var args = new EffectArguments(source, power, tileX, tileY, tileRange.Value, curseState, triggeredBy);
            return CastSpell(spellDef, target, args);
        }

        public static Result<EffectResult> CastSpell(SpellDef spellDef, Chara target, EffectArguments args)
        {
            var spellResult = spellDef.Apply(target, args);
            return Result.Ok(spellResult);
        }
    }
}

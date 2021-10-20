using OpenNefia.Core.Object;
using OpenNefia.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Logic
{
    public static class CharaAction
    {
        public static TurnResult Move(Chara chara, int newX, int newY)
        {
            chara.Direction = PosUtils.GetDirectionTowards(chara.X, chara.Y, newX, newY);

            var map = chara.GetCurrentMap();

            if (map == null || !map.CanPassThrough(newX, newY))
            {
                return TurnResult.TurnEnd;
            }

            chara.SetPosition(newX, newY);

            return TurnResult.TurnEnd;
        }
    }
}

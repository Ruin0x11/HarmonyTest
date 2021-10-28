using OpenNefia.Core.Object;
using OpenNefia.Core.Object.Aspect;
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
            if (!chara.IsAlive)
            {
                return TurnResult.TurnEnd;
            }

            chara.Direction = PosUtils.GetDirectionTowards(chara.X, chara.Y, newX, newY);

            var map = chara.GetCurrentMap();

            if (map == null || !map.CanPassThrough(newX, newY))
            {
                return TurnResult.TurnEnd;
            }

            chara.SetPosition(newX, newY);

            return TurnResult.TurnEnd;
        }

        public static TurnResult Drink(Chara chara, Item item)
        {
            if (!chara.IsAlive || item.IsAlive)
            {
                return TurnResult.TurnEnd;
            }

            var result = TurnResult.RestartPlayerTurn;
            var shouldConsume = false;

            foreach (var aspect in item.GetAspects<ICanDrinkAspect>().Where(a => a.CanDrink(chara)))
            {
                var drinkResult = aspect.OnDrink(chara);
                switch (drinkResult)
                {
                    case TurnResult.TurnEnd:
                        result = TurnResult.TurnEnd;
                        shouldConsume |= aspect.ShouldConsumeOnDrink;
                        break;
                    default:
                        break;
                }
            }

            if (shouldConsume)
            {
                item.Amount -= 1;
            }

            return result;
        }

        public static bool TakeItem(Chara chara, Item item)
        {
            return chara.Inventory.TakeOrTransferItem(item);
        }

        public static bool DropItem(Chara chara, Item item)
        {
            var map = chara.GetCurrentMap();
            if (map == null)
                return false;

            return map.TakeOrTransferObject(item, chara.X, chara.Y);
        }
    }
}

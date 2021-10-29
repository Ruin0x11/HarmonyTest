using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Map;
using OpenNefia.Core.Object;
using OpenNefia.Core.Object.Aspect;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.UI.Layer;
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

            var map = chara.GetContainingMap();

            if (map == null || !map.CanPassThrough(newX, newY))
            {
                return TurnResult.TurnEnd;
            }

            chara.SetPosition(newX, newY);

            TriggerSteppedOn(chara);

            return TurnResult.TurnEnd;
        }

        private static void TriggerSteppedOn(Chara chara)
        {
            foreach (var mef in chara.GetTilePos()!.Value.GetMapObjects<Mef>())
            {
                mef.OnSteppedOn(chara);
            }
        }

        public static TurnResult Drink(Chara chara, Item item)
        {
            if (!chara.IsAlive || !item.IsAlive)
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
                item.Consume(1);
            }

            return result;
        }

        public static TurnResult Throw(Chara chara, Item item, TilePos targetPos)
        {
            if (!chara.IsAlive || !item.IsAlive || !item.GetAspects<ICanThrowAspect>().Any(i => i.CanThrow(chara)))
            {
                return TurnResult.TurnEnd;
            }

            Current.Field!.MapDrawables.Enqueue(new RangedAttackMapDrawable(chara.GetTilePos()!.Value, targetPos, item.Chip, item.Color), chara.GetTilePos()!.Value);

            var shouldConsume = false;

            foreach (var aspect in item.GetAspects<ICanThrowAspect>())
            {
                shouldConsume |= aspect.OnThrownImpact(targetPos);
            }

            if (shouldConsume)
            {
                Sounds.PlayOneShot(SoundDefOf.Crush2, targetPos);
                item.Consume(1);
            }
            else
            {
                var split = item.SplitOff(1);

                if (split != null)
                {
                    MapUtils.TrySpawn(split, targetPos);
                }
            }

            return TurnResult.TurnEnd;
        }

        public static bool TakeItem(Chara chara, Item item)
        {
            return chara.Inventory.TakeOrTransferItem(item);
        }

        public static bool DropItem(Chara chara, Item item)
        {
            var map = chara.GetContainingMap();
            if (map == null)
                return false;

            return map.TakeOrTransferObject(item, chara.X, chara.Y);
        }
    }
}

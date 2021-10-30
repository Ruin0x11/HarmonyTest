using OpenNefia.Core.Map;
using OpenNefia.Core.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Logic
{
    public static class TargetText
    {
        [Localize]
        public static string TextOutOfSight = string.Empty;

        [Localize]
        public static LocaleFunc<Chara, int> TextYouAreTargeting = null!;

        [Localize]
        public static Dictionary<int, LocaleFunc<Chara>> TextTargetLevel = new Dictionary<int, LocaleFunc<Chara>>();

        public static bool GetTargetText(Chara onlooker, TilePos pos, out string text, bool visibleOnly = false)
        {
            if (visibleOnly && !onlooker.CanSee(pos))
            {
                text = TextOutOfSight;
                return false;
            }

            var target = pos.GetPrimaryChara();

            var builder = new StringBuilder();

            if (target != null && onlooker.CanSee(target))
            {
                var dist = onlooker.GetTilePos()!.Value.DistanceTo(target.GetTilePos()!.Value);
                var targetLevelText = GetTargetDangerText(onlooker, target);
                builder.AppendLine(TextYouAreTargeting(target, dist));
                builder.AppendLine(targetLevelText);
            }

            var item = pos.GetMapObjects<Item>().FirstOrDefault();
            if (item != null)
            {
                builder.AppendLine(GetTargetItemText(item));
            }

            var feat = pos.GetMapObjects<Feat>();
            if (feat != null)
            {
                builder.AppendLine(GetTargetFeatTexts(pos));
            }

            text = builder.ToString();

            return true;
        }

        public static string GetTargetDangerText(Chara onlooker, Chara target)
        {
            var level = 0;
            var fn = TextTargetLevel.GetValueOrDefault(level);
            if (fn == null)
            {
                return string.Empty;
            }
            return fn(onlooker);
        }

        private static string GetTargetItemText(Item item)
        {
            return item.ToString()!;
        }

        private static string GetTargetFeatTexts(TilePos pos)
        {
            var builder = new StringBuilder();
            foreach (var feat in pos.GetMapObjects<Feat>())
            {
                builder.AppendLine(feat.ToString());
            }
            return builder.ToString();
        }
    }
}

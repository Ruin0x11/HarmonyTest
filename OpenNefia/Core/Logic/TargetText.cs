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
        public static bool GetTargetText(Chara onlooker, TilePos pos, out string text, bool visibleOnly = false)
        {
            if (visibleOnly && !onlooker.CanSee(pos))
            {
                text = "﻿視界範囲外";
                return false;
            }

            var target = pos.GetPrimaryChara();

            var builder = new StringBuilder();

            if (target != null && onlooker.CanSee(target))
            {
                var dist = onlooker.GetTilePos()!.Value.DistanceTo(target.GetTilePos()!.Value);
                var targetLevelText = GetTargetDangerText(onlooker, target);
                builder.AppendLine(targetLevelText);
                builder.AppendLine($"Targeting (dist: {dist})");
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
            return "danger level";
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

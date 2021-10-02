using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenNefia.Core.Rendering.AssetDrawable;

namespace OpenNefia.Core.UI.Element
{
    public class UiWindowBacking : BaseUiElement
    {
        private AssetDrawable? AssetWindow;
        private Love.SpriteBatch? Batch;
        private bool Shadow;

        public UiWindowBacking(bool shadow)
        {
            this.Shadow = shadow;
        }

        public override void Relayout(int x, int y, int width, int height)
        {
            base.Relayout(x, y, width, height);

            var x_inner = width + x - width % 8 - 64;
            var y_inner = height + y - height % 8 - 64;

            y_inner = Math.Max(y_inner, y + 14);

            var parts = new List<AssetBatchPart>();

            if (!this.Shadow)
            {
                parts.Add(new AssetBatchPart("top_left", x, y));
            }
            parts.Add(new AssetBatchPart("top_right", x_inner, y));
            parts.Add(new AssetBatchPart("bottom_left", x, y_inner));
            parts.Add(new AssetBatchPart("bottom_right", x_inner, y_inner));

            for (int dx = 8; dx < width / 8 - 8; dx++)
            {
                var tile = Math.Abs((dx - 8) % 18);
                if (!this.Shadow)
                {
                    parts.Add(new AssetBatchPart($"top_mid_{tile}", dx * 8 + x, y));
                }
                parts.Add(new AssetBatchPart($"bottom_mid_{tile}", dx * 8 + x, y_inner));
            }

            for (int dy = 0; dy < height / 8 - 13; dy++)
            {
                var tile_y = dy % 12;
                if (!this.Shadow)
                {
                    parts.Add(new AssetBatchPart($"mid_left_{tile_y}", x, dy * 8 + y + 48));

                    for (int dx = 0; dx < width / 8 - 14; dx++)
                    {
                        var tile_x = Math.Abs((dx - 8) % 18);
                        parts.Add(new AssetBatchPart($"mid_mid_{tile_y}_{tile_x}", dx * 8 + x + 56, dy * 8 + y + 48));
                    }
                }
                parts.Add(new AssetBatchPart($"mid_right_{tile_y}", x_inner, dy * 8 + y + 48));
            }

            this.AssetWindow = new AssetDrawable(Asset.Entries.Window, this.Width, this.Height);
            this.Batch = this.AssetWindow.MakeBatch(parts);
        }

        public override void Update(float dt)
        {
        }

        public override void Draw()
        {
            Drawing.DrawImage(this.AssetWindow!.Image!);
            Drawing.DrawSpriteBatch(this.Batch!, 0, 0);
        }
    }
}

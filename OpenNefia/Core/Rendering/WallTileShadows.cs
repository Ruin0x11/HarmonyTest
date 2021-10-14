using OpenNefia.Core.Data.Types;
using OpenNefia.Core.UI.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Rendering
{
    public class WallTileShadows : BaseUiElement
    {
        private InstancedMap Map;
        private ICoords Coords;

        private HashSet<int> TopShadows;
        private HashSet<int> BottomShadows;

        public WallTileShadows(InstancedMap map, ICoords coords)
        {
            this.Map = map;
            this.Coords = coords;
            TopShadows = new HashSet<int>();
            BottomShadows = new HashSet<int>();
        }

        public void OnThemeSwitched(ICoords coords)
        {
            this.Coords = coords;
        }

        public void SetTile(int x, int y, TileDef tile)
        {
            var tileIndex = tile.Tile.TileIndex;
            if (tile.Wall != null)
            {
                var oneTileDown = Map.GetTile(x, y + 1);
                if (oneTileDown != null && oneTileDown.Wall == null && Map.IsMemorized(x, y + 1))
                {
                    tileIndex = tile.Wall.TileIndex;
                    BottomShadows.Add(y * Map.Width + x);
                }
                else
                {
                    BottomShadows.Remove(y * Map.Width + x);
                    TopShadows.Remove((y + 1) * Map.Width + x);
                }

                var oneTileUp = Map.GetTile(x, y - 1);
                if (oneTileUp != null && oneTileUp.Wall != null && Map.IsMemorized(x, y - 1))
                {
                    TopShadows.Remove(y * Map.Width + x);
                    BottomShadows.Remove((y - 1) * Map.Width + x);
                }
                else
                {
                    TopShadows.Add(y * Map.Width + x);
                }
            }
            else
            {
                TopShadows.Remove(y * Map.Width + x);
                var oneTileUp = Map.GetTile(x, y - 1);
                if (oneTileUp != null && oneTileUp.Wall != null && Map.IsMemorized(x, y - 1))
                {
                    BottomShadows.Add((y - 1) * Map.Width + x);
                }
                else
                {
                    BottomShadows.Remove((y - 1) * Map.Width + x);
                }
            }
        }

        public void Clear()
        {
            TopShadows.Clear();
            BottomShadows.Clear();
        }

        public override void Update(float dt)
        {
        }

        public override void Draw()
        {
            Coords.GetSize(out var tileW, out var tileH);

            Love.Graphics.SetBlendMode(Love.BlendMode.Subtract);
            GraphicsEx.SetColor(255, 255, 255, 20);

            foreach (var index in TopShadows)
            {
                var tileX = index % Map.Width;
                var tileY = index / Map.Height;
                GraphicsEx.FilledRect(tileX * tileW + X, tileY * tileH + Y - 20, tileW, tileH / 6);
            }

            foreach (var index in BottomShadows)
            {
                var tileX = index % Map.Width;
                var tileY = index / Map.Height;

                GraphicsEx.SetColor(255, 255, 255, 16);
                GraphicsEx.FilledRect(tileX * tileW + X, (tileY + 1) * tileH + Y, tileW, tileH / 2);

                GraphicsEx.SetColor(255, 255, 255, 12);
                GraphicsEx.FilledRect(tileX * tileW + X, (tileY + 1) * tileH + Y + tileW / 2, tileW, tileH / 4);
            }

            Love.Graphics.SetBlendMode(Love.BlendMode.Alpha);
            GraphicsEx.SetColor(255, 255, 255, 255);
        }
    }
}

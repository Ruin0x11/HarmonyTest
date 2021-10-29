using OpenNefia.Core.Rendering;
using System;

namespace OpenNefia.Core.Map
{
    internal class ShadowMap
    {
        private InstancedMap Map;
        private ICoords Coords;
        internal ShadowTile[] ShadowTiles;

        public ShadowMap(InstancedMap map)
        {
            Map = map;
            Coords = GraphicsEx.Coords;
            ShadowTiles = new ShadowTile[map.Width * map.Height];
        }

        private void SetShadowBorder(int tx, int ty, ShadowTile shadow)
        {
            var ind = ty * Map.Width + tx;

            if (ind >= 0 && ind < ShadowTiles.Length)
                ShadowTiles[ty * Map.Width + tx] |= shadow;
        }

        private void MarkShadow(int tx, int ty)
        {
            SetShadowBorder(tx + 1, ty, ShadowTile.West);
            SetShadowBorder(tx - 1, ty, ShadowTile.East);
            SetShadowBorder(tx, ty - 1, ShadowTile.North);
            SetShadowBorder(tx, ty + 1, ShadowTile.South);
            SetShadowBorder(tx + 1, ty + 1, ShadowTile.Northwest);
            SetShadowBorder(tx - 1, ty - 1, ShadowTile.Southeast);
            SetShadowBorder(tx + 1, ty - 1, ShadowTile.Southwest);
            SetShadowBorder(tx - 1, ty + 1, ShadowTile.Northeast);
        }

        internal void RefreshVisibility()
        {
            Array.Clear(ShadowTiles, 0, ShadowTiles.Length);

            var player = Current.Game.Player!;
            var playerX = player.X;
            var playerY = player.Y;

            GraphicsEx.GetWindowTiledSize(out var windowTiledW, out var windowTiledH);

            windowTiledW = Math.Min(windowTiledW, Map.Width);
            windowTiledH = Math.Min(windowTiledH, Map.Height);

            var startX = Math.Clamp(playerX - windowTiledW / 2 - 1, 0, Map.Width - windowTiledW);
            var startY = Math.Clamp(playerY - windowTiledH / 2 - 1, 0, Map.Height - windowTiledH);
            var endX = startX + windowTiledW + 2;
            var endY = startY + windowTiledH + 2;

            var fovSize = 15;
            var fovRadius = FovRadius.Get(fovSize);
            var radius = fovSize / 2 + 1;

            var fovYStart = playerY - fovSize / 2;
            var fovYEnd = playerY + fovSize / 2;

            var cx = playerX - radius;
            var cy = radius - playerY;

            for (int j = startY; j < endY; j++)
            {
                if (j < 0 || j >= Map.Height)
                {
                    for (int i = startX; i < endX; i++)
                    {
                        MarkShadow(i, j);
                    }
                }
                else
                {
                    for (int i = startX; i < endX; i++)
                    {
                        if (i < 0 || i >= Map.Width)
                        {
                            MarkShadow(i, j);
                        }
                        else
                        {
                            var shadow = true;

                            if (fovYStart <= j && j <= fovYEnd)
                            {
                                if (i >= fovRadius[j + cy, 0] + cx && i < fovRadius[j + cy, 1] + cx)
                                {
                                    if (Map.HasLos(playerX, playerY, i, j))
                                    {
                                        Map.MemorizeTile(i, j);
                                        shadow = false;
                                    }
                                }
                            }

                            if (shadow)
                            {
                                SetShadowBorder(i, j, ShadowTile.IsShadow);
                                MarkShadow(i, j);
                            }
                        }
                    }
                }
            }
        }
    }
}

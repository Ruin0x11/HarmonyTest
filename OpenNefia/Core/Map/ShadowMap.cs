using Love;
using OpenNefia.Core.Rendering;
using System;

namespace OpenNefia.Core.Map
{
    internal class ShadowMap
    {
        private InstancedMap Map;
        private ICoords Coords;
        internal ShadowTile[] ShadowTiles;
        internal Rectangle ShadowBounds;

        public ShadowMap(InstancedMap map)
        {
            Map = map;
            Coords = GraphicsEx.Coords;
            ShadowTiles = new ShadowTile[map.Width * map.Height];
            ShadowBounds = new Rectangle();
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

            var startX = Math.Clamp(playerX - windowTiledW / 2 - 2, 0, Map.Width - windowTiledW);
            var startY = Math.Clamp(playerY - windowTiledH / 2 - 2, 0, Map.Height - windowTiledH);
            var endX = startX + windowTiledW + 4;
            var endY = startY + windowTiledH + 4;

            Coords.TileToScreen(startX, startY, out var scsx, out var scsy);
            Coords.TileToScreen(endX - 1, endY - 1, out var scex, out var scey);
            ShadowBounds.X = scsx;
            ShadowBounds.Y = scsy;
            ShadowBounds.Width = scex - scsx;
            ShadowBounds.Height = scey - scsy;

            var fovSize = 15;
            var fovRadius = FovRadius.Get(fovSize);
            var radius = fovSize / 2 + 1;

            var fovYStart = playerY - fovSize / 2;
            var fovYEnd = playerY + fovSize / 2;

            var cx = playerX - radius;
            var cy = radius - playerY;

            if (startX > 0)
            {
                for (int y = startY; y < endY; y++)
                {
                    SetShadowBorder(startX, y, ShadowTile.West);
                }
            }
            if (endX - 4 < Map.Width - windowTiledW)
            {
                for (int y = startY; y < endY; y++)
                {
                    SetShadowBorder(endX - 2, y, ShadowTile.East);
                }
            }
            if (startY > 0)
            {
                for (int x = startX; x < endX; x++)
                {
                    SetShadowBorder(x, startY, ShadowTile.South);
                }
            }
            if (endY - 4 < Map.Height)
            {
                for (int x = startX; x < endX; x++)
                {
                    SetShadowBorder(x, endY - 2, ShadowTile.North);
                }
            }

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

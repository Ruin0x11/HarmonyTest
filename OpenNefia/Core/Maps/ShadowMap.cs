﻿using Love;
using OpenNefia.Core.Game;
using OpenNefia.Core.Maths;
using OpenNefia.Core.Rendering;
using System;

namespace OpenNefia.Core.Maps
{
    public sealed class ShadowMap
    {
        private IMap Map;
        private ICoords Coords;
        internal ShadowTile[] ShadowTiles;
        internal Vector2i ShadowPos;
        internal Vector2i ShadowSize;

        public ShadowMap(IMap map)
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

            var player = GameSession.Player!;
            var playerPos = player.Coords.Position;

            GraphicsEx.GetWindowTiledSize(out var windowTiledW, out var windowTiledH);

            windowTiledW = Math.Min(windowTiledW, Map.Width);
            windowTiledH = Math.Min(windowTiledH, Map.Height);

            var start = new Vector2i(Math.Clamp(playerPos.X - windowTiledW / 2 - 2, 0, Map.Width - windowTiledW),
                                    Math.Clamp(playerPos.Y - windowTiledH / 2 - 2, 0, Map.Height - windowTiledH));
            var end = new Vector2i(start.X + windowTiledW + 4, start.Y + windowTiledH + 4);

            Coords.TileToScreen(start, ref ShadowPos);
            Coords.TileToScreen(end - 1, ref ShadowSize);
            ShadowSize -= ShadowPos;

            var fovSize = 15;
            var fovRadius = FovRadius.Get(fovSize);
            var radius = fovSize / 2 + 1;

            var fovYStart = playerPos.Y - fovSize / 2;
            var fovYEnd = playerPos.Y + fovSize / 2;

            var cx = playerPos.X - radius;
            var cy = radius - playerPos.Y;

            if (start.X > 0)
            {
                for (int y = start.Y; y < end.Y; y++)
                {
                    SetShadowBorder(start.X, y, ShadowTile.West);
                }
            }
            if (end.X - 4 < Map.Width - windowTiledW)
            {
                for (int y = start.Y; y < end.Y; y++)
                {
                    SetShadowBorder(end.X - 2, y, ShadowTile.East);
                }
            }
            if (start.Y > 0)
            {
                for (int x = start.X; x < end.X; x++)
                {
                    SetShadowBorder(x, start.Y, ShadowTile.South);
                }
            }
            if (end.Y - 4 < Map.Height)
            {
                for (int x = start.X; x < end.X; x++)
                {
                    SetShadowBorder(x, end.Y - 2, ShadowTile.North);
                }
            }

            for (int j = start.Y; j < end.Y; j++)
            {
                if (j < 0 || j >= Map.Height)
                {
                    for (int i = start.X; i < end.X; i++)
                    {
                        MarkShadow(i, j);
                    }
                }
                else
                {
                    for (int i = start.X; i < end.X; i++)
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
                                    var coords = new MapCoordinates(player.MapId, i, j);
                                    if (MapQuery.HasLos(player.Coords, coords))
                                    {
                                        Map.MemorizeTile(coords.Position);
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

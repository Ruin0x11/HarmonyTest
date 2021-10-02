using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia
{
    public class InstancedMap
    {
        int Width;
        int Height;
        List<int> tiles;

        public InstancedMap(int width, int height)
        {
            Width = width;
            Height = height;
            tiles = new List<int>(Width * Height);
            for (int i = 0; i < Width * Height; i++)
            {
                tiles.Add(i % 5);
            }
        }

        public void Set(int x, int y, int tile)
        {
            tiles[y * Width + x] = tile;
        }

        public int Get(int x, int y)
        {
            return tiles[y * Width + x];
        }

        public void Draw(TileBatch batch, int sx, int sy)
        {
            batch.Clear();

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int tile = Get(x, y);
                    batch.Add(tile, x * 48, y * 48);
                }
            }

            batch.Draw(sx, sy);
        }
    }
}

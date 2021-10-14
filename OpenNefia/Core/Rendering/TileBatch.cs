using Love;
using OpenNefia.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Rendering
{
    internal class TileBatch
    {
        int TiledWidth;
        int TiledHeight;

        string[] Tiles;
        TileBatchRow[] Rows;
        HashSet<int> DirtyRows;
        bool RedrawAll;
        private ICoords Coords;
        private TileAtlas? Atlas;

        public TileBatch(int width, int height, TileAtlas atlas, ICoords coords)
        {
            TiledWidth = width;
            TiledHeight = height;
            Atlas = atlas;
            Coords = coords;
            Tiles = new string[width * height];
            Rows = new TileBatchRow[height];
            DirtyRows = new HashSet<int>();
            RedrawAll = true;
            for (int i = 0; i < height; i++)
            {
                Rows[i] = new TileBatchRow(Atlas);
            }
            OnThemeSwitched(Atlas, Coords);
        }

        public void OnThemeSwitched(TileAtlas atlas, ICoords coords)
        {
            Atlas = atlas;
            Coords = coords;
            RedrawAll = true;
            foreach (var row in Rows)
            {
                row.OnThemeSwitched(atlas);
            }
        }

        public void SetTile(int x, int y, string tile)
        {
            Tiles[y * TiledWidth + x] = tile;
            DirtyRows.Add(y);
        }

        public void UpdateBatches()
        {
            if (RedrawAll)
            {
                for (int y = 0; y < Rows.Length; y++)
                {
                    var row = Rows[y];
                    row.UpdateBatch(Atlas!, Tiles, y * TiledWidth, TiledWidth);
                }
            }
            else
            {
                foreach (int y in DirtyRows)
                {
                    var row = Rows[y];
                    row.UpdateBatch(Atlas!, Tiles, y * TiledWidth, TiledWidth);
                }
            }
            RedrawAll = false;
            DirtyRows.Clear();
        }

        public void Update(float dt)
        {
        }

        public void Draw(int screenX, int screenY)
        {
            for (int tileY = 0; tileY < Rows.Length; tileY++)
            {
                var row = Rows[tileY];
                row.Draw(screenX, screenY + Constants.TILE_SIZE * tileY);
            }
        }
    }

    internal class TileBatchRow
    {
        private SpriteBatch Batch;
        private int TileWidth;

        public TileBatchRow(TileAtlas atlas)
        {
            Batch = Love.Graphics.NewSpriteBatch(atlas.Image, 2048, Love.SpriteBatchUsage.Dynamic);
            TileWidth = Constants.TILE_SIZE;
        }

        internal void OnThemeSwitched(TileAtlas atlas)
        {
            Batch = Love.Graphics.NewSpriteBatch(atlas.Image, 2048, Love.SpriteBatchUsage.Dynamic);
        }

        internal void UpdateBatch(TileAtlas tileAtlas, string[] tiles, int startIndex, int width)
        {
            Batch.Clear();
            for (int x = 0; x < width; x++)
            {
                var index = startIndex + x;
                var tileId = tiles[index];
                var tile = tileAtlas.GetTile(tileId);
                if (tile == null)
                {
                    Logger.Error($"Missing tile {tileId}");
                }
                else
                {
                    Batch.Add(tile.Quad, TileWidth * x, 0);
                }
            }
            Batch.Flush();
        }

        public void Draw(int screenX, int screenY)
        {
            Love.Graphics.Draw(Batch, screenX, screenY);
        }
    }
}

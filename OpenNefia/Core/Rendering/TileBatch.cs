using Love;
using OpenNefia.Core.UI;
using OpenNefia.Core.UI.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Rendering
{
    internal class TileBatch : BaseDrawable
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
                Rows[i] = new TileBatchRow(Atlas, width);
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

        public void Clear()
        {
            this.RedrawAll = true;
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
                    row.UpdateBatches(Atlas!, Tiles, y * TiledWidth, TiledWidth);
                }
            }
            else
            {
                foreach (int y in DirtyRows)
                {
                    var row = Rows[y];
                    row.UpdateBatches(Atlas!, Tiles, y * TiledWidth, TiledWidth);
                }
            }
            RedrawAll = false;
            DirtyRows.Clear();
        }

        public override void Update(float dt)
        {
        }

        public override void Draw()
        {
            for (int tileY = 0; tileY < Rows.Length; tileY++)
            {
                var row = Rows[tileY];
                row.Draw(X, Y + Constants.TILE_SIZE * tileY);
            }
        }
    }

    internal class TileBatchRow
    {
        private SpriteBatch Batch;
        private SpriteBatch OverhangBatch;
        private int TileWidth;
        private int ScreenWidth;
        private bool HasOverhang = false;

        const int OVERHANG_HEIGHT = Constants.TILE_SIZE / 4;

        public TileBatchRow(TileAtlas atlas, int widthInTiles)
        {
            Batch = Love.Graphics.NewSpriteBatch(atlas.Image, 2048, Love.SpriteBatchUsage.Dynamic);
            OverhangBatch = Love.Graphics.NewSpriteBatch(atlas.Image, 2048, Love.SpriteBatchUsage.Dynamic);
            TileWidth = Constants.TILE_SIZE;
            ScreenWidth = widthInTiles * TileWidth;
        }

        internal void OnThemeSwitched(TileAtlas atlas)
        {
            Batch = Love.Graphics.NewSpriteBatch(atlas.Image, 2048, Love.SpriteBatchUsage.Dynamic);
            OverhangBatch = Love.Graphics.NewSpriteBatch(atlas.Image, 2048, Love.SpriteBatchUsage.Dynamic);
        }

        internal void UpdateBatches(TileAtlas tileAtlas, string[] tiles, int startIndex, int widthInTiles)
        {
            ScreenWidth = widthInTiles * TileWidth;
            Batch.Clear();
            OverhangBatch.Clear();
            HasOverhang = false;
            
            for (int x = 0; x < widthInTiles; x++)
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

                    if (tile.HasOverhang)
                    {
                        HasOverhang = true;
                        OverhangBatch.Add(tile.Quad, TileWidth * x, 0);
                    }
                }
            }

            Batch.Flush();
            OverhangBatch.Flush();
        }

        public void Draw(int screenX, int screenY)
        {
            Love.Graphics.Draw(Batch, screenX, screenY);

            if (HasOverhang)
            {
                GraphicsEx.SetScissor(screenX, screenY - OVERHANG_HEIGHT, ScreenWidth, OVERHANG_HEIGHT);
                Love.Graphics.Draw(OverhangBatch, screenX, screenY - OVERHANG_HEIGHT);
                GraphicsEx.SetScissor();
            }
        }
    }
}

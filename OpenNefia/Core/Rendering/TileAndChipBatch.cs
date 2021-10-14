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
    /// <summary>
    /// A tile batch for rendering "strips" of map tile/chip batches with proper Z-ordering.
    /// </summary>
    internal class TileAndChipBatch : BaseDrawable
    {
        int TiledWidth;
        int TiledHeight;

        string[] Tiles;
        TileBatchRow[] Rows;
        HashSet<int> DirtyRows;
        bool RedrawAll;
        private ICoords Coords;
        private TileAtlas TileAtlas;
        private TileAtlas ChipAtlas;

        public TileAndChipBatch(int width, int height, ICoords coords)
        {
            TiledWidth = width;
            TiledHeight = height;
            TileAtlas = Atlases.Tile;
            ChipAtlas = Atlases.Chip;
            Coords = coords;
            Tiles = new string[width * height];
            Rows = new TileBatchRow[height];
            DirtyRows = new HashSet<int>();
            RedrawAll = true;
            for (int tileY = 0; tileY < height; tileY++)
            {
                Rows[tileY] = new TileBatchRow(TileAtlas, ChipAtlas, width, Constants.TILE_SIZE * tileY);
            }
        }

        public void OnThemeSwitched(TileAtlas tileAtlas, TileAtlas chipAtlas, ICoords coords)
        {
            TileAtlas = tileAtlas;
            ChipAtlas = chipAtlas;
            Coords = coords;
            RedrawAll = true;
            foreach (var row in Rows)
            {
                row.OnThemeSwitched(TileAtlas, ChipAtlas);
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
                    row.UpdateBatches(Tiles, y * TiledWidth, TiledWidth);
                }
            }
            else
            {
                foreach (int y in DirtyRows)
                {
                    var row = Rows[y];
                    row.UpdateBatches(Tiles, y * TiledWidth, TiledWidth);
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
                row.Draw(X, Y);
            }
        }
    }

    internal class TileBatchRow
    {
        private SpriteBatch TileBatch;
        private SpriteBatch ChipBatch;
        private SpriteBatch TileOverhangBatch;
        private int TileWidth;
        private int YOffset;
        private int ScreenWidth;
        private bool HasOverhang = false;
        const int OVERHANG_HEIGHT = Constants.TILE_SIZE / 4;

        private TileAtlas TileAtlas;
        private TileAtlas ChipAtlas;

        public TileBatchRow(TileAtlas tileAtlas, TileAtlas chipAtlas, int widthInTiles, int yOffset)
        {
            TileAtlas = tileAtlas;
            ChipAtlas = chipAtlas;

            TileBatch = Love.Graphics.NewSpriteBatch(tileAtlas.Image, 2048, Love.SpriteBatchUsage.Dynamic);
            ChipBatch = Love.Graphics.NewSpriteBatch(chipAtlas.Image, 2048, Love.SpriteBatchUsage.Dynamic);
            TileOverhangBatch = Love.Graphics.NewSpriteBatch(tileAtlas.Image, 2048, Love.SpriteBatchUsage.Dynamic);
            
            TileWidth = Constants.TILE_SIZE;
            YOffset = yOffset;
            ScreenWidth = widthInTiles * TileWidth;
        }

        internal void OnThemeSwitched(TileAtlas tileAtlas, TileAtlas chipAtlas)
        {
            TileAtlas = tileAtlas;
            ChipAtlas = chipAtlas;

            TileBatch = Love.Graphics.NewSpriteBatch(tileAtlas.Image, 2048, Love.SpriteBatchUsage.Dynamic);
            ChipBatch = Love.Graphics.NewSpriteBatch(chipAtlas.Image, 2048, Love.SpriteBatchUsage.Dynamic);
            TileOverhangBatch = Love.Graphics.NewSpriteBatch(tileAtlas.Image, 2048, Love.SpriteBatchUsage.Dynamic);
        }

        internal void UpdateBatches(string[] tiles, int startIndex, int widthInTiles)
        {
            ScreenWidth = widthInTiles * TileWidth;
            TileBatch.Clear();
            TileOverhangBatch.Clear();
            HasOverhang = false;
            
            for (int x = 0; x < widthInTiles; x++)
            {
                var index = startIndex + x;
                var tileId = tiles[index];
                var tile = TileAtlas.GetTile(tileId);
                if (tile == null)
                {
                    Logger.Error($"Missing tile {tileId}");
                }
                else
                {
                    TileBatch.Add(tile.Quad, TileWidth * x, YOffset);

                    if (tile.HasOverhang)
                    {
                        HasOverhang = true;
                        TileOverhangBatch.Add(tile.Quad, TileWidth * x, YOffset);
                    }
                }
            }

            TileBatch.Flush();
            TileOverhangBatch.Flush();

            ChipBatch.Clear();

            ChipBatch.Flush();
        }

        public void Draw(int screenX, int screenY)
        {
            Love.Graphics.Draw(TileBatch, screenX, screenY);
            Love.Graphics.Draw(ChipBatch, screenX, screenY);

            if (HasOverhang)
            {
                GraphicsEx.SetScissor(screenX, screenY + YOffset - OVERHANG_HEIGHT, ScreenWidth, OVERHANG_HEIGHT);
                Love.Graphics.Draw(TileOverhangBatch, screenX, screenY - OVERHANG_HEIGHT);
                GraphicsEx.SetScissor();
            }
        }
    }
}

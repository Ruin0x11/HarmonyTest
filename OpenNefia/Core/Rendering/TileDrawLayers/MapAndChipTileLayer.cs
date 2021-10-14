using OpenNefia.Core.Data.Types;
using OpenNefia.Core.UI;
using OpenNefia.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Rendering.TileDrawLayers
{
    // Needs to be interleaved per-row to support wall occlusion.
    // This would be a combination of tile_layer, tile_overhang_layer and chip_layer.
    public class MapAndChipTileLayer : BaseTileLayer
    {
        private InstancedMap Map;
        private TileAtlas TileAtlas;
        private TileBatch TileBatch;
        private ICoords Coords;
        private WallTileShadows WallShadows;

        public MapAndChipTileLayer(InstancedMap map)
        {
            Map = map;
            TileAtlas = Atlases.Tile;
            Coords = GameWrapper.Instance.State.Coords;
            TileBatch = new TileBatch(map.Width, map.Height, TileAtlas, Coords);
            WallShadows = new WallTileShadows(map, Coords);
        }

        public void OnThemeSwitched(ICoords coords)
        {
            TileAtlas = Atlases.Tile;
            Coords = coords;
            this.TileBatch.OnThemeSwitched(TileAtlas, coords);
            this.WallShadows.OnThemeSwitched(coords);
        }

        public override void SetSize(int width = 0, int height = 0)
        {
            base.SetSize(width, height);
            this.TileBatch.SetSize(width, height);
            this.WallShadows.SetSize(width, height);
        }

        public override void SetPosition(int x = 0, int y = 0)
        {
            base.SetPosition(x, y);
            this.TileBatch.SetPosition(x, y);
            this.WallShadows.SetPosition(x, y);
        }

        private void SetMapTile(int x, int y, TileDef tile)
        {
            // If the tile is a wall, convert the displayed tile to that of
            // the bottom wall if appropriate.
            var tileIndex = tile.Tile.TileIndex;
            if (tile.Wall != null)
            {
                var oneTileDown = Map.GetTile(x, y + 1);
                if (oneTileDown != null && oneTileDown.Wall == null && Map.IsMemorized(x, y + 1))
                {
                    tileIndex = tile.Wall.TileIndex;
                }

                var oneTileUp = Map.GetTile(x, y - 1);
                if (oneTileUp != null && oneTileUp.Wall != null && Map.IsMemorized(x, y - 1))
                {
                    this.TileBatch.SetTile(x, y - 1, oneTileUp.Tile.TileIndex);
                }
            }
            else if (y > 0)
            {
                var oneTileUp = Map.GetTile(x, y - 1);
                if (oneTileUp != null && oneTileUp.Wall != null && Map.IsMemorized(x, y - 1))
                {
                    this.TileBatch.SetTile(x, y - 1, oneTileUp.Wall.TileIndex);
                }
            }

            this.WallShadows.SetTile(x, y, tile);
            this.TileBatch.SetTile(x, y, tileIndex);
        }

        public override void RedrawAll()
        {
            this.WallShadows.Clear();
            this.TileBatch.Clear();

            foreach (var (x, y, tileDef) in Map.TileMemory)
            {
                SetMapTile(x, y, tileDef);
            }

            this.TileBatch.UpdateBatches();
        }

        public override void RedrawDirtyTiles(HashSet<int> dirtyTilesThisTurn)
        {
            foreach (var index in dirtyTilesThisTurn)
            {
                var x = index % Map.Width;
                var y = index / Map.Height;
                SetMapTile(x, y, Map.GetTileMemory(x, y)!);
            }

            this.TileBatch.UpdateBatches();
        }

        public override void Update(float dt)
        {
            this.TileBatch.Update(dt);
            this.WallShadows.Update(dt);
        }

        public override void Draw()
        {
            this.TileBatch.Draw();
            this.WallShadows.Draw();
        }
    }
}

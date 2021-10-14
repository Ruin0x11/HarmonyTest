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
        internal class MapAndChipRow
        {
        }

        private InstancedMap Map;
        private TileAtlas TileAtlas;
        private TileBatch TileBatch;
        private ICoords Coords;

        public MapAndChipTileLayer(InstancedMap map)
        {
            Map = map;
            TileAtlas = Atlases.Tile;
            Coords = GameWrapper.Instance.State.Coords;
            TileBatch = new TileBatch(map.Width, map.Height, TileAtlas, Coords);
        }

        public void OnThemeSwitched(ICoords coords)
        {
            TileAtlas = Atlases.Tile;
            Coords = coords;
            this.TileBatch.OnThemeSwitched(TileAtlas, coords);
        }

        public override void RedrawAll()
        {
            foreach (var (x, y, tileDef) in Map.TileMemory)
            {
                this.TileBatch.SetTile(x, y, tileDef.Tile.TileIndex);
            }
            this.TileBatch.UpdateBatches();
        }

        public override void RedrawDirtyTiles(HashSet<int> dirtyTilesThisTurn)
        {
            foreach (var index in dirtyTilesThisTurn)
            {
                var x = index % Map.Width;
                var y = index / Map.Height;
                this.TileBatch.SetTile(x, y, Map.GetTileMemory(x, y).Id);
            }
            this.TileBatch.UpdateBatches();
        }

        public override void Update(float dt)
        {
            this.TileBatch.Update(dt);
        }

        public override void Draw()
        {
            this.TileBatch.Draw(X, Y);
        }
    }
}

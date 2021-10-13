using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Rendering.TileDrawLayers
{
    // Needs to be interleaved per-row to support wall occlusion.
    // This would be a combination of tile_layer, tile_overhang_layer and chip_layer.
    public class MapAndChipTileLayer : ITileLayer
    {
        public void RefreshFromMap(InstancedMap map)
        {
        }

        public void Update(float dt)
        {
        }

        public void Draw(int sx, int sy)
        {
        }
    }
}

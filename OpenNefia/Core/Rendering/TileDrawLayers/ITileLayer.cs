using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Rendering.TileDrawLayers
{
    public interface ITileLayer
    {
        public void RefreshFromMap(InstancedMap map);
        public void Update(float dt);
        public void Draw(int sx, int sy);
    }
}

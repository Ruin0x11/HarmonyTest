using OpenNefia.Core.Rendering.TileDrawLayers;
using OpenNefia.Core.UI.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Rendering
{
    public class MapRenderer : BaseUiElement
    {
        List<ITileLayer> TileLayers = new List<ITileLayer>();
        public InstancedMap Map { get; }

        public MapRenderer(InstancedMap map)
        {
            Map = map;
        }

        public override void SetSize(int width = 0, int height = 0)
        {
            base.SetSize(width, height);
        }

        public override void SetPosition(int x = 0, int y = 0)
        {
            base.SetPosition(x, y);
        }

        public override void Update(float dt)
        {
            foreach (var layer in TileLayers)
            {
                layer.RefreshFromMap(this.Map);
                layer.Update(dt);
            }
        }

        public override void Draw()
        {
            foreach (var layer in TileLayers)
            {
                layer.Draw(48, 48);
            }
        }
    }
}

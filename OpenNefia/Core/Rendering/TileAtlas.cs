using Love;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Rendering
{
    public class AnimFrame
    {
        public string TileId = string.Empty;
        public float Duration = 0f;
    }

    public class AtlasTile
    {
        public Love.Quad Quad;
        public int YOffset;

        public AtlasTile(Quad quad, int yOffset = 0)
        {
            Quad = quad;
            YOffset = yOffset;
        }
    }

    public class TileAtlas
    {
        private Dictionary<string, AtlasTile> Tiles = new Dictionary<string, AtlasTile>();
        private Dictionary<string, List<AnimFrame>> Anims = new Dictionary<string, List<AnimFrame>>();
        public Image Image { get; }

        public TileAtlas(Image image, Dictionary<string, AtlasTile> atlasTiles)
        {
            this.Image = image;
            this.Tiles = atlasTiles;
        }

        public AtlasTile? GetTile(TileSpec def)
        {
            if (Tiles.TryGetValue(def.Id, out var tile))
                return tile;
            return null;
        }

        public bool GetTileSize(TileSpec def, out int width, out int height)
        {
            var tile = GetTile(def);
            if (tile == null)
            {
                width = 0;
                height = 0;
                return false;
            }

            var rect = tile.Quad.GetViewport();

            width = (int)rect.Width;
            height = (int)rect.Height;

            return true;
        }
    }
}

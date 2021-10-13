using Love;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Util;
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
        private Dictionary<StructMultiKey<string, string>, AtlasTile> Tiles = new Dictionary<StructMultiKey<string, string>, AtlasTile>();
        private Dictionary<StructMultiKey<string, string>, List<AnimFrame>> Anims = new Dictionary<StructMultiKey<string, string>, List<AnimFrame>>();
        public Image Image { get; }

        public TileAtlas(Image image, Dictionary<StructMultiKey<string, string>, AtlasTile> atlasTiles)
        {
            this.Image = image;
            this.Tiles = atlasTiles;
        }

        public AtlasTile? GetTile(TileSpec spec)
        {
            if (Tiles.TryGetValue(spec.TileIndex, out var tile))
                return tile;
            return null;
        }

        public bool GetTileSize(TileSpec spec, out int width, out int height)
        {
            var tile = GetTile(spec);
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

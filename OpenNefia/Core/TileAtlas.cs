using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia
{
    public class TileAtlas
    {
        public Love.Texture Texture;
        List<Love.Quad> Quads;
        const int TILE_WIDTH = 48;
        const int TILE_HEIGHT = 48;

        public TileAtlas()
        {
            Texture = Love.Graphics.NewImage("Assets/map1.png");
            Quads = new List<Love.Quad>();

            for (int x = 0; x < 33; x++)
            {
                for (int y = 0; y < 33; y++)
                {
                    var quad = Love.Graphics.NewQuad(x * TILE_WIDTH, y * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT, Texture.GetWidth(), Texture.GetHeight());
                    Quads.Add(quad);
                }
            }
        }

        public Love.Quad GetTile(int tile)
        {
            return Quads[tile];
        }
    }
}

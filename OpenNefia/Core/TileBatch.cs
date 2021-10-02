using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia
{
    public class TileBatch
    {
        TileAtlas Atlas;
        Love.SpriteBatch Batch;

        public TileBatch(TileAtlas atlas)
        {
            Atlas = atlas;
            Batch = Love.Graphics.NewSpriteBatch(atlas.Texture, 1024, Love.SpriteBatchUsage.Static);
        }

        public void Clear()
        {
            Batch.Clear();
        }

        public void Add(int tile, int x, int y)
        {
            Batch.Add(Atlas.GetTile(tile), x, y);
        }

        public void Draw(int x, int y)
        {
            Love.Graphics.Draw(Batch, x, y);
        }
    }
}

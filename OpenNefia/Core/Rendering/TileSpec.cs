using OpenNefia.Core.Data.Serial;
using OpenNefia.Mod;

namespace OpenNefia.Core.Rendering
{
    public class TileSpec : IDefSerializable
    {
        public string TileId;
        public IResourcePath? ImagePath;
        public int CountX = 1;
        public ImageRegion? ImageRegion;

        public TileSpec(string id, IResourcePath imagePath, int countX = 1)
        {
            TileId = id;
            ImagePath = imagePath;
            CountX = countX;
            ImageRegion = null;
        }

        public TileSpec(string id, ImageRegion imageRegion)
        {
            TileId = id;
            ImagePath = null;
            ImageRegion = imageRegion;
        }
    }
}

using OpenNefia.Core.Data;
using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Util;
using OpenNefia.Mod;
using System.Collections.Generic;

namespace OpenNefia.Core.Rendering
{
    public class TileSpec : IDefSerializable
    {
        [DefRequired(DefaultValue="Default")]
        public string TileId = string.Empty;

        public IResourcePath? ImagePath;
        public int CountX = 1;

        public ImageRegion? ImageRegion;

        public StructMultiKey<string, string> TileIndex { get; internal set; } = new StructMultiKey<string, string>("", "");

        public TileSpec()
        {

        }

        public void ValidateDefField(List<string> errors)
        {
            if (ImagePath == null && ImageRegion == null)
            {
                errors.Add($"One of ImagePath or ImageRegion must be declared.");
            }
            if (TileIndex.Value1 == string.Empty || TileIndex.Value2 == string.Empty)
            {
                errors.Add($"Tile index must have mod and tile ID nonempty");
            }
        }
    }
}

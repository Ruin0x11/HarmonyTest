using Love;
using OpenNefia.Core.Data;
using OpenNefia.Core.Data.Serial;
using OpenNefia.Mod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenNefia.Core.Rendering
{
    public class ImageRegion : IDefSerializable
    {
        [DefRequired]
        [DefSerialUseAttribute]
        public IResourcePath SourceImagePath = null!;

        [DefRequired]
        [DefSerialUseAttribute]
        public int X = 0;

        [DefRequired]
        [DefSerialUseAttribute]
        public int Y = 0;

        [DefRequired]
        [DefSerialUseAttribute]
        public int Width = 0;

        [DefRequired]
        [DefSerialUseAttribute]
        public int Height = 0;

        public Love.Color? KeyColor { get; set; } = null;

        public ImageRegion(IResourcePath atlasImage, int x, int y, int width = 0, int height = 0, Color? keyColor = null)
        {
            SourceImagePath = atlasImage;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            KeyColor = keyColor;
        }

        public ImageRegion()
        {
        }
    }
}

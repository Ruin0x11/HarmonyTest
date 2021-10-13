using Love;
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
        [DefSerialUseAttribute]
        public IResourcePath SourceImagePath { get; set; }

        [DefSerialUseAttribute]
        public int X { get; set; } = 0;

        [DefSerialUseAttribute]
        public int Y { get; set; } = 0;

        [DefSerialUseAttribute]
        public int Width { get; set; } = 0;

        [DefSerialUseAttribute]
        public int Height { get; set; } = 0;

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
            SourceImagePath = null!;
        }
    }
}

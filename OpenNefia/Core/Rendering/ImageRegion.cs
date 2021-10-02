using Love;
using OpenNefia.Mod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Rendering
{
    public class ImageRegion
    {
        public IResourcePath ParentImagePath { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Love.Color? KeyColor { get; set; }

        public ImageRegion(IResourcePath parentImage, int x, int y, int width = 0, int height = 0, Color? keyColor = null)
        {
            ParentImagePath = parentImage;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            KeyColor = keyColor;
        }
    }
}

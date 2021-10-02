using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Rendering
{
    public static class ImageLoader
    {
        private static byte[] GdiImageToBytes(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        private static Love.Image LoadBitmap(string filepath)
        {
            var original = new Bitmap(filepath);
            var converted = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);

            using (Graphics gr = Graphics.FromImage(converted))
            {
                gr.DrawImage(original, new Rectangle(0, 0, converted.Width, converted.Height));
            }

            var bytes = GdiImageToBytes(converted);
            var imageData = Love.Image.NewImageData(converted.Width, converted.Height, Love.ImageDataPixelFormat.RGBA32F, bytes);

            return Love.Graphics.NewImage(imageData);
        }

        /// <summary>
        /// Extension to <see cref="Love.Graphics.NewImage"/> that also supports loading .BMP files.
        /// </summary>
        /// <param name="filepath">Path to image file.</param>
        /// <returns></returns>
        public static Love.Image Load(string filepath)
        {
            if (Path.GetExtension(filepath) == "bmp")
            {
                return LoadBitmap(filepath);
            }

            return Love.Graphics.NewImage(filepath);
        }
    }
}

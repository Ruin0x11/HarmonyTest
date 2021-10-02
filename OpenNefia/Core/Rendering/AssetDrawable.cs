using Love;
using OpenNefia.Core.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Rendering
{
    public class AssetDrawable : IDisposable
    {
        public Asset Asset;
        public Love.Image Image;

        private Dictionary<uint, Love.Quad> DividedQuads;
        private Dictionary<string, Love.Quad> KeyedQuads;

        public uint CountX { get; }
        public uint CountY { get; }

        private static Image LoadImageSource(ImageRegion imageRegion)
        {
            var path = imageRegion.ParentImagePath.Resolve();
            var parentImage = Love.Graphics.NewImage(path);

            var quad = Love.Graphics.NewQuad(imageRegion.X, imageRegion.Y, imageRegion.Width, imageRegion.Height, parentImage.GetWidth(), parentImage.GetHeight());

            var canvas = Love.Graphics.NewCanvas(imageRegion.Width, imageRegion.Height);
            var oldCanvas = Love.Graphics.GetCanvas();

            // Reset global drawing state to be clean so the asset gets copied correctly
            Love.Graphics.GetBlendMode(out Love.BlendMode blendMode, out Love.BlendAlphaMode blendAlphaMode);
            var scissor = Love.Graphics.GetScissor();
            var color = Love.Graphics.GetColor();
            Love.Graphics.SetBlendMode(Love.BlendMode.Alpha);
            Love.Graphics.SetScissor();
            Love.Graphics.SetColor(1f, 1f, 1f, 1f);
            Love.Graphics.SetCanvas(canvas);

            Love.Graphics.Draw(quad, parentImage, 0, 0);

            Love.Graphics.SetBlendMode(blendMode, blendAlphaMode);
            Love.Graphics.SetScissor(scissor);
            Love.Graphics.SetColor(color);
            Love.Graphics.SetCanvas(oldCanvas);

            var image = Love.Graphics.NewImage(canvas.NewImageData());

            quad.Dispose();
            canvas.Dispose();

            return image;
        }

        private static Love.Image LoadImage(Asset asset)
        {
            Love.Image image;

            if (asset.ImagePath != null)
            {
                var path = asset.ImagePath.Resolve();
                image = Love.Graphics.NewImage(path);
            }
            else if (asset.ImageRegion != null)
            {
                image = LoadImageSource(asset.ImageRegion);
            }
            else
            {
                throw new ArgumentException($"Asset has neither ImagePath nor ImageRegion: {asset.Id}");
            }

            if (asset.ImageFilter != null)
            {
                image.SetFilter(asset.ImageFilter.Min, asset.ImageFilter.Mag, asset.ImageFilter.Anisotropy);
            }

            return image;
        }

        public AssetDrawable(Asset asset)
        {
            this.Asset = asset;
            this.Image = LoadImage(this.Asset);
            this.DividedQuads = new Dictionary<uint, Quad>();
            this.KeyedQuads = new Dictionary<string, Quad>();

            var imageWidth = this.Image.GetWidth();
            var imageHeight = this.Image.GetHeight();

            var countX = this.Asset.CountX;
            var countY = this.Asset.CountY;

            if (countX > 1 || countY > 1)
            {
                var width = imageWidth / countX;
                var height = imageHeight / countY;

                uint quadId = 0;
                for (int j = 0; j < countY; j++)
                {
                    for (int i = 0; i < countX; i++)
                    {
                        this.DividedQuads[quadId] = Love.Graphics.NewQuad(width * i, height * j, width, height, imageWidth, imageHeight);
                        quadId++;
                    }
                }
            }
            else
            {
                this.DividedQuads[1] = Love.Graphics.NewQuad(0, 0, imageWidth, imageHeight, imageWidth, imageHeight);
            }

            this.CountX = countX;
            this.CountY = countY;
            
            foreach (var pair in this.Asset.Regions)
            {
                var key = pair.Key;
                var region = pair.Value;
                this.KeyedQuads[key] = Love.Graphics.NewQuad(region.X, region.Y, region.Width, region.Height, imageWidth, imageHeight);
            }
        }

        public AssetInstance MakeInstance(int width, int height)
        {
            return new AssetInstance(this, width, height);
        }

        public int GetWidth() => this.Image.GetWidth();
        public int GetHeight() => this.Image.GetWidth();
        
        public void Draw(float x, float y, float width, float height, bool centered, float rotation)
        {
            Drawing.DrawImage(this.Image, x, y, width, height, centered, rotation);
        }

        public void DrawRegion(uint regionId, float x, float y, float width, float height, bool centered, float rotation)
        {
            var quad = this.DividedQuads[regionId];
            if (quad == null)
            {
                throw new ArgumentException($"Invalid region ID {regionId}");
            }

            Drawing.DrawImageRegion(this.Image, quad, x, y, width, height, centered, rotation);
        }

        public void Dispose()
        {
            foreach (var quad in this.KeyedQuads.Values)
            {
                quad.Dispose();
            }
            foreach (var quad in this.DividedQuads.Values)
            {
                quad.Dispose();
            }

            this.KeyedQuads.Clear();
            this.DividedQuads.Clear();

            this.Image.Dispose();
        }
    }
}

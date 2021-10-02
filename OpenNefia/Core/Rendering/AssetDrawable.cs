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
        /// <summary>
        /// Data to submit to <see cref="MakeBatch(List{AssetBatchPart}, int, SpriteBatchUsage)"/>
        /// </summary>
        public class AssetBatchPart
        {
            public string RegionId { get; set; } = string.Empty;
            public int X { get; set; } = 0;
            public int Y { get; set; } = 0;

            public AssetBatchPart(string id, int x, int y)
            {
                RegionId = id;
                X = x;
                Y = y;
            }
        }

        public Asset Asset;
        public Love.Image Image;

        private Dictionary<string, Love.Quad> Quads;
        private Dictionary<string, Asset.Region> Regions;

        public uint CountX { get; }
        public uint CountY { get; }
        public int? BatchWidth { get; private set; }
        public int? BatchHeight { get; private set; }

        public int Width { get => this.Image.GetWidth(); }
        public int Height { get => this.Image.GetHeight(); }

        private static Image LoadImageSource(ImageRegion imageRegion)
        {
            var path = imageRegion.ParentImagePath.Resolve();
            var parentImage = ImageLoader.NewImage(path);

            
            var quad = Love.Graphics.NewQuad(imageRegion.X, imageRegion.Y, imageRegion.Width, imageRegion.Height, parentImage.GetWidth(), parentImage.GetHeight());

            var canvas = Love.Graphics.NewCanvas(imageRegion.Width, imageRegion.Height);
            var oldCanvas = Love.Graphics.GetCanvas();

            //// Reset global drawing state to be clean so the asset gets copied correctly
            Love.Graphics.GetBlendMode(out Love.BlendMode blendMode, out Love.BlendAlphaMode blendAlphaMode);
            var scissor = Love.Graphics.GetScissor();
            var color = Love.Graphics.GetColor();
            Love.Graphics.SetBlendMode(Love.BlendMode.Alpha);
            Drawing.SetScissor();
            Love.Graphics.SetColor(1f, 1f, 1f, 1f);
            Love.Graphics.SetCanvas(canvas);

            Love.Graphics.Draw(quad, parentImage, 0, 0);

            Love.Graphics.SetBlendMode(blendMode, blendAlphaMode);
            Drawing.SetScissor(scissor); // BUG: Love.Graphics.SetScissor is bugged.
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
                image = ImageLoader.NewImage(path);
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
            this.Quads = new Dictionary<string, Quad>();
            this.CountX = this.Asset.CountX;
            this.CountY = this.Asset.CountY;
            this.Regions = new Dictionary<string, Asset.Region>(this.Asset.Regions);

            this.SetupQuads();
        }

        public AssetDrawable(Asset asset, int batchWidth, int batchHeight) : this(asset)
        {
            this.Asset = asset;
            this.Image = LoadImage(this.Asset);
            this.Quads = new Dictionary<string, Quad>();
            this.CountX = this.Asset.CountX;
            this.CountY = this.Asset.CountY;
            this.BatchWidth = batchWidth;
            this.BatchHeight = batchHeight;
            this.Regions = this.Asset.GetRegions(this.BatchWidth.Value, this.BatchHeight.Value);

            this.SetupQuads();
        }

        private void SetupQuads()
        {
            var countX = this.CountX;
            var countY = this.CountY;

            var imageWidth = this.Image.GetWidth();
            var imageHeight = this.Image.GetHeight();

            if (countX > 1 || countY > 1)
            {
                var width = imageWidth / countX;
                var height = imageHeight / countY;

                uint quadId = 0;
                for (int j = 0; j < countY; j++)
                {
                    for (int i = 0; i < countX; i++)
                    {
                        this.Quads[quadId.ToString()] = Love.Graphics.NewQuad(width * i, height * j, width, height, imageWidth, imageHeight);
                        quadId++;
                    }
                }
            }
            else
            {
                this.Quads["1"] = Love.Graphics.NewQuad(0, 0, imageWidth, imageHeight, imageWidth, imageHeight);
            }

            foreach (var pair in this.Regions)
            {
                var key = pair.Key;
                var region = pair.Value;
                this.Quads[key] = Love.Graphics.NewQuad(region.X, region.Y, region.Width, region.Height, imageWidth, imageHeight);
            }
        }

        public Love.SpriteBatch MakeBatch(List<AssetBatchPart> parts, int maxSprites = 2048)
        {
            var batch = Love.Graphics.NewSpriteBatch(this.Image, maxSprites, Love.SpriteBatchUsage.Static);
            batch.Clear();

            foreach (var part in parts)
            {
                batch.Add(this.Quads[part.RegionId], part.X, part.Y);
            }

            batch.Flush();

            return batch;
        }
        
        public void Draw(float x = 0, float y = 0, float width = 0, float height = 0, bool centered = false, float rotation = 0)
        {
            Drawing.DrawImage(this.Image, x, y, width, height, centered, rotation);
        }

        public void DrawRegion(string regionId, float x = 0, float y = 0, float width = 0, float height = 0, bool centered = false, float rotation = 0)
        {
            var quad = this.Quads[regionId];
            if (quad == null)
            {
                throw new ArgumentException($"Invalid region ID {regionId}");
            }

            Drawing.DrawImageRegion(this.Image, quad, x, y, width, height, centered, rotation);
        }

        public void Dispose()
        {
            foreach (var quad in this.Quads.Values)
            {
                quad.Dispose();
            }
            this.Quads.Clear();
            this.Image.Dispose();
        }
    }
}

﻿using OpenNefia.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Rendering
{
    public class TileAtlasFactory : IDisposable
    {
        internal RectanglePacker Binpack { get; }
        public int TileWidth { get; }
        public int TileHeight { get; }

        private Dictionary<string, AtlasTile> AtlasTiles;
        private Love.Canvas WorkCanvas;
        private ImageFilter Filter;

        public delegate void LoadTileDelegate(Love.Image image, Love.Quad quad, int rectX, int rectY);
        private LoadTileDelegate? OnLoadTile;

        public TileAtlasFactory(int tileWidth = UI.Constants.TILE_SIZE, int tileHeight = UI.Constants.TILE_SIZE, int tileCountX = 48, int tileCountY = 48)
        {
            TileWidth = tileWidth;
            TileHeight = tileHeight;

            var imageWidth = tileCountX * tileWidth;
            var imageHeight = tileCountY * tileHeight;
            Binpack = new RectanglePacker(imageWidth, imageHeight);
            WorkCanvas = Love.Graphics.NewCanvas(imageWidth, imageHeight);
            Filter = new ImageFilter(Love.FilterMode.Linear, Love.FilterMode.Linear, 1);
            AtlasTiles = new Dictionary<string, AtlasTile>();
            OnLoadTile = null;
        }

        public TileAtlasFactory WithLoadTileCallback(LoadTileDelegate? callback)
        {
            this.OnLoadTile = callback;
            return this;
        }

        private Tuple<Love.Image, Love.Quad> LoadImageAndQuad(TileSpec tile)
        {
            if (tile.ImageRegion != null)
            {
                var image = ImageLoader.NewImage(tile.ImageRegion.SourceImagePath.Resolve());
                var quad = Love.Graphics.NewQuad(tile.ImageRegion.X, tile.ImageRegion.Y, tile.ImageRegion.Width, tile.ImageRegion.Height, image.GetWidth(), image.GetHeight());
                return Tuple.Create(image, quad);
            }
            else if (tile.ImagePath != null)
            {
                var image = ImageLoader.NewImage(tile.ImagePath.Resolve());
                var quad = Love.Graphics.NewQuad(0, 0, image.GetWidth(), image.GetHeight(), image.GetWidth(), image.GetHeight());
                return Tuple.Create(image, quad);
            }
            else
            {
                throw new Exception("Invalid tile spec");
            }
        }

        public void LoadTile(TileSpec tile)
        {
            var (image, quad) = LoadImageAndQuad(tile);

            var quadSize = quad.GetViewport();

            if (!this.Binpack.Pack((int)quadSize.Width, (int)quadSize.Height, out int rectX, out int rectY))
            {
                throw new Exception($"Ran out of space while packing tile atlas ({tile.TileIndex})");
            }

            if (this.OnLoadTile != null)
            {
                this.OnLoadTile(image, quad, rectX, rectY);
            }
            else
            {
                Love.Graphics.Draw(quad, image, rectX, rectY);
            }

            var innerQuad = Love.Graphics.NewQuad(rectX, rectY, quadSize.Width, quadSize.Height, this.WorkCanvas.GetWidth(), this.WorkCanvas.GetHeight());

            var isTall = quadSize.Height == quadSize.Width * 2;
            var yOffset = 0;
            if (isTall)
                yOffset = -this.TileHeight;

            this.AtlasTiles.Add(tile.TileIndex, new AtlasTile(innerQuad, yOffset));

            quad.Dispose();
        }

        public TileAtlasFactory LoadTiles(IEnumerable<TileSpec> tiles)
        {
            var canvas = Love.Graphics.GetCanvas();

            Love.Graphics.SetCanvas(this.WorkCanvas);
            Love.Graphics.SetBlendMode(Love.BlendMode.Alpha);
            Love.Graphics.SetColor(Love.Color.White);
            GraphicsEx.SetDefaultFilter(this.Filter);

            foreach (var tile in tiles)
            {
                LoadTile(tile);
            }

            Love.Graphics.SetCanvas(canvas);
            Love.Graphics.SetDefaultFilter(Love.FilterMode.Linear, Love.FilterMode.Linear, 1);

            return this;
        }

        public TileAtlas Build()
        {
            var image = Love.Graphics.NewImage(this.WorkCanvas.NewImageData());

            return new TileAtlas(image, this.AtlasTiles);
        }

        public void Dispose()
        {
            this.WorkCanvas.Dispose();
        }
    }
}

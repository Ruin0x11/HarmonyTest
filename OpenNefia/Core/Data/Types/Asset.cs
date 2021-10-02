using OpenNefia.Core.Data.Types.Assets;
using OpenNefia.Core.Rendering;
using OpenNefia.Mod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data.Types
{
    public class Asset : IDataType
    {
        /// <summary>
        /// Region in an image to use when making an asset instance.
        /// </summary>
        public struct Region
        {
            public int X;
            public int Y;
            public int Width;
            public int Height;

            public Region(int x, int y, int width, int height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }
        }

        public string Id { get; private set; }
        public Asset(string id) { this.Id = id; }

        /// <summary>
        /// Path of an image to use.
        /// </summary>
        public IResourcePath? ImagePath { get; private set; }

        /// <summary>
        /// Information for a region of an image to cut out and use for this <see cref="Asset"/>.
        /// </summary>
        public ImageRegion? ImageRegion { get; private set; }

        /// <summary>
        /// Filter to apply to the image.
        /// </summary>
        public ImageFilter? ImageFilter { get; private set; }


        /// <summary>
        /// Number of tiled images in the X direction. Each will get its own quad when the asset is instantiated.
        /// </summary>
        public uint CountX { get; private set; } = 1;

        /// <summary>
        /// Number of tiled images in the Y direction. Each will get its own quad when the asset is instantiated.
        /// </summary>
        public uint CountY { get; private set; } = 1;

        /// <summary>
        /// List of regions available when making an asset batch from this <see cref="Asset"/>.
        /// </summary>
        public Dictionary<string, Region> Regions { get; private set; } = new Dictionary<string, Region>();

        /// <summary>
        /// Given a width and height, generate a set of regions to use for those dimensions when making an asset instance from this <see cref="Asset"/>.
        /// 
        /// By default, returns the regions defined in the <see cref="Regions"/> field.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public virtual Dictionary<string, Region> GetRegions(int width, int height) => new Dictionary<string, Region>(Regions);

        public static class Entries
        {
            private static IResourcePath InterfaceBmp = new ModLocalPath(typeof(CoreMod), "Assets/interface.bmp");

            public static Asset Window = new WindowAsset($"Core.{nameof(Window)}")
            {
                ImageRegion = new ImageRegion(InterfaceBmp, 0, 48, 264, 192)
            };

            public static Asset SelectKey = new Asset($"Core.{nameof(SelectKey)}")
            {
                ImageRegion = new ImageRegion(InterfaceBmp, 0, 30, 24, 18)
            };

            public static Asset ListBullet = new Asset($"Core.{nameof(ListBullet)}")
            {
                ImageRegion = new ImageRegion(InterfaceBmp, 48, 360, 16, 16)
            };

            public static Asset TipIcons = new Asset($"Core.{nameof(TipIcons)}")
            {
                ImageRegion = new ImageRegion(InterfaceBmp, 96, 360, 24 * 8, 16),
                CountX = 8
            };
        }
    }
}

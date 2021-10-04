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
    public class Asset : Def
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

        public Asset(string id) : base(id) { }

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

            public static Asset TopicWindow0 = new TopicWindowAsset($"Core.{nameof(TopicWindow0)}")
            {
                ImageRegion = new ImageRegion(InterfaceBmp, 48 * 0, 240, 48, 48),
            };
            public static Asset TopicWindow1 = new TopicWindowAsset($"Core.{nameof(TopicWindow1)}")
            {
                ImageRegion = new ImageRegion(InterfaceBmp, 48 * 1, 240, 48, 48),
            };
            public static Asset TopicWindow2 = new TopicWindowAsset($"Core.{nameof(TopicWindow2)}")
            {
                ImageRegion = new ImageRegion(InterfaceBmp, 48 * 2, 240, 48, 48),
            };
            public static Asset TopicWindow3 = new TopicWindowAsset($"Core.{nameof(TopicWindow3)}")
            {
                ImageRegion = new ImageRegion(InterfaceBmp, 48 * 3, 240, 48, 48),
            };
            public static Asset TopicWindow4 = new TopicWindowAsset($"Core.{nameof(TopicWindow4)}")
            {
                ImageRegion = new ImageRegion(InterfaceBmp, 48 * 4, 240, 48, 48),
            };
            public static Asset TopicWindow5 = new TopicWindowAsset($"Core.{nameof(TopicWindow5)}")
            {
                ImageRegion = new ImageRegion(InterfaceBmp, 48 * 5, 240, 48, 48),
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

            public static Asset LabelInput = new Asset($"Core.{nameof(LabelInput)}")
            {
                ImageRegion = new ImageRegion(InterfaceBmp, 128, 288, 128, 32),
            };
            public static Asset ArrowLeft = new Asset($"Core.{nameof(ArrowLeft)}")
            {
                ImageRegion = new ImageRegion(InterfaceBmp, 312, 336, 24, 24),
            };
            public static Asset ArrowRight = new Asset($"Core.{nameof(ArrowRight)}")
            {
                ImageRegion = new ImageRegion(InterfaceBmp, 336, 336, 24, 24),
            };

            public static Asset ImeStatusEnglish = new Asset($"Core.{nameof(ImeStatusEnglish)}")
            {
                ImageRegion = new ImageRegion(InterfaceBmp, 24, 336, 24, 24),
            };
            public static Asset ImeStatusJapanese = new Asset($"Core.{nameof(ImeStatusJapanese)}")
            {
                ImageRegion = new ImageRegion(InterfaceBmp, 48, 336, 24, 24),
            };
            public static Asset ImeStatusNone = new Asset($"Core.{nameof(ImeStatusNone)}")
            {
                ImageRegion = new ImageRegion(InterfaceBmp, 72, 336, 24, 24),
            };

            public static Asset InputCaret = new Asset($"Core.{nameof(InputCaret)}")
            {
                ImageRegion = new ImageRegion(InterfaceBmp, 0, 336, 12, 24),
            };
        }
    }
}

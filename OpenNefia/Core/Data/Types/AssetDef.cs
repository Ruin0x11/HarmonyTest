using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Data.Types.Assets;
using OpenNefia.Core.Rendering;
using OpenNefia.Mod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace OpenNefia.Core.Data.Types
{
    public class AssetSpec : IDefDeserializable
    {
        /// <summary>
        /// Path of an image to use.
        /// </summary>
        public IResourcePath? ImagePath;

        /// <summary>
        /// Information for a region of an image to cut out and use for this <see cref="AssetDef"/>.
        /// </summary>
        public ImageRegion? ImageRegion;

        /// <summary>
        /// Filter to apply to the image.
        /// </summary>
        public ImageFilter? ImageFilter;

        public AssetSpec()
        {
        }

        public void DeserializeDefField(IDefDeserializer deserializer, XElement elem, Type containingModType)
        {
            if (elem.Attribute("SourceImagePath") != null)
            {
                this.ImageRegion = new ImageRegion();
                this.ImageRegion.DeserializeDefField(deserializer, elem, containingModType);
            }
            else if (elem.Value != null)
            {
                this.ImagePath = new ModLocalPath(containingModType, elem.Value);
            }
        }

        public void ValidateDefField(List<string> errors)
        {
            if (ImagePath == null && ImageRegion == null)
            {
                errors.Add($"One of ImagePath or ImageRegion must be declared.");
            }
        }
    }

    public class AssetDef : Def
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

        public AssetDef(string id) : base(id) { }

        /// <summary>
        /// Image to use for this asset.
        /// </summary>
        public AssetSpec Image = new AssetSpec();

        /// <summary>
        /// Number of tiled images in the X direction. Each will get its own quad when the asset is instantiated.
        /// </summary>
        public uint CountX = 1;

        /// <summary>
        /// Number of tiled images in the Y direction. Each will get its own quad when the asset is instantiated.
        /// </summary>
        public uint CountY = 1;

        /// <summary>
        /// List of regions available when making an asset batch from this <see cref="AssetDef"/>.
        /// </summary>
        public Dictionary<string, Region> Regions = new Dictionary<string, Region>();

        /// <summary>
        /// Given a width and height, generate a set of regions to use for those dimensions when making an asset instance from this <see cref="AssetDef"/>.
        /// 
        /// By default, returns the regions defined in the <see cref="Regions"/> field.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public virtual Dictionary<string, Region> GetRegions(int width, int height) => new Dictionary<string, Region>(Regions);
    }
}

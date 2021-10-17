using OpenNefia.Core.Data;
using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Util;
using OpenNefia.Mod;
using System;
using System.Collections.Generic;
using System.Xml;

namespace OpenNefia.Core.Rendering
{
    public class TileSpec : IDefSerializable
    {
        [DefRequired(DefaultValue="Default")]
        public string TileId = string.Empty;

        public IResourcePath? ImagePath;
        public int CountX = 1;

        public ImageRegion? ImageRegion;

        public string TileIndex { get; internal set; } = string.Empty;
        public bool HasOverhang { get; internal set; } = false;

        public TileSpec()
        {

        }

        public void DeserializeDefField(IDefDeserializer deserializer, XmlNode node, Type containingModType)
        {
            if (node.Attributes?["SourceImagePath"] != null)
            {
                this.ImageRegion = new ImageRegion();
                this.ImageRegion.DeserializeDefField(deserializer, node, containingModType);
            }
            else if (node.InnerText != null)
            {
                this.ImagePath = new ModLocalPath(containingModType, node.InnerText);
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
}

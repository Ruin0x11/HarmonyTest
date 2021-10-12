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
        public IResourcePath ParentImagePath { get; set; }

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
            ParentImagePath = atlasImage;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            KeyColor = keyColor;
        }

        public ImageRegion()
        {
            ParentImagePath = null!;
        }

        public void DeserializeDefField(IDefDeserializer deserializer, XmlNode node, Type containingModType)
        {
            deserializer.PopulateFieldByName(nameof(X), node, this, containingModType);
            deserializer.PopulateFieldByName(nameof(Y), node, this, containingModType);
            deserializer.PopulateFieldByName(nameof(Width), node, this, containingModType);
            deserializer.PopulateFieldByName(nameof(Height), node, this, containingModType);

            if (node.ChildNodes.Count == 1)
            {
                deserializer.PopulateFieldByNode(nameof(ParentImagePath), node.ChildNodes[0]!, this, containingModType);
            }
            else
            {
                deserializer.PopulateFieldByNode(nameof(ParentImagePath), node[nameof(ParentImagePath)]!, this, containingModType);
                if (node[nameof(KeyColor)] != null)
                    deserializer.PopulateFieldByNode(nameof(KeyColor), node[nameof(KeyColor)]!, this, containingModType);
            }
        }
    }
}

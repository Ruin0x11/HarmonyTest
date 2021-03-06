using Love;
using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Data.Serial.CrossRefs;
using OpenNefia.Core.Rendering;
using OpenNefia.Mod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static OpenNefia.Core.Rendering.GraphicsEx;

namespace OpenNefia.Core.Data.Types
{
    public class FontDef : Def
    {
        public static class ColorKinds
        {
            public const string Background = nameof(Background);
        }

        public FontDef(string id) : this(id, 14, 12, FontFormatting.None, null, null, null) { }

        public FontDef(string id, int size = 14, int smallSize = 12, FontFormatting style = FontFormatting.None, IResourcePath? fontFilepath = null, ColorDef? fgColor = null, ColorDef? bgColor = null) : base(id)
        {
            this.Size = size;
            this.SmallSize = smallSize;
            this.Formatting = style; 
            this.FontFilepath = fontFilepath;
            this.ExtraColors = new Dictionary<string, ColorDef>();

            if (fgColor == null)
                fgColor = ColorDefOf.TextBlack;
            this.Color = fgColor;

            if (bgColor != null)
                this.ExtraColors.Add(ColorKinds.Background, bgColor);
        }

        [DefUseAttributes]
        [DefRequired]
        public int Size = 14;

        [DefUseAttributes]
        [DefRequired]
        public int SmallSize = 12;

        [DefUseAttributes]
        public FontFormatting Formatting = FontFormatting.None;

        [DefUseAttributes]
        public FontStyle Style = FontStyle.Normal;

        [DefUseAttributes]
        public IResourcePath? FontFilepath;
        
        [DefUseAttributes]
        [DefSerialName("FgColor")]
        public ColorDef Color = null!;

        [DefIgnored]
        public Dictionary<string, ColorDef> ExtraColors = new Dictionary<string, ColorDef>();

        private Love.Font? _LoveFont;
        public Love.Font LoveFont
        {
            get
            {
                if (_LoveFont == null)
                    _LoveFont = GraphicsEx.GetFont(this);
                return _LoveFont;
            }
        }
        public static implicit operator Love.Font(FontDef c) => c.LoveFont;

        public int GetWidth(string text) => this.LoveFont.GetWidth(text);
        public int GetHeight() => this.LoveFont.GetHeight();
        public Tuple<int, string[]> GetWrap(string text, int width) => this.LoveFont.GetWrap(text, width);

        public override void DeserializeDefField(IDefDeserializer deserializer, XElement node, Type containingModType)
        {
            base.DeserializeDefField(deserializer, node, containingModType);

            var bgColor = node.Attribute("BgColor");
            if (bgColor != null)
            {
                var crossRef = new DefCustomCrossRef<FontDef, ColorDef>(
                    new List<string>() { bgColor.Value }, 
                    this, 
                    (o, colorDefs) => o.ExtraColors.Add(ColorKinds.Background, colorDefs.First()));
                deserializer.AddCrossRef(crossRef);
            }
        }

        public override void OnResolveReferences()
        {
            if (this.Color == null)
            {
                switch (this.Style)
                {
                    case FontStyle.Normal:
                        this.Color = ColorDefOf.TextBlack;
                        break;
                    case FontStyle.Outlined:
                    case FontStyle.Shadowed:
                        this.Color = ColorDefOf.TextWhite;
                        break;
                }
            }

            if (!this.ExtraColors.ContainsKey(ColorKinds.Background))
            {
                switch (this.Style)
                {
                    case FontStyle.Normal:
                        this.ExtraColors.Add(ColorKinds.Background, ColorDefOf.TextWhite);
                        break;
                    case FontStyle.Outlined:
                    case FontStyle.Shadowed:
                        this.ExtraColors.Add(ColorKinds.Background, ColorDefOf.TextBlack);
                        break;
                }
            }
        }

        public override void OnMerge()
        {
            this._LoveFont = null;
        }
    }
}

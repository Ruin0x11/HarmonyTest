using Love;
using OpenNefia.Core.Rendering;
using OpenNefia.Mod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenNefia.Core.Rendering.GraphicsEx;

namespace OpenNefia.Core.Data.Types
{
    public class FontAsset : Def
    {
        public static class ColorKinds
        {
            public const string Background = nameof(Background);
        }

        public FontAsset(string id, int size, int? smallSize = null, FontStyle style = FontStyle.None, IResourcePath? fontFilepath = null, ColorAsset? fgColor = null, ColorAsset? bgColor = null) : base(id)
        {
            if (smallSize == null)
                smallSize = size;

            this.Size = size;
            this.SmallSize = smallSize.Value;
            this.Style = style; 
            this.FontFilepath = fontFilepath;
            this.ExtraColors = new Dictionary<string, ColorAsset>();

            if (fgColor == null)
                fgColor = ColorAsset.Entries.TextBlack;
            this.Color = fgColor;

            if (bgColor != null)
                this.ExtraColors.Add(ColorKinds.Background, bgColor);
        }

        public int Size { get; private set; }
        public int SmallSize {  get; private set; }
        public FontStyle Style { get; private set; }
        public IResourcePath? FontFilepath { get; private set; }
        public ColorAsset Color { get; private set; }
        public Dictionary<string, ColorAsset> ExtraColors { get; private set; }

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
        public static implicit operator Love.Font(FontAsset c) => c.LoveFont;

        internal int GetWidth(string text) => this.LoveFont.GetWidth(text);
        internal int GetHeight() => this.LoveFont.GetHeight();

        public static class Entries
        {
            public static FontAsset ListText = new FontAsset($"Core.{nameof(ListText)}", 14, 12);
            public static FontAsset ListKeyName = new FontAsset($"Core.{nameof(ListKeyName)}", 15, 13, fgColor: ColorAsset.Entries.TextWhite, bgColor: ColorAsset.Entries.TextBlack);

            public static FontAsset WindowTitle = new FontAsset($"Core.{nameof(WindowTitle)}", 15, 14, fgColor: ColorAsset.Entries.TextWhite, bgColor: ColorAsset.Entries.TextBlack);
            public static FontAsset WindowKeyHints = new FontAsset($"Core.{nameof(WindowKeyHints)}", 15, 14);

            public static FontAsset PromptText = new FontAsset($"Core.{nameof(WindowKeyHints)}", 16, 14, fgColor: ColorAsset.Entries.TextWhite, bgColor: ColorAsset.Entries.TextBlack);
        }
    }
}

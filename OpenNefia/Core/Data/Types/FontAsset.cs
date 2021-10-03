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
    public class FontAsset : IDataType
    {
        public string Id { get; private set; }
        public FontAsset(string id, int size, int? smallSize = null, FontStyle style = FontStyle.None, IResourcePath? fontFilepath = null)
        {
            if (smallSize == null)
                smallSize = size;

            this.Id = id;
            this.Size = size;
            this.SmallSize = smallSize.Value;
            this.Style = style; 
            this.FontFilepath = fontFilepath;
        }

        public int Size {  get; private set; }
        public int SmallSize {  get; private set; }
        public FontStyle Style { get; private set; }
        public IResourcePath? FontFilepath { get; private set; }

        private Love.Font? _LoveObject;
        public Love.Font LoveObject
        {
            get
            {
                if (_LoveObject == null)
                    _LoveObject = GraphicsEx.GetFont(this);
                return _LoveObject;
            }
        }
        public static implicit operator Love.Font(FontAsset c) => c.LoveObject;

        internal int GetWidth(string text) => this.LoveObject.GetWidth(text);
        internal int GetHeight() => this.LoveObject.GetHeight();

        public static class Entries
        {
            public static FontAsset ListText = new FontAsset($"Core.{nameof(ListText)}", 14, 12);
            public static FontAsset ListKeyName = new FontAsset($"Core.{nameof(ListKeyName)}", 15, 13);

            public static FontAsset WindowTitle = new FontAsset($"Core.{nameof(WindowTitle)}", 15, 14);
            public static FontAsset WindowKeyHints = new FontAsset($"Core.{nameof(WindowKeyHints)}", 15, 14);

            public static FontAsset PromptText = new FontAsset($"Core.{nameof(WindowKeyHints)}", 16, 14);
        }
    }
}

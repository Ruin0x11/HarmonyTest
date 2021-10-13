using Love;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data.Types
{
    public class ColorAsset : Def
    {
        public ColorAsset(string id, byte r, byte g, byte b, byte a = 255) : base(id)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }

        public ColorAsset(string id, Love.Color color) : this(id, color.r, color.g, color.b, color.a)
        {
        }

        public byte R;
        public byte G;
        public byte B;
        public byte A;

        private Love.Color? _LoveObject;
        public Love.Color LoveObject {
            get
            {
                if (_LoveObject == null)
                    _LoveObject = Love.Color.FromRGBA(R, G, B, A);
                return _LoveObject.Value;
            }
        }
        public static implicit operator Love.Color(ColorAsset c) => c.LoveObject;

        public static class Entries
        {
            public static ColorAsset TextWhite = new ColorAsset($"Core.{nameof(TextWhite)}", Color.White);
            public static ColorAsset TextBlack = new ColorAsset($"Core.{nameof(TextBlack)}", Color.Black);

            public static ColorAsset ListSelectedAdd = new ColorAsset($"Core.{nameof(ListSelectedAdd)}", 50, 50, 50);
            public static ColorAsset ListSelectedSub = new ColorAsset($"Core.{nameof(ListSelectedSub)}", 30, 10, 0);

            public static ColorAsset WindowBottomLine1 = new ColorAsset($"Core.{nameof(WindowBottomLine1)}", 194, 170, 146);
            public static ColorAsset WindowBottomLine2 = new ColorAsset($"Core.{nameof(WindowBottomLine2)}", 234, 220, 188);

            public static ColorAsset TopicWindowStyle0 = new ColorAsset($"Core.{nameof(TopicWindowStyle0)}", 255, 255, 255);
            public static ColorAsset TopicWindowStyle1 = new ColorAsset($"Core.{nameof(TopicWindowStyle1)}", 255 - 195, 255 - 205, 255 - 195);
            public static ColorAsset TopicWindowStyle2 = new ColorAsset($"Core.{nameof(TopicWindowStyle2)}", 255 - 210, 255 - 215, 255 - 205);
            public static ColorAsset TopicWindowStyle3 = new ColorAsset($"Core.{nameof(TopicWindowStyle3)}", 255 - 10, 255 - 13, 255 - 16);
            public static ColorAsset TopicWindowStyle4 = new ColorAsset($"Core.{nameof(TopicWindowStyle4)}", 255 - 195, 255 - 205, 255 - 195);
            public static ColorAsset TopicWindowStyle5 = new ColorAsset($"Core.{nameof(TopicWindowStyle5)}", 255, 255, 255);
            public static ColorAsset TopicWindowStyle6 = new ColorAsset($"Core.{nameof(TopicWindowStyle6)}", 255, 255, 255, 180);

            public static ColorAsset PromptBackground = new ColorAsset($"Core.{nameof(PromptBackground)}", 0, 0, 0, 127);
        }
    }
}

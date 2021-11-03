using OpenNefia.Core.Data.Serial;

namespace OpenNefia.Core.Data.Types
{
    public class ColorDef : Def
    {
        public ColorDef(string id) : this(id, 255, 255, 255, 255) { }

        public ColorDef(string id, byte r, byte g, byte b, byte a = 255) : base(id)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }

        public ColorDef(string id, Love.Color color) : this(id, color.r, color.g, color.b, color.a)
        {
        }

        [DefUseAttributes]
        [DefRequired]
        public byte R;

        [DefUseAttributes]
        [DefRequired]
        public byte G;

        [DefUseAttributes]
        [DefRequired]
        public byte B;

        [DefUseAttributes]
        [DefRequired]
        public byte A = 255;

        private Love.Color? _LoveObject;
        public Love.Color LoveObject {
            get
            {
                if (_LoveObject == null)
                    _LoveObject = Love.Color.FromRGBA(R, G, B, A);
                return _LoveObject.Value;
            }
        }
        public static implicit operator Love.Color(ColorDef c) => c.LoveObject;
        public static implicit operator Love.Color?(ColorDef? c) => c?.LoveObject;

        public override void OnMerge()
        {
            this._LoveObject = null;
        }
    }
}

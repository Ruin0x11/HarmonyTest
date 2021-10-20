using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Element.List
{
    public class UiListCell<T> : BaseDrawable, IUiListCell<T>
    {
        public T Data { get; set; }
        public UiListChoiceKey? Key { get; }

        public string Text
        {
            get => this.UiText.Text;
            set => this.UiText.Text = value;
        }
        protected IUiText UiText;
        protected IUiText KeyNameText;

        public int TextWidth { get => this.UiText.Width; }

        public int XOffset { get; set; }

        protected AssetDrawable AssetListBullet;
        public AssetDrawable AssetSelectKey;
        protected FontDef FontListKeyName;
        public ColorDef ColorSelectedAdd;
        public ColorDef ColorSelectedSub;

        public UiListCell(T data, IUiText text, UiListChoiceKey? key = null)
        {
            this.Data = data;
            this.UiText = text;
            this.Key = key;

            this.AssetSelectKey = new AssetDrawable(AssetDefOf.SelectKey);
            this.AssetListBullet = new AssetDrawable(AssetDefOf.ListBullet);
            this.FontListKeyName = FontDefOf.ListKeyName;
            this.ColorSelectedAdd = ColorDefOf.ListSelectedAdd;
            this.ColorSelectedSub = ColorDefOf.ListSelectedSub;

            var keyName = string.Empty;
            if (this.Key != null)
            {
                keyName = UiUtils.GetKeyName(this.Key.Key);
            }
            this.KeyNameText = new UiText(this.FontListKeyName, keyName);
        }

        public UiListCell(T data, string text, UiListChoiceKey? key = null) : this(data, new UiText(FontDefOf.ListText, text), key) {}

        public override void SetPosition(int x, int y)
        {
            base.SetPosition(x, y);
            this.UiText.SetPosition(x + this.AssetSelectKey.Width + 2 + 4 + this.XOffset, y + 1);

            var keyNameX = x + (this.AssetSelectKey.Width - this.KeyNameText.Width) / 2 - 2;
            var keyNameY = y + (this.AssetSelectKey.Height - GraphicsEx.GetTextHeight()) / 2;
            this.KeyNameText.SetPosition(keyNameX, keyNameY);
        }

        public override void SetSize(int width = -1, int height = -1)
        {
            this.UiText.SetSize(width - this.AssetSelectKey.Width - 6 + this.XOffset, height);
            this.KeyNameText.SetSize();
            base.SetSize(Math.Max(width, this.UiText.Width + this.KeyNameText.Width + 6 + this.XOffset), height);
        }

        public virtual void DrawHighlight()
        {
            var width = Math.Clamp(this.TextWidth + this.AssetSelectKey.Width + 8 + this.XOffset, 10, 480);
            Love.Graphics.SetBlendMode(Love.BlendMode.Subtract);
            GraphicsEx.SetColor(this.ColorSelectedSub);
            GraphicsEx.FilledRect(this.UiText.X - 4, this.Y - 2, width, 19);
            Love.Graphics.SetBlendMode(Love.BlendMode.Add);
            GraphicsEx.SetColor(this.ColorSelectedAdd);
            GraphicsEx.FilledRect(this.UiText.X - 3, this.Y - 1, width - 2, 17);
            Love.Graphics.SetBlendMode(Love.BlendMode.Alpha);
            GraphicsEx.SetColor(Love.Color.White);
            this.AssetListBullet.Draw(this.UiText.X - 5 + width - 20, this.Y + 2);
        }

        public override void Draw()
        {
            GraphicsEx.SetColor(Love.Color.White);
            this.AssetSelectKey.Draw(this.X, this.Y - 1);
            this.KeyNameText.Draw();
            this.UiText.Draw();
        }

        public override void Update(float dt)
        {
            this.KeyNameText.Update(dt);
            this.UiText.Update(dt);
        }

        public override void Dispose()
        {
            this.AssetSelectKey.Dispose();
            this.KeyNameText.Dispose();
            this.UiText.Dispose();
        }
    }
}

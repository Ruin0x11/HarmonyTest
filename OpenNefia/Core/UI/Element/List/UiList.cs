using Love;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Element.List
{
    public class UiList<T> : BaseInputUiElement, IUiList<T>
    {
        protected IListModel<T> Model { get; }
        public uint ItemHeight { get; }
        public int ItemOffsetX { get; }
        public int ItemOffsetY { get; }

        protected AssetDrawable AssetSelectKey;
        protected List<UiShadowedText> KeyNameTexts;

        public int SelectedIndex { get => this.Model.SelectedIndex; }
        public T? SelectedItem { get => this.Model.SelectedItem; }


        public UiList(IListModel<T> choices, uint itemHeight = 19, int itemOffsetX = 0, int itemOffsetY = -2)
        {
            this.Model = choices;
            this.ItemHeight = itemHeight;
            this.ItemOffsetX = 0;
            this.ItemOffsetY = -2;

            this.AssetSelectKey = new AssetDrawable(Asset.Entries.SelectKey);

            var font = Drawing.GetFont(13);
            this.KeyNameTexts = new List<UiShadowedText>();
            for (int i = 0; i < Keybind.Entries.SelectionKeys.Length; i++)
            {
                this.KeyNameTexts.Add(new UiShadowedText("a", font));
            }

            this.BindKeys();
        }

        protected virtual void BindKeys()
        {
            for (int i = 0; i < Keybind.Entries.SelectionKeys.Length; i++)
            {
                var selectionKeybind = Keybind.Entries.SelectionKeys[i];
                this.BindKey(selectionKeybind, (_) => {
                    this.Activate(i);
                    return null;
                });
            }

            this.BindKey(Keybind.Entries.UIUp, (_) => { this.IncrementIndex(-1); return null; });
            this.BindKey(Keybind.Entries.UIDown, (_) => { this.IncrementIndex(1); return null; });
            this.BindKey(Keybind.Entries.Enter, (_) => { this.Activate(this.SelectedIndex); return null; });
        }

        public override List<UiKeyHint> MakeKeyHints()
        {
            return new List<UiKeyHint>();
        }

        public void IncrementIndex(int delta) => this.Model.IncrementIndex(delta);
        public bool CanSelect(int index) => this.Model.CanSelect(index);
        public void Select(int index) => this.Model.Select(index);
        public bool CanActivate(int index) => this.Model.CanActivate(index);
        public void Activate(int index) => this.Model.Activate(index);

        public override void Relayout(int x, int y, int width, int height)
        {
            base.Relayout(x, y, width, height);
        }

        protected void DrawSelectKey(int index, string keyName, int x, int y)
        {
            Drawing.SetColor(Color.White);
            this.AssetSelectKey.Draw(x, y);

            var text = this.KeyNameTexts[index];
            text.X = x + (this.AssetSelectKey.Width - Drawing.GetTextWidth(text.Text)) / 2 - 2;
            text.Y = y + (this.AssetSelectKey.Height - Drawing.GetTextHeight()) / 2;
            this.KeyNameTexts[index].Draw();
        }

        protected void DrawItemText(int index, string text, int x, int y, int xOffset = 0, Love.Color? textColor = null)
        {
            if (index == this.SelectedIndex)
            {
                var width = Math.Clamp(Drawing.GetTextWidth(text) + 32 + xOffset, 10, 480);
            }
        }

        public override void Update(float dt)
        {
        }

        public override void Draw()
        {
        }
    }
}

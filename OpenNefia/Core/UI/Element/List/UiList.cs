using Love;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Rendering;
using System;
using System.Collections;
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
        protected AssetDrawable AssetListBullet;
        protected List<UiShadowedText> KeyNameTexts;
        protected List<UiText> ItemTexts;

        protected ColorAsset ColorSelectedAdd;
        protected ColorAsset ColorSelectedSub;

        protected FontAsset FontListText;
        protected FontAsset FontListKeyName;

        public int SelectedIndex { get => this.Model.SelectedIndex; }
        public T? SelectedItem { get => this.Model.SelectedItem; }

        public int Count => this.Model.Count;
        public bool IsReadOnly => this.Model.IsReadOnly;

        public T this[int index] { get => this.Model[index]; set => this.Model[index] = value; }

        public UiList(IListModel<T> choices, uint itemHeight = 19, int itemOffsetX = 0, int itemOffsetY = -2)
        {
            this.Model = choices;
            this.ItemHeight = itemHeight;
            this.ItemOffsetX = 0;
            this.ItemOffsetY = -2;

            this.AssetSelectKey = new AssetDrawable(Asset.Entries.SelectKey);
            this.AssetListBullet = new AssetDrawable(Asset.Entries.ListBullet);
            this.ColorSelectedAdd = ColorAsset.Entries.ListSelectedAdd;
            this.ColorSelectedSub = ColorAsset.Entries.ListSelectedSub;
            this.FontListText = FontAsset.Entries.ListText;
            this.FontListKeyName = FontAsset.Entries.ListKeyName;

            var font = Drawing.GetFont(this.FontListKeyName);
            this.KeyNameTexts = new List<UiShadowedText>();
            for (int i = 0; i < Keybind.Entries.SelectionKeys.Length; i++)
            {
                this.KeyNameTexts.Add(new UiShadowedText("a", font));
            }

            this.ItemTexts = new List<UiText>();

            this.RefreshTexts();
            this.BindKeys();
        }

        private void RefreshTexts()
        {
            this.ItemTexts = this.Model.Select(x => new UiText(this.GetItemText(x), this.FontListText, this.GetItemColor(x))).ToList();
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

        public virtual string GetItemText(T choice) => this.Model.GetItemText(choice);
        public virtual ColorAsset GetItemColor(T item) => ColorAsset.Entries.TextBackground;

        public void IncrementIndex(int delta) => this.Model.IncrementIndex(delta);
        public virtual bool CanSelect(int index) => this.Model.CanSelect(index);
        public virtual void Select(int index) => this.Model.Select(index);
        public virtual bool CanActivate(int index) => this.Model.CanActivate(index);
        public virtual void Activate(int index) => this.Model.Activate(index);

        public IEnumerator<T> GetEnumerator() => this.Model.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.Model.GetEnumerator();

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

        protected void DrawItemText(int index, UiText text, int x, int y, int xOffset = 0, Love.Color? textColor = null)
        {
            if (index == this.SelectedIndex)
            {
                var width = Math.Clamp(text.Width + 32 + xOffset, 10, 480);
                Love.Graphics.SetBlendMode(Love.BlendMode.Subtract);
                Drawing.SetColor(this.ColorSelectedSub);
                Drawing.DrawFilledRect(x, y - 2, width, 19);
                Love.Graphics.SetBlendMode(Love.BlendMode.Add);
                Drawing.SetColor(this.ColorSelectedAdd);
                Drawing.DrawFilledRect(x + 1, y - 1, width - 2, 17);
                Love.Graphics.SetBlendMode(Love.BlendMode.Alpha);
                Drawing.SetColor(Love.Color.White);
                this.AssetListBullet.Draw(x + width - 20, y + 2);
            }
            text.X = x + 4 + xOffset;
            text.Y = y + 1;
            text.Draw();
        }

        public override void Update(float dt)
        {
        }

        public override void Draw()
        {
        }

        public int IndexOf(T item) => this.Model.IndexOf(item);
        public void Insert(int index, T item) => this.Model.Insert(index, item);
        public void RemoveAt(int index) => this.Model.RemoveAt(index);
        public void Add(T item) => this.Model.Add(item);
        public void Clear() => this.Model.Clear();
        public bool Contains(T item) => this.Model.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => this.Model.CopyTo(array, arrayIndex);
        public bool Remove(T item) => this.Model.Remove(item); 
    }
}

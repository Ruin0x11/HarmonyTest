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
        public int ItemHeight { get; }
        public int ItemOffsetX { get; }
        public int ItemOffsetY { get; }

        protected AssetDrawable AssetSelectKey;
        protected AssetDrawable AssetListBullet;
        public ColorAsset ColorSelectedAdd;
        public ColorAsset ColorSelectedSub;
        public FontAsset FontListText;
        public FontAsset FontListKeyName;

        protected List<IUiText> KeyNameTexts;
        protected List<IUiText> ItemTexts;

        public int SelectedIndex { get => this.Model.SelectedIndex; }
        public T? SelectedChoice { get => this.Model.SelectedChoice; }

        public int Count => this.Model.Count;
        public bool IsReadOnly => this.Model.IsReadOnly;

        public T this[int index] { get => this.Model[index]; set => this.Model[index] = value; }

        public UiList(IListModel<T> choices, int itemHeight = 19, int itemOffsetX = 0, int itemOffsetY = -2)
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

            var font = GraphicsEx.GetFont(this.FontListKeyName);
            this.KeyNameTexts = new List<IUiText>();
            for (int i = 0; i < this.Count; i++)
            {
                var key = this.GetChoiceKey(i);
                this.KeyNameTexts.Add(new UiShadowedText("a", font));
            }

            this.ItemTexts = new List<IUiText>();

            this.RefreshTexts();
            this.BindKeys();
        }

        public UiList(List<T> choices, int itemHeight = 19, int itemOffsetX = 0, int itemOffsetY = -2)
            : this(new ListModel<T>(choices), itemHeight, itemOffsetX, itemOffsetY)
        {
        }

        public UiList(int itemHeight = 19, int itemOffsetX = 0, int itemOffsetY = -2)
            : this(new ListModel<T>(), itemHeight, itemOffsetX, itemOffsetY)
        {
        }

        private void RefreshTexts()
        {
            this.ItemTexts = this.Model.Select((x, i) => new UiText(this.GetChoiceText(i), this.FontListText, this.GetChoiceColor(i))).ToList<IUiText>();
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


        public virtual string GetChoiceText(int index) => this[index]!.ToString()!;
        public virtual ColorAsset GetChoiceColor(int index) => ColorAsset.Entries.TextBackground;
        public virtual Keys GetChoiceKey(int index) => Keys.A + index;

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

        protected virtual void DrawSelectKey(int index, int x, int y)
        {
            GraphicsEx.SetColor(Color.White);
            this.AssetSelectKey.Draw(x, y);

            var text = this.KeyNameTexts[index];
            text.X = x + (this.AssetSelectKey.Width - GraphicsEx.GetTextWidth(text.Text)) / 2 - 2;
            text.Y = y + (this.AssetSelectKey.Height - GraphicsEx.GetTextHeight()) / 2;
            this.KeyNameTexts[index].Draw();
        }

        protected virtual void DrawItemText(int index, int x, int y, int xOffset = 0)
        {
            var text = this.ItemTexts[index];

            if (index == this.SelectedIndex)
            {
                var width = Math.Clamp(text.Width + 32 + xOffset, 10, 480);
                Love.Graphics.SetBlendMode(Love.BlendMode.Subtract);
                GraphicsEx.SetColor(this.ColorSelectedSub);
                GraphicsEx.DrawFilledRect(x, y - 2, width, 19);
                Love.Graphics.SetBlendMode(Love.BlendMode.Add);
                GraphicsEx.SetColor(this.ColorSelectedAdd);
                GraphicsEx.DrawFilledRect(x + 1, y - 1, width - 2, 17);
                Love.Graphics.SetBlendMode(Love.BlendMode.Alpha);
                GraphicsEx.SetColor(Love.Color.White);
                this.AssetListBullet.Draw(x + width - 20, y + 2);
            }

            text.X = x + 4 + xOffset;
            text.Y = y + 1;
            text.Draw();
        }

        protected virtual void DrawItem(int index, int x, int y)
        {
            this.DrawSelectKey(index, x, y);
            this.DrawItemText(index, x + 26, y + 1, 0);
        }

        public override void Update(float dt)
        {
            foreach (var text in this.KeyNameTexts)
                text.Update(dt);

            foreach (var text in this.ItemTexts)
                text.Update(dt);
        }

        public override void Draw()
        {
            for (int index = 0; index < this.Count; index++)
            {
                var x = this.X + this.ItemOffsetX;
                var y = index * this.ItemHeight + this.Y + this.ItemOffsetY;
                this.DrawItem(index, x, y);
            }
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

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
        protected IListModel<IUiListCell<T>> Model { get; }
        public int ItemHeight { get; }
        public int ItemOffsetX { get; }
        public int ItemOffsetY { get; }

        public int SelectedIndex { get => this.Model.SelectedIndex; }
        public IUiListCell<T> SelectedChoice { get => this.Model.SelectedChoice; }

        protected List<IUiText> KeyNameTexts;

        public int Count => this.Model.Count;
        public bool IsReadOnly => this.Model.IsReadOnly;

        protected AssetDrawable AssetSelectKey;
        protected AssetDrawable AssetListBullet;
        public ColorAsset ColorSelectedAdd;
        public ColorAsset ColorSelectedSub;
        public FontAsset FontListKeyName;

        public IUiListCell<T> this[int index] { get => this.Model[index]; set => this.Model[index] = value; }

        public UiList(IEnumerable<T> choices, int itemHeight = 19, int itemOffsetX = 0, int itemOffsetY = -2)
        {
            this.ItemHeight = itemHeight;
            this.ItemOffsetX = 0;
            this.ItemOffsetY = -2;

            this.AssetSelectKey = new AssetDrawable(Asset.Entries.SelectKey);
            this.AssetListBullet = new AssetDrawable(Asset.Entries.ListBullet);
            this.ColorSelectedAdd = ColorAsset.Entries.ListSelectedAdd;
            this.ColorSelectedSub = ColorAsset.Entries.ListSelectedSub;
            this.FontListKeyName = FontAsset.Entries.ListKeyName;

            var cells = choices.Select((c, i) => this.MakeChoiceCell(c, i));
            this.Model = new ListModel<IUiListCell<T>>(cells);

            var font = GraphicsEx.GetFont(this.FontListKeyName);
            this.KeyNameTexts = new List<IUiText>();
            for (int i = 0; i < this.Count; i++)
            {
                var key = this.GetChoiceKey(this[i].Data, i);
                this.KeyNameTexts.Add(new UiShadowedText("a", font));
            }

            this.BindKeys();
        }

        public UiList(int itemHeight = 19, int itemOffsetX = 0, int itemOffsetY = -2)
            : this(new List<T>(), itemHeight, itemOffsetX, itemOffsetY)
        {
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

        public virtual IUiListCell<T> MakeChoiceCell(T choice, int index)
        {
            return new UiListCell<T>(choice, this.GetChoiceText(choice, index));
        }

        public virtual string GetChoiceText(T choice, int index) => $"{choice}";
        public virtual Keys GetChoiceKey(T choice, int index) => Keys.A + index;

        public void IncrementIndex(int delta) => this.Model.IncrementIndex(delta);
        public virtual bool CanSelect(int index) => this.Model.CanSelect(index);
        public virtual void Select(int index) => this.Model.Select(index);
        public virtual bool CanActivate(int index) => this.Model.CanActivate(index);
        public virtual void Activate(int index) => this.Model.Activate(index);

        public IEnumerator<IUiListCell<T>> GetEnumerator() => this.Model.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.Model.GetEnumerator();

        public override void Relayout(int x, int y, int width, int height)
        {
            base.Relayout(x, y, width, height);

            var totalHeight = 0;

            for (int index = 0; index < this.Count; index++)
            {
                var cell = this.Model[index];
                var ix = this.X + this.ItemOffsetX;
                var iy = index * this.ItemHeight + this.Y + this.ItemOffsetY;

                cell.Relayout(ix, iy, width, this.ItemHeight);
                totalHeight += cell.Height;
                this.Width = Math.Max(this.Width, cell.Width);
            }

            this.Height = Math.Max(this.Height, totalHeight);
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

        public override void Update(float dt)
        {
            foreach (var cell in this.Model)
                cell.Update(dt);
        }

        public override void Draw()
        {
            for (int index = 0; index < this.Count; index++)
            {
                var cell = this.Model[index];
                cell.Draw();

                if (index == this.SelectedIndex)
                {
                    var width = Math.Clamp(cell.Width + 32 + cell.XOffset, 10, 480);
                    Love.Graphics.SetBlendMode(Love.BlendMode.Subtract);
                    GraphicsEx.SetColor(this.ColorSelectedSub);
                    GraphicsEx.DrawFilledRect(cell.X, cell.Y - 2, width, 19);
                    Love.Graphics.SetBlendMode(Love.BlendMode.Add);
                    GraphicsEx.SetColor(this.ColorSelectedAdd);
                    GraphicsEx.DrawFilledRect(cell.X + 1, cell.Y - 1, width - 2, 17);
                    Love.Graphics.SetBlendMode(Love.BlendMode.Alpha);
                    GraphicsEx.SetColor(Love.Color.White);
                    this.AssetListBullet.Draw(cell.X + width - 20, cell.Y + 2);
                }
            }
        }

        public int IndexOf(IUiListCell<T> item) => this.Model.IndexOf(item);
        public void Insert(int index, IUiListCell<T> item) => this.Model.Insert(index, item);
        public void RemoveAt(int index) => this.Model.RemoveAt(index);
        public void Add(IUiListCell<T> item) => this.Model.Add(item);
        public void Clear() => this.Model.Clear();
        public bool Contains(IUiListCell<T> item) => this.Model.Contains(item);
        public void CopyTo(IUiListCell<T>[] array, int arrayIndex) => this.Model.CopyTo(array, arrayIndex);
        public bool Remove(IUiListCell<T> item) => this.Model.Remove(item); 
    }
}

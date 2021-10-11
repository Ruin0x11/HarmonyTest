using Love;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Data.Types.DefOf;
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
        protected IList<IUiListCell<T>> Cells { get; }
        public int ItemHeight { get; }
        public int ItemOffsetX { get; }
        public int ItemOffsetY { get; }

        public bool HighlightSelected { get; set; }
        public bool SelectOnActivate { get; set; }

        private int _SelectedIndex;
        public int SelectedIndex { 
            get => _SelectedIndex; 
            set
            {
                if (value < 0 || value >= this.Cells.Count)
                    throw new ArgumentException($"Index {value} is out of bounds (count: {this.Cells.Count})");
                this._SelectedIndex = value;
            }
        }
        public IUiListCell<T> SelectedCell { get => this.Cells[this.SelectedIndex]!; }

        protected List<UiListChoiceKey> ChoiceKeys;
        protected List<IUiText> KeyNameTexts;

        protected AssetDrawable AssetSelectKey;
        protected AssetDrawable AssetListBullet;
        public ColorAsset ColorSelectedAdd;
        public ColorAsset ColorSelectedSub;
        public FontAsset FontListKeyName;

        public event UiListEventHandler<T>? EventOnSelect;
        public event UiListEventHandler<T>? EventOnActivate;

        public UiList(IEnumerable<T> choices, int itemHeight = 19, int itemOffsetX = 0, int itemOffsetY = -2)
        {
            this.ItemHeight = itemHeight;
            this.ItemOffsetX = 0;
            this.ItemOffsetY = -2;
            this.HighlightSelected = true;
            this.SelectOnActivate = true;

            this.AssetSelectKey = new AssetDrawable(Asset.Entries.SelectKey);
            this.AssetListBullet = new AssetDrawable(Asset.Entries.ListBullet);
            this.ColorSelectedAdd = ColorAsset.Entries.ListSelectedAdd;
            this.ColorSelectedSub = ColorAsset.Entries.ListSelectedSub;
            this.FontListKeyName = FontAsset.Entries.ListKeyName;

            this.Cells = choices.Select((c, i) => this.MakeChoiceCell(c, i)).ToList();

            this.ChoiceKeys = new List<UiListChoiceKey>();
            this.KeyNameTexts = new List<IUiText>();
            for (int i = 0; i < this.Count; i++)
            {
                var choiceKey = this.GetChoiceKey(this[i].Data, i);
                var keyName = UiUtils.GetKeyName(choiceKey.Key);
                this.ChoiceKeys.Add(choiceKey);
                this.KeyNameTexts.Add(new UiShadowedText(this.FontListKeyName, keyName));
            }

            this.BindKeys();
        }

        public UiList(int itemHeight = 19, int itemOffsetX = 0, int itemOffsetY = -2)
            : this(new List<T>(), itemHeight, itemOffsetX, itemOffsetY)
        {
        }

        protected virtual void BindKeys()
        {
            for (int i = 0; i < this.ChoiceKeys.Count; i++)
            {
                var choiceKey = this.ChoiceKeys[i];
                IKeybind keybind;
                if (choiceKey.UseKeybind)
                {
                    if (Keybind.Entries.SelectionKeys.ContainsKey(choiceKey.Key))
                    {
                        keybind = Keybind.Entries.SelectionKeys[choiceKey.Key];
                    }
                    else
                    {
                        keybind = new RawKey(choiceKey.Key);
                    }
                }
                else
                {
                    keybind = new RawKey(choiceKey.Key);
                }

                // C# doesn't capture locals in closures like Lua does with upvalues.
                var indexCopy = i;
                this.Keybinds[keybind] += (_) => this.Activate(indexCopy);
            }

            this.Keybinds[Keybind.Entries.UIUp] += (_) =>
            {
                Gui.PlaySound(SoundDefOf.Cursor1);
                this.IncrementIndex(-1);
            };
            this.Keybinds[Keybind.Entries.UIDown] += (_) =>
            {
                Gui.PlaySound(SoundDefOf.Cursor1);
                this.IncrementIndex(1);
            };
            this.Keybinds[Keybind.Entries.Enter] += (_) => this.Activate(this.SelectedIndex);

            this.MouseMoved.Callback += (evt) =>
            {
                for (var index = 0; index < this.Cells.Count; index++)
                {
                    if (this.Cells[index].ContainsPoint(evt.X, evt.Y))
                    {
                        if (this.SelectedIndex != index)
                        {
                            Gui.PlaySound(SoundDefOf.Cursor1);
                            this.Select(index);
                        }
                        break;
                    }
                }
            };

            this.MouseButtons[UI.MouseButtons.Mouse1] += (evt) =>
            {
                if (this.SelectedCell.ContainsPoint(evt.X, evt.Y))
                {
                    this.Activate(this.SelectedIndex);
                }
            };
        }

        #region Data Creation

        public override List<UiKeyHint> MakeKeyHints()
        {
            return new List<UiKeyHint>();
        }

        public virtual IUiListCell<T> MakeChoiceCell(T choice, int index)
        {
            return new UiListCell<T>(choice, this.GetChoiceText(choice, index));
        }

        public virtual string GetChoiceText(T choice, int index) => $"{choice}";
        public virtual UiListChoiceKey GetChoiceKey(T choice, int index) => new UiListChoiceKey(Keys.A + index);

        #endregion

        #region List Handling

        protected virtual void OnSelect(UiListEventArgs<T> e)
        {
            UiListEventHandler<T>? handler = EventOnSelect;
            handler?.Invoke(this, e);
        }

        protected virtual void OnActivate(UiListEventArgs<T> e)
        {
            UiListEventHandler<T>? handler = EventOnActivate;
            handler?.Invoke(this, e);
        }

        public virtual bool CanSelect(int index)
        {
            return index >= 0 && index < Cells.Count;
        }

        public void IncrementIndex(int delta)
        {
            var newIndex = this.SelectedIndex + delta;
            var sign = Math.Sign(delta);

            while (!this.CanSelect(newIndex) && newIndex != SelectedIndex)
            {
                newIndex += sign;
                if (newIndex < 0)
                    newIndex = this.Count - 1;
                else if (newIndex >= this.Count)
                    newIndex = 0;
            }
            this.Select(newIndex);
        }

        public void Select(int index)
        {
            if (!this.CanSelect(index))
            {
                return;
            }

            this.SelectedIndex = index;
            this.OnSelect(new UiListEventArgs<T>(this[index], index));
        }

        public virtual bool CanActivate(int index)
        {
            return index >= 0 && index < Cells.Count;
        }

        public void Activate(int index)
        {
            if (!this.CanActivate(index))
            {
                return;
            }

            if (this.SelectOnActivate)
                this.Select(index);

            this.OnActivate(new UiListEventArgs<T>(this[index], index));
        }

        #endregion

        #region UI Handling

        public override void SetPosition(int x, int y)
        {
            base.SetPosition(x, y);

            for (int index = 0; index < this.Count; index++)
            {
                var cell = this.Cells[index];
                var ix = this.X + this.ItemOffsetX;
                var iy = index * this.ItemHeight + this.Y + this.ItemOffsetY;

                cell.SetPosition(ix + this.AssetSelectKey.Width + 2, iy + 1);

                var text = this.KeyNameTexts[index];
                var textX = ix + (this.AssetSelectKey.Width - GraphicsEx.GetTextWidth(text.Text)) / 2 - 2;
                var textY = iy + (this.AssetSelectKey.Height - GraphicsEx.GetTextHeight()) / 2;
                text.SetPosition(textX, textY);
            }
        }

        public override void SetSize(int width, int height)
        {
            var totalHeight = 0;

            for (int index = 0; index < this.Count; index++)
            {
                var cell = this.Cells[index];
                cell.SetSize(width - (this.AssetSelectKey.Width + 2), this.ItemHeight);
                totalHeight += cell.Height;
            }

            base.SetSize(width, Math.Max(height, totalHeight));
        }

        protected virtual void DrawSelectKey(int index)
        {
            var cell = this.Cells[index];
            GraphicsEx.SetColor(Color.White);
            this.AssetSelectKey.Draw(cell.X - (this.AssetSelectKey.Width + 2), cell.Y - 1);
            this.KeyNameTexts[index].Draw();
        }
        
        protected virtual void DrawHighlight(int index)
        {
            var cell = this.Cells[index];

            var width = Math.Clamp(cell.TextWidth + this.AssetSelectKey.Width + 8 + cell.XOffset, 10, 480);
            Love.Graphics.SetBlendMode(Love.BlendMode.Subtract);
            GraphicsEx.SetColor(this.ColorSelectedSub);
            GraphicsEx.FilledRect(cell.X, cell.Y - 2, width, 19);
            Love.Graphics.SetBlendMode(Love.BlendMode.Add);
            GraphicsEx.SetColor(this.ColorSelectedAdd);
            GraphicsEx.FilledRect(cell.X + 1, cell.Y - 1, width - 2, 17);
            Love.Graphics.SetBlendMode(Love.BlendMode.Alpha);
            GraphicsEx.SetColor(Love.Color.White);
            this.AssetListBullet.Draw(cell.X + width - 20, cell.Y + 2);
        }

        public override void Update(float dt)
        {
            foreach (var cell in this.Cells)
                cell.Update(dt);
        }

        public override void Draw()
        {
            for (int index = 0; index < this.Count; index++)
            {
                this.DrawSelectKey(index);

                var cell = this.Cells[index];
                cell.Draw();

                if (this.HighlightSelected && index == this.SelectedIndex)
                {
                    this.DrawHighlight(index);
                }
            }
        }

        #endregion

        #region IList implementation

        public int Count => this.Cells.Count;
        public bool IsReadOnly => this.Cells.IsReadOnly;

        public IUiListCell<T> this[int index] { get => this.Cells[index]; set => this.Cells[index] = value; }
        public int IndexOf(IUiListCell<T> item) => this.Cells.IndexOf(item);
        public void Insert(int index, IUiListCell<T> item) => this.Cells.Insert(index, item);
        public void RemoveAt(int index) => this.Cells.RemoveAt(index);
        public void Add(IUiListCell<T> item) => this.Cells.Add(item);
        public void Clear() => this.Cells.Clear();
        public bool Contains(IUiListCell<T> item) => this.Cells.Contains(item);
        public void CopyTo(IUiListCell<T>[] array, int arrayIndex) => this.Cells.CopyTo(array, arrayIndex);
        public bool Remove(IUiListCell<T> item) => this.Cells.Remove(item);

        public bool IsFixedSize => false;
        public bool IsSynchronized => false;
        public object SyncRoot => this;

        public IEnumerator<IUiListCell<T>> GetEnumerator() => this.Cells.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.Cells.GetEnumerator();

        #endregion
    }
}

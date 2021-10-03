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
        protected IList<IUiListCell<T>> Choices { get; }
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
                if (value < 0 || value >= this.Choices.Count)
                    throw new ArgumentException($"Index {value} is out of bounds (count: {this.Choices.Count})");
                this._SelectedIndex = value;
            }
        }
        public IUiListCell<T> SelectedChoice { get => this.Choices[this.SelectedIndex]!; }

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

            this.Choices = choices.Select((c, i) => this.MakeChoiceCell(c, i)).ToList();

            var font = GraphicsEx.GetFont(this.FontListKeyName);
            this.ChoiceKeys = new List<UiListChoiceKey>();
            this.KeyNameTexts = new List<IUiText>();
            for (int i = 0; i < this.Count; i++)
            {
                var choiceKey = this.GetChoiceKey(this[i].Data, i);
                var keyName = UiUtils.GetKeyName(choiceKey.Key);
                this.ChoiceKeys.Add(choiceKey);
                this.KeyNameTexts.Add(new UiShadowedText(keyName, font));
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

                var indexCopy = i;
                this.BindKey(keybind, (_) => this.Activate(indexCopy));
            }

            this.BindKey(Keybind.Entries.UIUp, (_) => this.IncrementIndex(-1));
            this.BindKey(Keybind.Entries.UIDown, (_) => this.IncrementIndex(1));
            this.BindKey(Keybind.Entries.Enter, (_) => this.Activate(this.SelectedIndex));
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
            return index >= 0 && index < Choices.Count;
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
            return index >= 0 && index < Choices.Count;
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

        public override void Relayout(int x, int y, int width, int height)
        {
            base.Relayout(x, y, width, height);

            var totalHeight = 0;

            for (int index = 0; index < this.Count; index++)
            {
                var cell = this.Choices[index];
                var ix = this.X + this.ItemOffsetX + 26;
                var iy = index * this.ItemHeight + this.Y + this.ItemOffsetY + 1;

                cell.Relayout(ix, iy, width, this.ItemHeight);
                totalHeight += cell.Height;
                this.Width = Math.Max(this.Width, cell.Width);
            }

            this.Height = Math.Max(this.Height, totalHeight);
        }

        protected virtual void DrawSelectKey(int index)
        {
            var cell = this.Choices[index];
            var x = cell.X - 26;
            var y = cell.Y - 1;

            GraphicsEx.SetColor(Color.White);
            this.AssetSelectKey.Draw(x, y);

            var text = this.KeyNameTexts[index];
            text.X = x + (this.AssetSelectKey.Width - GraphicsEx.GetTextWidth(text.Text)) / 2 - 2;
            text.Y = y + (this.AssetSelectKey.Height - GraphicsEx.GetTextHeight()) / 2;
            this.KeyNameTexts[index].Draw();
        }
        
        protected virtual void DrawHighlight(int index)
        {
            var cell = this.Choices[index];

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

        public override void Update(float dt)
        {
            foreach (var cell in this.Choices)
                cell.Update(dt);
        }

        public override void Draw()
        {
            for (int index = 0; index < this.Count; index++)
            {
                this.DrawSelectKey(index);

                var cell = this.Choices[index];
                cell.Draw();

                if (this.HighlightSelected && index == this.SelectedIndex)
                {
                    this.DrawHighlight(index);
                }
            }
        }

        #endregion

        #region IList implementation

        public int Count => this.Choices.Count;
        public bool IsReadOnly => this.Choices.IsReadOnly;

        public IUiListCell<T> this[int index] { get => this.Choices[index]; set => this.Choices[index] = value; }
        public int IndexOf(IUiListCell<T> item) => this.Choices.IndexOf(item);
        public void Insert(int index, IUiListCell<T> item) => this.Choices.Insert(index, item);
        public void RemoveAt(int index) => this.Choices.RemoveAt(index);
        public void Add(IUiListCell<T> item) => this.Choices.Add(item);
        public void Clear() => this.Choices.Clear();
        public bool Contains(IUiListCell<T> item) => this.Choices.Contains(item);
        public void CopyTo(IUiListCell<T>[] array, int arrayIndex) => this.Choices.CopyTo(array, arrayIndex);
        public bool Remove(IUiListCell<T> item) => this.Choices.Remove(item);

        public bool IsFixedSize => false;
        public bool IsSynchronized => false;
        public object SyncRoot => this.SyncRoot;

        public IEnumerator<IUiListCell<T>> GetEnumerator() => this.Choices.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.Choices.GetEnumerator();

        #endregion
    }
}

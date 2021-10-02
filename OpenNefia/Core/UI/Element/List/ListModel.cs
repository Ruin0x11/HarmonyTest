using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Element.List
{
    public class ListModel<T> : IListModel<T>
    {
        public List<T> Choices;
        public int SelectedIndex { get; private set; }
        public T? SelectedChoice { get => this.Choices[SelectedIndex]; }

        public ListModel() : this(new List<T>()) 
        { 
        }

        public ListModel(List<T> choices)
        {
            this.Choices = choices;
        }

        public virtual bool CanSelect(int index)
        {
            return index >= 0 && index < Choices.Count;
        }

        public void IncrementIndex(int delta)
        {
            var newIndex = Math.Abs((this.SelectedIndex + delta) % Choices.Count);
            while (!this.CanSelect(newIndex) && newIndex != SelectedIndex)
            {
                newIndex = (newIndex + 1) % Choices.Count;
            }
        }

        public void Select(int index)
        {
            if (!this.CanSelect(index)) {
                return;
            }

            this.SelectedIndex = index;
            this.OnSelect(index);
        }

        protected virtual void OnSelect(int index)
        {
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

            this.OnActivate(index);
        }

        protected virtual void OnActivate(int index)
        {
        }

        public int IndexOf(T item) => this.Choices.IndexOf(item);
        public void Insert(int index, T item) => this.Choices.Insert(index, item);
        public void RemoveAt(int index) => this.Choices.RemoveAt(index);
        public void Add(T item) => this.Choices.Add(item);
        public void Clear() => this.Choices.Clear();
        public bool Contains(T item) => this.Choices.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => this.Choices.CopyTo(array, arrayIndex);
        public bool Remove(T item) => this.Choices.Remove(item);

        public int Count => this.Choices.Count;
        public bool IsReadOnly => false;
        public bool IsFixedSize => false;
        public bool IsSynchronized => false;
        public object SyncRoot => this.SyncRoot;
        public T this[int index] { get => this.Choices[index]; set => this.Choices[index] = value; }

        public IEnumerator<T> GetEnumerator() => this.Choices.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.Choices.GetEnumerator();
    }
}

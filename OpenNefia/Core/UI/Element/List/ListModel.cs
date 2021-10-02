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
        public List<T> Items;
        public int SelectedIndex { get; private set; }
        public T? SelectedItem { get => this.Items[SelectedIndex]; }

        public ListModel() : this(new List<T>()) 
        { 
        }

        public ListModel(List<T> items)
        {
            this.Items = items;
        }

        public virtual string GetItemText(T item) => item!.ToString()!;

        public virtual bool CanSelect(int index)
        {
            return index >= 0 && index < Items.Count;
        }

        public void IncrementIndex(int delta)
        {
            var newIndex = Math.Abs((this.SelectedIndex + delta) % Items.Count);
            while (!this.CanSelect(newIndex) && newIndex != SelectedIndex)
            {
                newIndex = (newIndex + 1) % Items.Count;
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
            return index >= 0 && index < Items.Count;
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

        public int IndexOf(T item) => this.Items.IndexOf(item);
        public void Insert(int index, T item) => this.Items.Insert(index, item);
        public void RemoveAt(int index) => this.Items.RemoveAt(index);
        public void Add(T item) => this.Items.Add(item);
        public void Clear() => this.Items.Clear();
        public bool Contains(T item) => this.Items.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => this.Items.CopyTo(array, arrayIndex);
        public bool Remove(T item) => this.Items.Remove(item);

        public int Count => this.Items.Count;
        public bool IsReadOnly => false;
        public bool IsFixedSize => false;
        public bool IsSynchronized => false;
        public object SyncRoot => this.SyncRoot;
        public T this[int index] { get => this.Items[index]; set => this.Items[index] = value; }

        public IEnumerator<T> GetEnumerator() => this.Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.Items.GetEnumerator();
    }
}

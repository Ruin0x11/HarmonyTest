using System;
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
        public T? SelectedItem { get => this.Choices[SelectedIndex]; }

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

        }

        protected virtual void OnChoose(int index)
        {
        }
    }
}

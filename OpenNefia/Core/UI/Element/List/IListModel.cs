using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Element.List
{
    public interface IListModel<T>
    {
        public int SelectedIndex { get; }
        public T? SelectedItem { get; }

        bool CanSelect(int index);
        void IncrementIndex(int delta);
        void Select(int index);
        bool CanActivate(int index);
        void Activate(int index);
    }
}

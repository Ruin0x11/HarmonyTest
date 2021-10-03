using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Element.List
{
    public interface IListModel<T> : ICollection<T>, IEnumerable<T>, IList<T>
    {
        public int SelectedIndex { get; }
        public T SelectedChoice { get; }

        bool CanSelect(int index);
        void IncrementIndex(int delta);
        void Select(int index);
        bool CanActivate(int index);
        void Activate(int index);
    }
}

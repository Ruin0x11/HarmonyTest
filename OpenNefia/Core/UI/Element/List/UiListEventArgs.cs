using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Element.List
{
    public class UiListEventArgs<T>
    {
        public int SelectedIndex { get; }
        public IUiListCell<T> SelectedChoice { get; }

        public UiListEventArgs(IUiListCell<T> choice, int index) {

            this.SelectedChoice = choice;
            this.SelectedIndex = index;
        }
    }
}

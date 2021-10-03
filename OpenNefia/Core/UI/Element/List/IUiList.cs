using OpenNefia.Core.Data.Types;
using OpenNefia.Core.UI.Layer;
using System;
using System.Collections.Generic;

namespace OpenNefia.Core.UI.Element.List
{
    public interface IUiList<T> : IList<IUiListCell<T>>
    {
        public bool HighlightSelected { get; set; }

        string GetChoiceText(T choice, int index);
        Keys GetChoiceKey(T choice, int index);
        IUiListCell<T> MakeChoiceCell(T choice, int index);

        event UiListEventHandler<T>? EventOnSelect;
        event UiListEventHandler<T>? EventOnActivate;

        public int SelectedIndex { get; }
        public IUiListCell<T> SelectedChoice { get; }

        bool CanSelect(int index);
        void IncrementIndex(int delta);
        void Select(int index);
        bool CanActivate(int index);
        void Activate(int index);
    }
}

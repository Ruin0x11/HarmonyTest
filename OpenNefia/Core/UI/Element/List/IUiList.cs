using OpenNefia.Core.Data.Types;
using OpenNefia.Core.UI.Layer;

namespace OpenNefia.Core.UI.Element.List
{
    public interface IUiList<T> : IListModel<IUiListCell<T>>
    {
        string GetChoiceText(T choice, int index);
        Keys GetChoiceKey(T choice, int index);
        IUiListCell<T> MakeChoiceCell(T choice, int index); 
    }
}

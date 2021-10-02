using OpenNefia.Core.Data.Types;
using OpenNefia.Core.UI.Layer;

namespace OpenNefia.Core.UI.Element.List
{
    public interface IUiList<T> : IListModel<T>
    {
        string GetChoiceText(int index);
        ColorAsset GetChoiceColor(int index);
        Keys GetChoiceKey(int index);
    }
}

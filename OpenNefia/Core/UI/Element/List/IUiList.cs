using OpenNefia.Core.Data.Types;

namespace OpenNefia.Core.UI.Element.List
{
    public interface IUiList<T> : IListModel<T>
    {
        ColorAsset GetItemColor(T item);
    }
}

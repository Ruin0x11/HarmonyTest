using OpenNefia.Core.UI.Element;

namespace OpenNefia.Core.UI
{
    public interface IUiLayerWithResult<T> : IUiLayer where T: class
    {
        IUiFocusManager FocusManager { get; }
        bool WasCancelled { get; }
        T? Result { get; }

        void Cancel();
        void Finish(T result);
        UiResult<T>? GetResult();

        UiResult<T>? Query();
        void OnQuery();
        bool IsQuerying();
    }
}

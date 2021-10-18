namespace OpenNefia.Core.UI.Layer
{
    public enum TitleScreenAction
    {
        ReturnToTitle,
        StartGame,
        Quit
    }

    public class TitleScreenResult 
    {
        public TitleScreenAction Action { get; }

        public TitleScreenResult(TitleScreenAction action)
        {
            this.Action = action;
        }
    }

    public interface ITitleScreenLayer : IUiLayerWithResult<TitleScreenResult>
    {
    }
}
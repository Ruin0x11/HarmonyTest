using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Love;
using OpenNefia.Core.UI.Element;
using OpenNefia.Game;

namespace OpenNefia.Core.UI
{
    public abstract class BaseUiLayer<T> : BaseInputUiElement, IUiLayer where T : struct
    {
        public IUiFocusManager FocusManager { get; } = new UiFocusManager();

        public abstract UiResult<T>? GetResult();

        public bool IsQuerying()
        {
            return GameWrapper.Instance.IsQuerying(this);
        }

        public void OnLoveKeyPressed(KeyConstant key, bool isRepeat)
        {
            this.FocusManager.FocusedElement?.ReceiveKeyPressed(key, isRepeat);
        }

        public void OnLoveKeyReleased(KeyConstant key)
        {
            this.FocusManager.FocusedElement?.ReceiveKeyReleased(key);
        }

        public void OnLoveTextInput(string text)
        {
            this.FocusManager.FocusedElement?.ReceieveTextInput(text);
        }

        public UiResult<T> Query()
        {
            GameWrapper.Instance.CurrentLayer?.HaltInput();

            GameWrapper.Instance.PushLayer(this);
            this.FocusManager.FocusedElement = this;

            UiResult<T>? result;

            while (true)
            {
                var dt = Timer.GetDelta();
                this.RunKeyActions(dt);
                GameWrapper.Instance.Update(dt);
                result = this.GetResult();
                if (result != null && result.Type != UiResult<T>.ResultType.Continuing)
                {
                    break;
                }

                GameWrapper.Instance.Draw();
                GameWrapper.Instance.SystemStep();
            }

            GameWrapper.Instance.PopLayer(this);
            this.HaltInput();

            return result;
        }
    }
}

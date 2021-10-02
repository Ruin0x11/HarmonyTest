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
        public abstract UiResult<T>? GetResult();

        protected IKeyHandler Keys = new KeyHandler();

        public UiResult<T> Query()
        {
            GameWrapper.Instance.CurrentLayer?.HaltInput();

            GameWrapper.Instance.PushLayer(this);

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

        protected void RunKeyActions(float dt)
        {
            this.Keys.RunKeyActions(dt);
        }

        public void HaltInput()
        {
            this.Keys.HaltInput();
        }

        public bool IsQuerying()
        {
            return GameWrapper.Instance.IsQuerying(this);
        }

        public void OnKeyPressed(KeyConstant key, bool is_repeat)
        {
            Keys.OnKeyPressed(key, is_repeat);
        }

        public void OnKeyReleased(KeyConstant key)
        {
            Keys.OnKeyReleased(key);
        }

        public void OnTextInput(string text)
        {
            Keys.OnTextInput(text);
        }
    }
}

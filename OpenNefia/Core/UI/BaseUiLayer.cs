using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Love;
using OpenNefia.Core.UI.Element;
using OpenNefia.Game;

namespace OpenNefia.Core.UI
{
    public abstract class BaseUiLayer<T> : BaseInputUiElement, IUiLayerWithResult<T> where T: class
    {
        public bool WasFinished { get => this.Result != null; }
        public bool WasCancelled { get; private set; }
        public T? Result { get; private set; }

        public virtual void Cancel()
        {
            this.WasCancelled = true;
        }

        public virtual void Finish(T result)
        {
            this.Result = result;
        }

        public virtual UiResult<T>? GetResult()
        {
            if (this.Result != null)
                return UiResult<T>.Finished(this.Result);
            if (this.WasCancelled)
                return UiResult<T>.Cancelled();

            return null;
        }

        public abstract void SetDefaultSize();

        public virtual void OnQuery() 
        {
        }

        public bool IsInActiveLayerList()
        {
            return Engine.Instance.IsInActiveLayerList(this);
        }

        public bool IsQuerying()
        {
            return Engine.Instance.IsQuerying(this);
        }

        public void OnLoveKeyPressed(KeyConstant key, bool isRepeat)
        {
            this.ReceiveKeyPressed(key, isRepeat);
        }

        public void OnLoveKeyReleased(KeyConstant key)
        {
            this.ReceiveKeyReleased(key);
        }

        public void OnLoveTextInput(string text)
        {
            this.ReceiveTextInput(text);
        }

        public void OnLoveMouseMoved(float x, float y, float dx, float dy, bool isTouch)
        {
            this.ReceiveMouseMoved(x, y, dx, dy, isTouch);
        }

        public void OnLoveMousePressed(float x, float y, int button, bool isTouch)
        {
            this.ReceiveMousePressed(x, y, button, isTouch);
        }

        public void OnLoveMouseReleased(float x, float y, int button, bool isTouch)
        {
            this.ReceiveMouseReleased(x, y, button, isTouch);
        }

        public virtual UiResult<T> Query()
        {
            Engine.Instance.CurrentLayer?.HaltInput();

            Engine.Instance.PushLayer(this);

            // Global REPL hotkey
            this.Keybinds[Keys.Backquote] += (_) =>
            {
                var repl = Current.Game.Repl.Value;
                if (!repl.IsInActiveLayerList())
                    repl.Query();
            };

            this.Result = null;
            this.WasCancelled = false;

            UiResult<T>? result;

            this.OnQuery();

            while (true)
            {
                var dt = Timer.GetDelta();
                this.RunKeyActions(dt);
                Engine.Instance.Update(dt);
                result = this.GetResult();
                if (result != null && result.Type != UiResult<T>.ResultType.Continuing)
                {
                    break;
                }

                Engine.Instance.Draw();
                Engine.Instance.SystemStep();
            }

            Engine.Instance.PopLayer(this);
            this.HaltInput();

            return result;
        }
    }
}

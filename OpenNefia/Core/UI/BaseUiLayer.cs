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

        public virtual bool IsQuerying()
        {
            return GameWrapper.Instance.IsQuerying(this);
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
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Love;
using OpenNefia.Core.Extensions;
using OpenNefia.Core.UI.Element;
using OpenNefia.Game;

namespace OpenNefia.Core.UI
{
    public abstract class BaseUiLayer<T> : BaseInputUiElement, IUiLayerWithResult<T> where T: class
    {
        public bool WasFinished { get => this.Result != null; }
        public bool WasCancelled { get; private set; }
        public T? Result { get; private set; }
        private bool IsLocalized = false;

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
                return new UiResult<T>.Finished(this.Result);
            if (this.WasCancelled)
                return new UiResult<T>.Cancelled();

            return null;
        }

        public override sealed void GetPreferredSize(out int width, out int height)
        {
            GetPreferredBounds(out var _, out var _, out width, out height);
        }

        public abstract void GetPreferredBounds(out int x, out int y, out int width, out int height);

        public virtual void OnQuery() 
        {
        }

        public virtual void OnQueryFinish()
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

            if (!IsLocalized)
            {
                this.Localize(this.GetType()!.FullName!);
                IsLocalized = true;
            }

            Engine.Instance.PushLayer(this);

            UiResult<T>? result;

            try
            {
                // Global REPL hotkey
                this.Keybinds[Keys.Backquote] += (_) =>
                {
                    var repl = Current.Game.Repl.Value;
                    if (!repl.IsInActiveLayerList())
                        repl.Query();
                };

                this.Result = null;
                this.WasCancelled = false;


                this.OnQuery();

                while (true)
                {
                    var dt = Timer.GetDelta();
                    this.RunKeyActions(dt);
                    Engine.Instance.Update(dt);
                    result = this.GetResult();
                    if (result != null)
                    {
                        break;
                    }

                    Engine.Instance.Draw();
                    Engine.Instance.SystemStep();
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error during {this.GetType().Name}.Query()", ex);
                result = new UiResult<T>.Error(ex);
            }
            finally
            {
                Engine.Instance.PopLayer(this);
            }

            this.HaltInput();
            this.OnQueryFinish();

            return result;
        }
    }
}

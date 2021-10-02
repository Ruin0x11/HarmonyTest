using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Love;
using OpenNefia.Game;

namespace OpenNefia.Core.UI
{
    public abstract class BaseUiLayer<T> : IUiLayer where T : struct
    {
        public abstract void Draw();
        public abstract void Update(float dt);
        public abstract UiResult<T>? GetResult();

        public UiResult<T> Query()
        {
            GameWrapper.Instance.PushLayer(this);

            UiResult<T>? result = null;

            while (true)
            {
                var dt = Timer.GetDelta();
                GameWrapper.Instance.Update();
                result = this.GetResult();
                if (result != null && result.Type != UiResult<T>.ResultType.Continuing)
                {
                    break;
                }

                GameWrapper.Instance.Draw();
            }

            GameWrapper.Instance.PopLayer(this);

            return result;
        }

        public bool IsQuerying()
        {
            return GameWrapper.Instance.IsQuerying(this);
        }
    }
}

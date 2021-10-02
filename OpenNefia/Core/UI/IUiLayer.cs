using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI
{
    public interface IUiLayer : IUiInput
    {
        public void Draw();
        public void Update(float dt);

        public void OnKeyPressed(Love.KeyConstant key, bool is_repeat);
        public void OnKeyReleased(Love.KeyConstant key);
        public void OnTextInput(string text);

        public void HaltInput();
    }
}

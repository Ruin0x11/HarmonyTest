using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI
{
    public interface IUiLayer
    {
        public void Draw();
        public void Update(float dt);
    }
}

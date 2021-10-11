using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI
{
    public class MouseButtonEvent : IInputEvent
    {
        public KeyPressState State { get; }
        public int X { get; }
        public int Y { get; }
        public bool Vetoed { get; private set; }

        public MouseButtonEvent(KeyPressState state, int x, int y)
        {
            this.State = state;
            this.X = x;
            this.Y = y;
            this.Vetoed = false;
        }

        public void Veto() { this.Vetoed = true; }
    }
}

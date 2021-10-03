using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI
{
    public class KeyInputEvent
    {
        public KeyPressState State { get; }
        public bool Vetoed { get; private set; }

        public KeyInputEvent(KeyPressState state)
        {
            this.State = state;
            this.Vetoed = false;
        }

        public void Veto() { this.Vetoed = true; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI
{
    /// <summary>
    /// Provides some convenient syntax for defining key forwards on UI classes that support them.
    /// </summary>
    public class KeyForwardsWrapper : IKeyForwarder
    {
        public IKeyInput Parent { get; }

        public KeyForwardsWrapper(IKeyInput parent)
        {
            this.Parent = parent;
        }

        public static KeyForwardsWrapper operator +(KeyForwardsWrapper forwardsWrapper, IKeyInput child)
        {
            forwardsWrapper.ForwardTo(child);
            return forwardsWrapper;
        }

        public static KeyForwardsWrapper operator -(KeyForwardsWrapper forwardsWrapper, IKeyInput child)
        {
            forwardsWrapper.UnforwardTo(child);
            return forwardsWrapper;
        }

        public void Clear() => this.ClearAllForwards();

        public void ForwardTo(IKeyInput child, int? priority = null)
        {
            this.Parent.ForwardTo(child, priority);
        }

        public void UnforwardTo(IKeyInput child)
        {
            this.Parent.UnforwardTo(child);
        }

        public void ClearAllForwards()
        {
            this.Parent.ClearAllForwards();
        }
    }
}

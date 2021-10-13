using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Event
{
    internal class OnCharaMovedEvent : IEvent<MapObject, OnCharaMovedEvent.Args, int>
    {
        public class Args
        {
            public int X = 0;
            public int Y = 0;
        }

        public int GetDefaultResult()
        {
            return 0;
        }
    }
}

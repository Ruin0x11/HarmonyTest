using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Event
{
    internal class OnCharaMovedHandler : IEventHandler<OnCharaMovedEvent, MapObject, OnCharaMovedEvent.Args, int>
    {
        public void Handle(MapObject receiver, OnCharaMovedEvent.Args args, IEventResult<int> result)
        {
            result.Result = 42;
            result.Veto();
        }
    }
}

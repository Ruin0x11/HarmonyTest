using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Event
{
    public interface IEventHandler<TEvent, TReciever, TArgs, TResult> where TEvent : IEvent<TReciever, TArgs, TResult>
    {
        void Handle(TReciever receiver, TArgs args, IEventResult<TResult> result);
    }
}

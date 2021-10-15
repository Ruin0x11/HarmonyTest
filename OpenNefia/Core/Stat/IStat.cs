using OpenNefia.Core.Data;
using OpenNefia.Core.Object;
using OpenNefia.Serial;
using System;

namespace OpenNefia.Core.Stat
{
    public interface IStat<T> : IRefreshable, IDataExposable
    {
    }
}
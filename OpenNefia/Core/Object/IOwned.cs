using System.Collections.Generic;

namespace OpenNefia.Core.Object
{
    public interface IOwned
    {
        public ILocation? CurrentLocation { get; }

        public IEnumerable<ILocation> EnumerateParents();
    }
}
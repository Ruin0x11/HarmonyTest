using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Object
{
    public interface IStackableObject<T> where T: IStackableObject<T>
    {
        public int Amount { get; }

        public bool CanStackWith(T other);
        public bool Separate(int amount, out T? stack);
        public bool StackWith(T other);
        public bool StackAll(bool showMessage = false);
        public bool MoveSome(int amount, ILocation where, int x, int y);
    }
}

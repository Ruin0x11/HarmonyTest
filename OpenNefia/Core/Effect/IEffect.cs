using OpenNefia.Core.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Effect
{
    public interface IEffect<TSource, TTarget> where TSource : MapObject where TTarget : MapObject
    {
        public EffectResult Apply(TSource source, TTarget target);
    }
}

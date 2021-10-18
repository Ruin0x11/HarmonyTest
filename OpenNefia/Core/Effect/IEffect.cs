using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Effect
{
    public interface IEffect : IDefDeserializable
    {
        public EffectResult Apply(EffectArguments args);
    }
}

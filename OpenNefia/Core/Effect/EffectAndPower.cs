using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Data.Types.DefOf;
using OpenNefia.Core.Object;
using OpenNefia.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OpenNefia.Core.Effect
{
    public class EffectAndPower : IDataExposable
    {
        public EffectDef EffectDef;

        public EffectArgumentsBase Args;

        public EffectAndPower(EffectDef effectDef, EffectArgumentsBase args)
        {
            EffectDef = effectDef;
            Args = args;
        }

        public EffectAndPower() : this(EffectDefOf.Default, new EffectArgumentsBase()) { }

        public EffectResult Apply(Chara chara, MapObject? source = null, TriggeredBy triggeredBy = TriggeredBy.Unknown)
        {
            return EffectDef.Apply(chara, Args.ToArgs(source: source, triggeredBy: triggeredBy));
        }

        public void Expose(DataExposer data)
        {
            data.ExposeDef(ref EffectDef, nameof(EffectDef));
            data.ExposeDeep(ref Args, nameof(Args));
        }
    }
}

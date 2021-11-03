using OpenNefia.Core.Data;
using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Data.Serial.CrossRefs;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OpenNefia.Core.Effect.Composite
{
    public class EffectNull : IEffect
    {
        public EffectResult Apply(Chara chara, EffectArguments args) => EffectResult.Succeeded;

        public void DeserializeDefField(IDefDeserializer deserializer, XElement element, Type containingModType)
        {
        }
    }
}

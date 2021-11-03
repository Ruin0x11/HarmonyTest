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
    public class EffectReference : IEffect
    {
        public EffectDef Def = null!;

        public EffectResult Apply(Chara chara, EffectArguments args) => Def.Effect.Apply(chara, args);

        public void DeserializeDefField(IDefDeserializer deserializer, XElement element, Type containingModType)
        {
            var defId = element.Value;
            var field = this.GetType().GetField(nameof(Def))!;
            var crossRef = new DefFieldCrossRef(new DefIdentifier(field.FieldType, defId), this, field);
            deserializer.AddCrossRef(crossRef);
        }
    }
}

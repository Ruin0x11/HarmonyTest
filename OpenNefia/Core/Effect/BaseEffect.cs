using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Object;
using System;
using System.Xml.Linq;

namespace OpenNefia.Core.Effect
{
    public abstract class BaseEffect : IEffect
    {
        public abstract EffectResult Apply(Chara chara, EffectArguments args);

        public void DeserializeDefField(IDefDeserializer deserializer, XElement element, Type containingModType)
        {
            deserializer.PopulateAllFields(element, this, containingModType);
        }
    }
}

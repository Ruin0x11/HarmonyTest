using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Object;
using OpenNefia.Serial;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace OpenNefia.Core.Effect
{
    public class EffectArgumentsBase : IDefDeserializable
    {
        public int Power = 1;

        public int TileRange = 1;

        public CurseState? CurseState = null;

        public EffectArguments ToArgs(
            MapObject? source = null,
            int? tileX = null,
            int? tileY = null,
            TriggeredBy triggeredBy = TriggeredBy.Unknown)
        {
            return new EffectArguments(source, tileX: tileX, tileY: tileY, curseState: CurseState ?? Effect.CurseState.Normal, triggeredBy: triggeredBy);
        }

        public void DeserializeDefField(IDefDeserializer deserializer, XElement element, Type containingModType)
        {
            deserializer.PopulateAllFields(element, this, containingModType);
        }
    }

    public class EffectArguments
    {
        public MapObject? Source;
        public int Power;
        public int TileRange;
        public CurseState CurseState;
        public TriggeredBy TriggeredBy;
        
        // TODO IEffectExtraData?
        public Dictionary<string, object> ExtraData;

        public EffectArguments(MapObject? source = null,
            int power = 0,
            int? tileX = null,
            int? tileY = null,
            int tileRange = 1,
            CurseState curseState = CurseState.Normal, 
            TriggeredBy triggeredBy = TriggeredBy.Unknown)
        {
            Source = source;
            Power = power;
            TileRange = tileRange;
            CurseState = curseState;
            TriggeredBy = triggeredBy;
            ExtraData = new Dictionary<string, object>();
        }
    }
}
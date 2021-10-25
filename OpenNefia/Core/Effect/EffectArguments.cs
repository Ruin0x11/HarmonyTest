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

        public EffectArguments ToArgs(MapObject target,
            MapObject? source = null,
            int? tileX = null,
            int? tileY = null,
            TriggeredBy triggeredBy = TriggeredBy.Unknown)
        {
            return new EffectArguments(target, source, tileX: tileX, tileY: tileY, curseState: CurseState ?? Effect.CurseState.Normal, triggeredBy: triggeredBy);
        }

        public void DeserializeDefField(IDefDeserializer deserializer, XElement element, Type containingModType)
        {
            deserializer.PopulateAllFields(element, this, containingModType);
        }
    }

    public class EffectArguments
    {
        public MapObject? Target;
        public MapObject? Source;
        public int TileX;
        public int TileY;
        public int Power;
        public int TileRange;
        public CurseState CurseState;
        public TriggeredBy TriggeredBy;
        
        // TODO IEffectExtraData?
        public Dictionary<string, object> ExtraData;

        public EffectArguments(MapObject target,
            MapObject? source = null,
            int power = 0,
            int? tileX = null,
            int? tileY = null,
            int tileRange = 1,
            CurseState curseState = CurseState.Normal, 
            TriggeredBy triggeredBy = TriggeredBy.Unknown)
        {
            Target = target;
            Source = source;
            Power = power;
            TileX = tileX.HasValue ? tileX.Value : target.X;
            TileY = tileY.HasValue ? tileY.Value : target.Y;
            TileRange = tileRange;
            CurseState = curseState;
            TriggeredBy = triggeredBy;
            ExtraData = new Dictionary<string, object>();
        }

        public EffectArguments(int tileX, 
            int tileY,
            MapObject? source = null,
            int power = 0,
            int tileRange = 1,
            CurseState curseState = CurseState.Normal,
            TriggeredBy triggeredBy = TriggeredBy.Unknown)
        {
            Target = null;
            Source = source;
            Power = power;
            TileX = tileX;
            TileY = tileY;
            TileRange = tileRange;
            CurseState = curseState;
            TriggeredBy = triggeredBy;
            ExtraData = new Dictionary<string, object>();
        }
    }
}
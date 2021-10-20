using OpenNefia.Core.Object;
using OpenNefia.Serial;
using System.Collections.Generic;

namespace OpenNefia.Core.Effect
{
    public class EffectArguments : IDataExposable
    {
        public Chara Target;
        public MapObject? Source;
        public int TileX;
        public int TileY;
        public int Power;
        public int TileRange;
        public CurseState CurseState;
        public TriggeredBy TriggeredBy;
        
        // TODO IEffectExtraData?
        public Dictionary<string, object> ExtraData;

        public EffectArguments(Chara target,
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

        public void Expose(DataExposer data)
        {
            data.ExposeWeak(ref Target!, nameof(Target));
            data.ExposeWeak(ref Source, nameof(Source));
            data.ExposeValue(ref TileX, nameof(TileX));
            data.ExposeValue(ref TileY, nameof(TileY));
            data.ExposeValue(ref Power, nameof(Power));
            data.ExposeValue(ref CurseState, nameof(CurseState));
            data.ExposeValue(ref TriggeredBy, nameof(TriggeredBy));
            data.ExposeCollection(ref ExtraData, nameof(ExtraData));
        }
    }
}
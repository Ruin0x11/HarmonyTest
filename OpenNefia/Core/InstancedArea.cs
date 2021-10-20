using OpenNefia.Serial;
using System.Collections.Generic;

namespace OpenNefia.Core.Data.Types
{
    public sealed class InstancedArea : IDataExposable
    {
        private class AreaFloor
        {
            public int Floor;
            public ulong MapUid;
        }

        public bool IsTrackedBySave { get => false; }

        private ulong _Uid;
        private AreaDef _Def;

        private Dictionary<int, AreaFloor> FloorNumberToAreaFloor = new Dictionary<int, AreaFloor>();

        public ulong Uid { get => _Uid; }
        public AreaDef Def { get => _Def; }

        public InstancedArea(AreaDef def)
        {
            _Uid = Current.Game.Uids.GetNextAndIncrement();
            _Def = def;
        }

        public void Expose(DataExposer data)
        {
            data.ExposeValue(ref _Uid, nameof(_Uid));
            data.ExposeDef(ref _Def, nameof(_Def));
        }
    }
}
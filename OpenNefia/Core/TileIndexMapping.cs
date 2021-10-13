using OpenNefia.Core.Data.Types;
using OpenNefia.Game.Serial;
using System.Collections.Generic;

namespace OpenNefia.Core
{
    internal class TileIndexMapping : IDataExposable
    {
        private int CurrentIndex = 0;
        internal Dictionary<int, string> IndexToTileDefId = new Dictionary<int, string>();
        internal Dictionary<string, int> TileDefIdToIndex = new Dictionary<string, int>();

        internal void Clear()
        {
            CurrentIndex = 0;
            IndexToTileDefId.Clear();
            TileDefIdToIndex.Clear();
        }

        internal void AddMapping(TileDef def)
        {
            var index = CurrentIndex;

            IndexToTileDefId[index] = def.Id;
            TileDefIdToIndex[def.Id] = index;

            CurrentIndex++;
        }

        public void Expose(DataExposer data)
        {
            data.ExposeValue(ref CurrentIndex, nameof(CurrentIndex));
            data.ExposeCollection(ref IndexToTileDefId, nameof(IndexToTileDefId));

            if (data.Stage == SerialStage.ResolvingRefs)
            {
                foreach (var (index, tileDefId) in this.IndexToTileDefId)
                {
                    TileDefIdToIndex.Add(tileDefId, index);
                }
            }
        }
    }
}

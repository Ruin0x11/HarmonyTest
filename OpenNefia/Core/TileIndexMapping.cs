using OpenNefia.Core.Data.Types;
using OpenNefia.Game.Serial;
using System.Collections.Generic;

namespace OpenNefia.Core
{
    internal class TileIndexMapping : IDataExposable
    {
        private int CurrentIndex = 0;
        internal Dictionary<int, TileDef> IndexToTileDef = new Dictionary<int, TileDef>();
        internal Dictionary<string, int> TileDefIdToIndex = new Dictionary<string, int>();

        internal void Clear()
        {
            CurrentIndex = 0;
            IndexToTileDef.Clear();
            TileDefIdToIndex.Clear();
        }

        internal void AddMapping(TileDef def)
        {
            var index = CurrentIndex;

            IndexToTileDef[index] = def;
            TileDefIdToIndex[def.Id] = index;

            CurrentIndex++;
        }

        public void Expose(DataExposer data)
        {
            data.ExposeValue(ref CurrentIndex, nameof(CurrentIndex));
            data.ExposeCollection(ref IndexToTileDef, nameof(IndexToTileDef), ExposeMode.Deep, ExposeMode.Reference);

            if (data.Stage == SerialStage.ResolvingRefs)
            {
                foreach (var (index, tileDef) in this.IndexToTileDef)
                {
                    TileDefIdToIndex.Add(tileDef.Id, index);
                }
            }
        }
    }
}

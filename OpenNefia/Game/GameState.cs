using OpenNefia.Core;
using OpenNefia.Core.Object;
using OpenNefia.Core.Rendering;
using OpenNefia.Game.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Game
{
    /// <summary>
    /// Everything that constitutes a full save.
    /// </summary>
    public class GameState : IDataExposable
    {
        public InstancedMap? CurrentMap;
        internal UidTracker UidTracker;
        internal TileIndexMapping TileIndexMapping;
        internal ICoords Coords;
        internal CharaObject? Player;

        public GameState()
        {
            CurrentMap = null;
            UidTracker = new UidTracker();
            TileIndexMapping = new TileIndexMapping();
            Coords = new OrthographicCoords();
        }

        public void Expose(DataExposer data)
        {
            data.ExposeDeep(ref TileIndexMapping, nameof(TileIndexMapping));
            data.ExposeDeep(ref UidTracker, nameof(UidTracker));
            data.ExposeDeep(ref CurrentMap, nameof(CurrentMap));
        }

        public static void Save(GameState state, string filepath)
        {
            var exposer = new DataExposer(filepath, SerialStage.Saving);
            exposer.ExposeDeep(ref state!, "Save");
            exposer.Save();
        }

        public static GameState Load(string filepath)
        {
            var state = new GameState();
            var exposer = new DataExposer(filepath, SerialStage.LoadingDeep);
            exposer.ExposeDeep(ref state, "Save");

            exposer.Stage = SerialStage.ResolvingRefs;
            exposer.ExposeDeep(ref state, "Save");

            return state!;
        }
    }
}

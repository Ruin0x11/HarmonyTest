using OpenNefia.Core;
using OpenNefia.Core.Object;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.UI.Layer.Repl;
using OpenNefia.Serial;
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
        internal InstancedMap? ActiveMap;
        internal UidTracker Uids;
        internal TileIndexMapping TileIndexMapping;
        internal ICoords Coords;
        internal Chara? Player;

        public Lazy<ReplLayer> Repl = new Lazy<ReplLayer>(() => new ReplLayer());

        public GameState()
        {
            ActiveMap = null;
            Uids = new UidTracker();
            TileIndexMapping = new TileIndexMapping();
            Coords = new OrthographicCoords();
        }

        public void Expose(DataExposer data)
        {
            data.ExposeDeep(ref TileIndexMapping, nameof(TileIndexMapping));
            data.ExposeDeep(ref Uids, nameof(Uids));
            data.ExposeDeep(ref ActiveMap, nameof(ActiveMap));
        }

        public static void Save(GameState state, string filepath)
        {
            SerializationUtils.Serialize(filepath, state, nameof(GameState));
        }

        public static GameState Load(string filepath)
        {
            return SerializationUtils.Deserialize(filepath, new GameState(), nameof(GameState));
        }
    }
}

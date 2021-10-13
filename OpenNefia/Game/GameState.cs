using OpenNefia.Core;
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
    public class GameState
    {
        public InstancedMap? CurrentMap;
        internal UidTracker UidTracker;

        public GameState()
        {
            CurrentMap = null;
            UidTracker = new UidTracker();
        }

        public void Save(string filepath)
        {

        }

        public void Load(string filepath)
        {

        }
    }
}

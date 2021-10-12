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
    internal class GameState
    {
        public InstancedMap? CurrentMap;

        public GameState()
        {
            CurrentMap = null;
        }

        public void Save(string filepath)
        {

        }

        public void Load(string filepath)
        {

        }
    }
}

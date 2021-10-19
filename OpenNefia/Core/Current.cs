using OpenNefia.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core
{
    public static class Current
    {
        public static GameState Game { get; internal set; } = new GameState();
        public static InstancedMap? Map { get => Game.CurrentMap; }
    }
}

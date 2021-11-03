using OpenNefia.Core.Object;
using OpenNefia.Core.UI.Hud;
using OpenNefia.Core.UI.Layer;
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
        public static GameState Game { get; internal set; } = null!;
        public static InstancedMap? Map { get => Game.ActiveMap; }
        public static FieldLayer? Field { get => FieldLayer.Instance; }

        public static Chara? Player
        {
            get => Game.Player;
            internal set => Game.Player = value;
        }

        internal static void InitStaticGlobals()
        {
            Game = new GameState();
        }
    }
}

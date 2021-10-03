using OpenNefia.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data.Types
{
    public record Keybind : IDataType, IKeybind
    {
        public string Id { get; private set; }

        public Keybind(string id) { this.Id = id; }

        public bool IsShiftDelayed { get; private set; } = false;

        public static class Entries
        {
            public static Keybind Enter = new Keybind($"Base.{nameof(Enter)}");
            public static Keybind Cancel = new Keybind($"Base.{nameof(Cancel)}");
            public static Keybind Quit = new Keybind($"Base.{nameof(Quit)}");
            public static Keybind Escape = new Keybind($"Base.{nameof(Escape)}");

            public static Keybind UIUp = new Keybind($"Base.{nameof(UIUp)}") { IsShiftDelayed = true };
            public static Keybind UIDown = new Keybind($"Base.{nameof(UIDown)}") { IsShiftDelayed = true };
            public static Keybind UILeft = new Keybind($"Base.{nameof(UILeft)}") { IsShiftDelayed = true };
            public static Keybind UIRight = new Keybind($"Base.{nameof(UIRight)}") { IsShiftDelayed = true };

            public static Keybind North = new Keybind($"Base.{nameof(North)}") { IsShiftDelayed = true };
            public static Keybind South = new Keybind($"Base.{nameof(South)}") { IsShiftDelayed = true };
            public static Keybind West = new Keybind($"Base.{nameof(West)}") { IsShiftDelayed = true };
            public static Keybind East = new Keybind($"Base.{nameof(East)}") { IsShiftDelayed = true };
            public static Keybind Northwest = new Keybind($"Base.{nameof(Northwest)}") { IsShiftDelayed = true };
            public static Keybind Northeast = new Keybind($"Base.{nameof(Northeast)}") { IsShiftDelayed = true };
            public static Keybind Southwest = new Keybind($"Base.{nameof(Southwest)}") { IsShiftDelayed = true };
            public static Keybind Southeast = new Keybind($"Base.{nameof(Southeast)}") { IsShiftDelayed = true };

            public static Keybind Wait = new Keybind($"Base.{nameof(Wait)}");
            public static Keybind Identify = new Keybind($"Base.{nameof(Identify)}");
            public static Keybind Mode = new Keybind($"Base.{nameof(Mode)}");
            public static Keybind Mode2 = new Keybind($"Base.{nameof(Mode2)}");

            public static Keybind Repl = new Keybind($"Base.{nameof(Repl)}");

            public static Keybind SelectionA = new Keybind($"Base.{nameof(SelectionA)}");
            public static Keybind SelectionB = new Keybind($"Base.{nameof(SelectionB)}");
            public static Keybind SelectionC = new Keybind($"Base.{nameof(SelectionC)}");
            public static Keybind SelectionD = new Keybind($"Base.{nameof(SelectionD)}");
            public static Keybind SelectionE = new Keybind($"Base.{nameof(SelectionE)}");
            public static Keybind SelectionF = new Keybind($"Base.{nameof(SelectionF)}");
            public static Keybind SelectionG = new Keybind($"Base.{nameof(SelectionG)}");
            public static Keybind SelectionH = new Keybind($"Base.{nameof(SelectionH)}");
            public static Keybind SelectionI = new Keybind($"Base.{nameof(SelectionI)}");
            public static Keybind SelectionJ = new Keybind($"Base.{nameof(SelectionJ)}");
            public static Keybind SelectionK = new Keybind($"Base.{nameof(SelectionK)}");
            public static Keybind SelectionL = new Keybind($"Base.{nameof(SelectionL)}");
            public static Keybind SelectionM = new Keybind($"Base.{nameof(SelectionM)}");
            public static Keybind SelectionN = new Keybind($"Base.{nameof(SelectionN)}");
            public static Keybind SelectionO = new Keybind($"Base.{nameof(SelectionO)}");
            public static Keybind SelectionP = new Keybind($"Base.{nameof(SelectionP)}");
            public static Keybind SelectionQ = new Keybind($"Base.{nameof(SelectionQ)}");
            public static Keybind SelectionR = new Keybind($"Base.{nameof(SelectionR)}");

            public static Dictionary<Keys, Keybind> SelectionKeys = new Dictionary<Keys, Keybind>()
            {
                { Keys.A, SelectionA },
                { Keys.B, SelectionB },
                { Keys.C, SelectionC },
                { Keys.D, SelectionD },
                { Keys.E, SelectionE },
                { Keys.F, SelectionF },
                { Keys.G, SelectionG },
                { Keys.H, SelectionH },
                { Keys.I, SelectionI },
                { Keys.J, SelectionJ },
                { Keys.K, SelectionK },
                { Keys.L, SelectionL },
                { Keys.M, SelectionM },
                { Keys.N, SelectionN },
                { Keys.O, SelectionO },
                { Keys.P, SelectionP },
                { Keys.Q, SelectionQ },
                { Keys.R, SelectionR },
            };
        }
    }
}

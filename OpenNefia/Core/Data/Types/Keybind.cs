using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data.Types
{
    public class Keybind : IDataType
    {
        public string Id { get; private set; }

        public Keybind(string id) { this.Id = id; }

        public static class Entries
        {
            public static Keybind Enter = new Keybind($"Base.{nameof(Enter)}");
            public static Keybind Cancel = new Keybind($"Base.{nameof(Cancel)}");
            public static Keybind Quit = new Keybind($"Base.{nameof(Quit)}");
            public static Keybind Escape = new Keybind($"Base.{nameof(Escape)}");

            public static Keybind UIUp = new Keybind($"Base.{nameof(UIUp)}");
            public static Keybind UIDown = new Keybind($"Base.{nameof(UIDown)}");
            public static Keybind UILeft = new Keybind($"Base.{nameof(UILeft)}");
            public static Keybind UIRight = new Keybind($"Base.{nameof(UIRight)}");

            public static Keybind North = new Keybind($"Base.{nameof(North)}");
            public static Keybind South = new Keybind($"Base.{nameof(South)}");
            public static Keybind West = new Keybind($"Base.{nameof(West)}");
            public static Keybind East = new Keybind($"Base.{nameof(East)}");
            public static Keybind Northwest = new Keybind($"Base.{nameof(Northwest)}");
            public static Keybind Northeast = new Keybind($"Base.{nameof(Northeast)}");
            public static Keybind Southwest = new Keybind($"Base.{nameof(Southwest)}");
            public static Keybind Southeast = new Keybind($"Base.{nameof(Southeast)}");

            public static Keybind Wait = new Keybind($"Base.{nameof(Wait)}");
            public static Keybind Identify = new Keybind($"Base.{nameof(Identify)}");
            public static Keybind Mode = new Keybind($"Base.{nameof(Mode)}");
            public static Keybind Mode2 = new Keybind($"Base.{nameof(Mode2)}");

            public static Keybind Repl = new Keybind($"Base.{nameof(Repl)}");
        }
    }
}

using OpenNefia.Core.Data.Types;
using System;

namespace OpenNefia.Core
{
    public static class Messages
    {
        public static void Print(string queryText, ColorDef? color = null)
        {
            if (color == null)
                color = ColorDefOf.MesWhite;

            Current.Field?.Hud.MessageWindow.Print(queryText, color);
        }
    }
}
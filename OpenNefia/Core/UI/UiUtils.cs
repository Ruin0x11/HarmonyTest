using Love;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI
{
    public static class UiUtils
    {
        public static string GetKeyName(Keys keyAndModifiers)
        {
            return Enum.GetName(typeof(Keys), keyAndModifiers)!.ToLowerInvariant();
        }

        public static Rectangle GetCenteredParams(int width, int height)
        {
            var ingame = false;
            var x = (Love.Graphics.GetWidth() - width) / 2;
            var y = 0;
            if (ingame)
            {
                var tiledHeight = Love.Graphics.GetHeight() / Constants.TILE_SIZE;
                y = ((tiledHeight - 2) * Constants.TILE_SIZE - height) / 2 + 8;
            }
            else
            {
                y = (Love.Graphics.GetHeight() - height) / 2;
            }

            return new Rectangle(x, y, width, height);
        }
    }
}

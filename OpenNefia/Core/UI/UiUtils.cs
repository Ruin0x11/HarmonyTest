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
    }
}

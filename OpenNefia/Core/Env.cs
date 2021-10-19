using System;
using System.Reflection;

namespace OpenNefia.Core
{
    public static class Env
    {
        public static Version Version { 
            get => Assembly.GetExecutingAssembly().GetName().Version!;
        }
    }
}

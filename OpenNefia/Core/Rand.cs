using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core
{
    public static class Rand
    {
        private static System.Random Instance = new System.Random();

        public static int NextInt(int n)
        {
            return Instance.Next(n);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core
{
    public class Logger
    {
        public static Logger Instance = new Logger();

        public static void Info(string s)
        {
            Console.WriteLine(s);
        }
    }
}

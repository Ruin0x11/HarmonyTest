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

        public static void Error(string s, Exception? ex = null)
        {
            Console.WriteLine(s);
            if (ex != null)
            {
                Console.WriteLine(ex);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ordo.Utils
{
    internal static class Func
    {
        public static void Print(bool shouldPrint, string level, string message)
        {
            if (shouldPrint) {
                Console.WriteLine($"[{level.ToUpper()}] {message}");
            }
        }
    }
}

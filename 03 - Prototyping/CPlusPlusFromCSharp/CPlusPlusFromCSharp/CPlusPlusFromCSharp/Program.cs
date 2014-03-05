using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CPlusPlusFromCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            writePersonName();
            Console.ReadLine();
        }

        [DllImport ("CPlusPlusGeneral")]
        private static extern void writePersonName();
    }
}

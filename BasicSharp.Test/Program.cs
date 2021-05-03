using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string code = 
            "FOR i = 1 TO 100\r\n" +
            "PRINTL \"Hello World!\"\r\n" +
            "SLEEP 500\r\n" +
            "NEXT i\r\n";
            Interpreter basic = new Interpreter(code);
            try
            {
                basic.Exec();
            }
            catch (Exception e)
            {
                Console.WriteLine("BAD");
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("OK");
            Console.Read();
        }
    }
}

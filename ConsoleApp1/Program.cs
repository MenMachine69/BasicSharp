using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hellow World!");
            Console.WriteLine("Wie lautet Dein Name?");
            string name = Console.ReadLine();
            Console.WriteLine("Hallo " + name + "!");

            Console.ReadKey();
        }
    }
}

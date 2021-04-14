using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace BasicConsole
{
    public class TextModeConsole : IConsole
    {
        public void Write(string str) { Console.Write(str); }

        public void WriteLine(string str) { Console.WriteLine(str); }

        public string ReadLine() { return Console.ReadLine(); }

        public int Read(int millisec)
        {
            int readed = -1;

            Stopwatch watch = new Stopwatch();
            watch.Start();

            do
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    readed = (int)key.Key;
                }

                Thread.Sleep(1);

            } while ((millisec == 0 && readed == -1) || (readed == -1 && millisec > 0 && watch.ElapsedMilliseconds < millisec));

            watch.Stop();

            return readed;
        }

        public void Clear() { Console.Clear(); }

        public void SetPos(int row, int col) { Console.SetCursorPosition(col + 1, row + 1); }

        public void Sleep(int millisec) { Thread.Sleep(millisec); }
    }
}

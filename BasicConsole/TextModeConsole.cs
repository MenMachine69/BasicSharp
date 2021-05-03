using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace BasicConsole
{
    public class TextModeConsole : IConsole
    {
        public void SoundSelect(int patch, int channel) { }

        public void SoundPlay(int note, int channel, int millisec, int velocity, int duration) { }

        public void SoundStop() { }

        public void SoundVolume(int volume) { }

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

        public void Cls(int layer) { Console.Clear(); }

        public void SetPos(int row, int col) { Console.SetCursorPosition(col + 1, row + 1); }

        public void Sleep(int millisec) { Thread.Sleep(millisec); }

        #region unsupported commands
        public void LoadSprite(string file, string name) { }

        public void DrawSprite(string name, int x, int y) { }

        public void DrawSprite(string name, int x, int y, int width, int height) { }

        public void RemoveSprite(string name, int x, int y) { }
        
        public void SetCanvas(int canvas) { }

        public void SetColor(int forecolor, int backcolor) { }

        public void DrawElipse(int left, int top, int width, int height, int color, int thick, int rotation) { }

        public void DrawRect(int left, int top, int width, int height, int color, int thick, int rotation) { }

        public void DrawLine(int xStart, int yStart, int xEnd, int yEnd, int color, int thick) { }

        public void FillElipse(int left, int top, int width, int height, int color, int rotation) { }

        public void FillRect(int left, int top, int width, int height, int color, int rotation) { }

        public void SetPixel(int x, int y, int color) { }

        #endregion
    }
}

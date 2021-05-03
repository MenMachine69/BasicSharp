using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BasicConsole
{
    public interface IConsole
    {
        void SoundSelect(int patch, int channel);
        void SoundPlay(int note, int channel, int millisec, int velocity, int duration);
        void SoundStop();
        void SoundVolume(int volume);
        void Write(string str);
        void WriteLine(string str);
        string ReadLine();
        int Read(int millisec);
        void Cls(int layer);
        void SetPos(int row, int col);
        void Sleep(int millisec);

        void LoadSprite(string file, string name);

        void DrawSprite(string name, int x, int y);

        void DrawSprite(string name, int x, int y, int width, int height);

        void RemoveSprite(string name, int x, int y);

        void SetCanvas(int canvas);

        void SetColor(int forecolor, int backcolor);

        void DrawElipse(int left, int top, int width, int height, int color, int thick, int rotation);

        void DrawRect(int left, int top, int width, int height, int color, int thick, int rotation);

        void DrawLine(int xStart, int yStart, int xEnd, int yEnd, int color, int thick);

        void FillElipse(int left, int top, int width, int height, int color, int rotation);

        void FillRect(int left, int top, int width, int height, int color, int rotation);

        void SetPixel(int x, int y, int color);
    }
}

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
        void Write(string str);
        void WriteLine(string str);
        string ReadLine();
        int Read(int millisec);
        void Clear();
        void SetPos(int row, int col);
        void Sleep(int millisec);
    }
}

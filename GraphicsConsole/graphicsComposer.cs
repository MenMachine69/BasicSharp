using BasicConsole;
using BasicSharp;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphicsConsole
{
    /// <summary>
    /// internal representation of the Console content to support background painting etc. 
    /// </summary>
    internal class composer : IConsole
    {
        static object _lock = new object();
        
        ReaderWriterLockSlim thisLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        Interpreter _interpreter = null;
        Task _interpreterTask = null;
        CancellationTokenSource _cts = new CancellationTokenSource();
        CancellationToken _token;

        Tuple<byte, Tuple<Color, Color>>[,] _textBuffer = new Tuple<byte, Tuple<Color, Color>>[40, 80]; // internal buffer for text layer
        Bitmap[] _pufferGraphics = new Bitmap[4];    // 4 separete graphics layers
        Dictionary<string, Image> _sprites = new Dictionary<string, Image>();
        Bitmap _pufferText;        // Text output Buffer 
        Bitmap _pufferBackground;  // Background Bitmap
        Bitmap _output;
        int _currLayer = 0;
        CharTable _font;
        int[] _currPos = new int[] { 1, 1 };
        bool _composing = false;

        Color _foreColor = Color.PowderBlue;
        Color _backColor = Color.Blue;
        SolidBrush _foreBrush;
        SolidBrush _backBrush;

        MidiOut _midi;
               


        bool _needrefresh;

        public composer()
        {
            _font = new CharTable();

            Reset();
        }

        public event EventHandler Started;
        public event EventHandler Stopped;
        public bool Running { get; private set; }

        public Bitmap Image
        {
            get { return _output; }
        }

        #region graphics output
        public void ClearCanvas(int layer)
        {
            _waitUntilComposed();

            if (layer >= 1 && layer <= 4)
            {
                using (Graphics g = Graphics.FromImage(_pufferGraphics[layer - 1]))
                    g.Clear(Color.Transparent);

                _needrefresh = true;
            }
        }

        public void LoadSprite(Image img, string name)
        {
            if (_sprites.ContainsKey(name))
                _sprites[name] = img;
            else
                _sprites.Add(name, img);
        }

        public void LoadSprite(string filename, string name)
        {
            if (File.Exists(filename))
            {
                Image img = Tools.ImageFromFile(filename);
                if (img != null)
                    LoadSprite(img, name);
            }
        }

        public void DrawSprite(string name, int x, int y)
        {
            DrawSprite(name, x, y, 0, 0);
        }

        public void DrawSprite(string name, int x, int y, int width, int heigth)
        {
            if (_sprites.ContainsKey(name))
            {
                _waitUntilComposed();

                using (Graphics g = Graphics.FromImage(_pufferGraphics[_currLayer]))
                {
                    if (width < 1 || heigth < 1)
                        g.DrawImageUnscaled(_sprites[name], new Point(x, y));
                    else
                        g.DrawImage(_sprites[name], new Rectangle(new Point(x, y), new Size(width, heigth)));
                }

                _needrefresh = true;
            }
        }

        public void RemoveSprite(string name, int x, int y)
        {
            if (_sprites.ContainsKey(name))
            {
                _waitUntilComposed();

                using (SolidBrush brush = new SolidBrush(Color.Transparent))
                {
                    using (Graphics g = Graphics.FromImage(_pufferGraphics[_currLayer]))
                        g.FillRectangle(brush, new Rectangle(x, y, _sprites[name].Width, _sprites[name].Height));

                    _needrefresh = true;
                }
            }
        }

        public void DrawGraphicElement(eGrahicsElement element, Color color, int x1, int y1)
        {
            DrawGraphicElement(element, color, x1, y1, 0, 0, 0, 0, 0, 0);
        }

        public void DrawGraphicElement(eGrahicsElement element, Color color, int x1, int y1, int x2, int y2)
        {
            DrawGraphicElement(element, color, x1, y1, x2, y2, 0, 0, 0, 0);
        }

        public void DrawGraphicElement(eGrahicsElement element, Color color, int x1, int y1, int x2, int y2, int x3)
        {
            DrawGraphicElement(element, color, x1, y1, x2, y2, x3, 0, 0, 0);
        }

        public void DrawGraphicElement(eGrahicsElement element, Color color, int x1, int y1, int x2, int y2, int x3, int y3)
        {
            DrawGraphicElement(element, color, x1, y1, x2, y2, x3, y3, 0, 0);
        }

        public void DrawGraphicElement(eGrahicsElement element, Color color, int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
        {
            _waitUntilComposed();

            using (Pen pen = new Pen(color))
            {
                using (SolidBrush brush = new SolidBrush(color))
                {
                    using (Graphics g = Graphics.FromImage(_pufferGraphics[_currLayer]))
                    {
                        switch (element)
                        {
                            case eGrahicsElement.Line:
                                g.DrawLine(pen, new Point(x1, y1), new Point(x2, y2));
                                break;
                            case eGrahicsElement.Ellipse:
                                g.DrawEllipse(pen, new Rectangle(new Point(x1, y1), new Size(x2, y2)));
                                break;
                            case eGrahicsElement.Rectangle:
                                g.DrawRectangle(pen, new Rectangle(new Point(x1, y1), new Size(x2, y2)));
                                break;
                            case eGrahicsElement.Pie:
                                g.DrawPie(pen, new Rectangle(new Point(x1, y1), new Size(x2, y2)), x3, y3);
                                break;
                            case eGrahicsElement.Arc:
                                g.DrawArc(pen, new Rectangle(new Point(x1, y1), new Size(x2, y2)), x3, y3);
                                break;
                            case eGrahicsElement.Bezier:
                                g.DrawBezier(pen, new Point(x1, y1), new Point(x2, y2), new Point(x3, y3), new Point(x4, y4));
                                break;
                            case eGrahicsElement.Point:
                                g.FillRectangle(brush, new Rectangle(x1, y1, 1, 1));
                                break;
                            case eGrahicsElement.FillEllipse:
                                g.FillEllipse(brush, new Rectangle(new Point(x1, y1), new Size(x2, y2)));
                                break;
                            case eGrahicsElement.FillPie:
                                g.FillPie(brush, new Rectangle(new Point(x1, y1), new Size(x2, y2)), x3, y3);
                                break;
                            case eGrahicsElement.FillRectangle:
                                g.FillRectangle(brush, new Rectangle(new Point(x1, y1), new Size(x2, y2)));
                                break;
                        }
                    }
                }
            }
            _needrefresh = true;
        }
        #endregion

        #region text output
        public void ClearText()
        {
            _waitUntilComposed();

            _initTextBuffer();
            _currPos[0] = 1;
            _currPos[1] = 1;

            using (Graphics g = Graphics.FromImage(_pufferText))
                g.Clear(Color.Transparent);

            _needrefresh = true;
        }

        public void Locate(int row, int col)
        {
            _currPos[0] = row;
            _currPos[1] = col;
        }

        public void Write(string towrite)
        {
            Write(towrite, Color.Empty, Color.Empty);
        }

        public void Write(byte ascii_code)
        {
            Write(ascii_code, Color.Empty, Color.Empty);

        }

        public void Write(byte ascii_code, Color fore, Color back)
        {
            Write(ascii_code, fore, back, true);
        }

        public void Write(byte ascii_code, Color fore, Color back, bool moveCur)
        {
            _waitUntilComposed();

            if (ascii_code == 13) // LF
            {
                _currPos[0]++;

                if (_currPos[0] > 40)
                {
                    _scrollTextUp();
                    _currPos[0] = 40;
                }
            }
            else if (ascii_code == 10) // CR
            {
                _currPos[1] = 1;
            }
            else if (ascii_code == 9) // TAB
            {
                int newpos = ((_currPos[1] / 5) + 1) * 5;

                if (newpos <= 80) // wenn es rechts drüber rausgeht wird es ignoriert...
                    _currPos[1] = newpos;
            }
            else
            {
                // Zeichen setzen und dann den Cursor weiterschieben
                _textBuffer[_currPos[0] - 1, _currPos[1] - 1] = new Tuple<byte, Tuple<Color, Color>>(ascii_code, new Tuple<Color, Color>((fore == Color.Empty ? _foreColor : fore), (back == Color.Empty ? Color.Transparent : back)));

                var charTuple = _textBuffer[_currPos[0] - 1, _currPos[1] - 1];

                // Text in das Bitmap übertragen...
                using (Graphics g = Graphics.FromImage(_pufferText))
                {
                    int xPos = (_currPos[1] - 1) * 8;
                    int yPos = (_currPos[0] - 1) * 16;

                    if (charTuple.Item2.Item2 != Color.Transparent)
                    {
                        using (SolidBrush brush = new SolidBrush(charTuple.Item2.Item2))
                            g.FillRectangle(brush, new Rectangle(new Point(xPos, yPos), new Size(8, 16)));
                    }
                    else
                    {
                        using (SolidBrush brush = new SolidBrush(_backColor))
                            g.FillRectangle(brush, new Rectangle(new Point(xPos, yPos), new Size(8, 16)));
                    }

                    if (charTuple.Item1 != ' ')
                    {
                        if (charTuple.Item2.Item1 != _foreColor)
                        {
                            using (SolidBrush brush = new SolidBrush(charTuple.Item2.Item1))
                                _font.GetAsciiChar(charTuple.Item1).Draw(brush, g, new Point(xPos, yPos), 1, 1);
                        }
                        else
                            _font.GetAsciiChar(charTuple.Item1).Draw(_foreBrush, g, new Point(xPos, yPos), 1, 1);
                    }

                }

                if (moveCur)
                    _currPos[1]++;
            }


            if (_currPos[1] > 80)
            {
                // Zeilenschaltung...
                _currPos[0]++;
                _currPos[1] = 1;

                if (_currPos[0] > 40)
                {
                    _scrollTextUp();
                    _currPos[0] = 40;
                }
            }


            _needrefresh = true;
        }

        public void Write(string towrite, Color fore, Color back)
        {
            _waitUntilComposed();

            foreach (char c in towrite)
            {
                if (c == '\n') // LF
                {
                    _currPos[0]++;

                    if (_currPos[0] > 40)
                    {
                        _scrollTextUp();
                        _currPos[0] = 40;
                    }
                }
                else if (c == '\r') // CR
                {
                    _currPos[1] = 1;
                }
                else if (c == '\t') // TAB
                {
                    int newpos = ((_currPos[1] / 5) + 1) * 5;

                    if (newpos <= 80) // wenn es rechts drüber rausgeht wird es ignoriert...
                        _currPos[1] = newpos;
                }
                else
                {
                    // Zeichen setzen und dann den Cursor weiterschieben
                    _textBuffer[_currPos[0] - 1, _currPos[1] - 1] = new Tuple<byte, Tuple<Color, Color>>(_font.GetCharCode(c), new Tuple<Color, Color>((fore == Color.Empty ? _foreColor : fore), (back == Color.Empty ? Color.Transparent : back)));

                    var charTuple = _textBuffer[_currPos[0] - 1, _currPos[1] - 1];

                    // Text in das Bitmap übertragen...
                    using (Graphics g = Graphics.FromImage(_pufferText))
                    {
                        int xPos = (_currPos[1] - 1) * 8;
                        int yPos = (_currPos[0] - 1) * 16;

                        if (charTuple.Item2.Item2 != Color.Transparent)
                        {
                            using (SolidBrush brush = new SolidBrush(charTuple.Item2.Item2))
                                g.FillRectangle(brush, new Rectangle(new Point(xPos, yPos), new Size(8, 16)));
                        }
                        if (charTuple.Item1 != ' ')
                        {
                            if (charTuple.Item2.Item1 != _foreColor)
                            {
                                using (SolidBrush brush = new SolidBrush(charTuple.Item2.Item1))
                                    _font.GetAsciiChar(charTuple.Item1).Draw(brush, g, new Point(xPos, yPos), 1, 1);
                            }
                            else
                                _font.GetAsciiChar(charTuple.Item1).Draw(_foreBrush, g, new Point(xPos, yPos), 1, 1);
                        }

                    }

                    _currPos[1]++;
                }


                if (_currPos[1] > 80)
                {
                    // Zeilenschaltung...
                    _currPos[0]++;
                    _currPos[1] = 1;

                    if (_currPos[0] > 40)
                    {
                        _scrollTextUp();
                        _currPos[0] = 40;
                    }
                }
            }

            _needrefresh = true;
        }

        void _scrollTextUp()
        {
            // Text nach oben scrollen...
            for (int row = 1; row < 40; ++row)
                for (int col = 0; col < 80; ++col)
                    _textBuffer[row - 1, col] = _textBuffer[row, col];

            for (int col = 0; col < 80; ++col)
                _textBuffer[39, col] = new Tuple<byte, Tuple<Color, Color>>(32, new Tuple<Color, Color>(_foreColor, Color.Transparent));

            // _pufferText nach oben verschieben...
            using (Bitmap temp = _pufferText.Clone(new Rectangle(0, 16, 640, 624), _pufferText.PixelFormat))
            {
                using (Graphics g = Graphics.FromImage(_pufferText))
                {
                    g.Clear(Color.Transparent);
                    g.DrawImageUnscaled(temp, new Point(0, 0));
                }
            }
        }

        void _initTextBuffer()
        {
            for (int row = 0; row < 40; ++row)
                for (int col = 0; col < 80; ++col)
                    _textBuffer[row, col] = new Tuple<byte, Tuple<Color, Color>>(32, new Tuple<Color, Color>(_foreColor, Color.Transparent));
        }

        public void Back()
        {
            _currPos[1]--;

            if (_currPos[1] < 1)
            {
                _currPos[1] = 80;
                _currPos[0]--;
            }

            Write(32, Color.Empty, Color.Empty, false);
        }
        #endregion

        public void SetBackColor(Color color)
        {
            _backColor = color;

            _backBrush?.Dispose();

            _backBrush = new SolidBrush(_backColor);

            _needrefresh = true;
        }

        public void SetForeColor(Color color)
        {
            _foreColor = color;
            _foreBrush?.Dispose();

            _foreBrush = new SolidBrush(_foreColor);

            _needrefresh = true;
        }

        public bool Cancel { get; set; }

        public Interpreter Interpreter { get => _interpreter; set { _interpreter = value; _interpreter.Console = this; } }

        public void Run()
        {
            while (Cancel == false)
            {
                _waitUntilComposed();

                System.Threading.Thread.Sleep(1);

                if (_needrefresh)
                    _composeOutput();

                _waitUntilComposed();

                Application.DoEvents();

                _needrefresh = false;

                if (_interpreterTask != null)
                {
                    if (_interpreterTask.IsCompleted || _interpreterTask.IsFaulted || _interpreterTask.IsCanceled)
                    {
                        _interpreterTask.Dispose();
                        _interpreterTask = null;
                        _cts = new CancellationTokenSource();
                        Running = false;
                        Stopped.BeginInvoke(null, EventArgs.Empty, null, null);
                    }
                }
            }
        }

        public void Stop()
        {

            if (_token != null)
                _cts.Cancel();
        }

        public void Start()
        {
            Reset();

            _token = _cts.Token;
            _interpreter.CancelationToken = _token;
            _interpreterTask = Task.Factory.StartNew(_interpreter.Exec, _token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            Running = true;
            Started.BeginInvoke(null, EventArgs.Empty, null, null);
        }

        public void Reset()
        {
            _waitUntilComposed();

            // init sound...
            if (_midi == null)
            {
                // find gs wavetable device
                for (int device = 0; device < MidiOut.NumberOfDevices; device++)
                {
                    if (MidiOut.DeviceInfo(device).ProductName.ToLower().Contains("gs wavetable"))
                        _midi = new MidiOut(device);
                }
            }
            else
                _midi.Reset();

            if (_midi != null)
                _midi.Send(MidiMessage.ChangePatch(1, 1).RawData);  // set grand piano as poatch for channel 1


            _initTextBuffer();
            SetBackColor(Color.Blue);
            SetForeColor(Color.PowderBlue);

            if (_pufferText == null)
                _pufferText = new Bitmap(640, 640);

            _pufferGraphics[0] = new Bitmap(640, 640);
            _pufferGraphics[1] = new Bitmap(640, 640);
            _pufferGraphics[2] = new Bitmap(640, 640);
            _pufferGraphics[3] = new Bitmap(640, 640);

            if (_output == null)
                _output = new Bitmap(650, 650);

            if (_pufferBackground != null)
                _pufferBackground.Dispose();

            _pufferBackground = null;

            _sprites.Clear();

            WriteLine("BASICSHARP 1.0");
            WriteLine("READY");

            _composeOutput();

            _waitUntilComposed();
        }

        void _composeOutput()
        {
            _composing = true;
            lock (_lock)
            {
                this.thisLock.EnterReadLock();
                try
                {
                    using (Graphics canvas = Graphics.FromImage(_output))
                    {
                        if (_pufferBackground != null)
                            canvas.DrawImageUnscaled(_pufferBackground, new Point(4, 4));
                        else
                            canvas.Clear(_backColor);

                        for (int i = 0; i <= 3; ++i)
                            canvas.DrawImageUnscaled(_pufferGraphics[i], new Point(4, 4));

                        canvas.DrawImageUnscaled(_pufferText, new Point(4, 4));
                    }
                }
                finally
                {
                    this.thisLock.ExitReadLock();
                }
            }
            _composing = false;
        }

        void _waitUntilComposed()
        {
            while (_composing)
                System.Threading.Thread.Sleep(1);
        }

        public GraphicsConsoleControl Host { get; set; }

        #region IConsole implementation
        public void WriteLine(string str)
        {
            if (_currPos[1] > 1)
                Write("\r\n" + str + "\r\n");
            else
                Write(str + "\r\n");
        }

        public string ReadLine()
        {
            _waitUntilComposed();

            return Host.ReadLine(_inputFeedback);
        }

        internal void _inputFeedback(string key)
        {
            Write(key);
        }

        public int Read(int millisec)
        {
            _waitUntilComposed();

            return Host.ReadKey(millisec);
        }

        public void Cls(int layer)
        {
            if (layer == 0)
                ClearText();
            else 
                ClearCanvas(layer);
        }

        public void SetPos(int row, int col)
        {
            Locate(row, col);
        }

        public void Sleep(int millisec)
        {
            Thread.Sleep(millisec);
        }

        public void SetCanvas(int layer)
        {
            if (layer < 0 || layer > 3)
                throw new ArgumentException("Layer must be between 0 and 3 (4 layers available).");

            _currLayer = layer;
        }
        
        public void SetColor(int forecolor, int backcolor)
        {
            SetForeColor(Color.FromArgb(forecolor));
            SetBackColor(Color.FromArgb(backcolor));
        }
        
        public void DrawElipse(int left, int top, int width, int height, int color, int thick, int rotation) 
        {
            DrawGraphicElement(eGrahicsElement.Ellipse, Color.FromArgb(color), left, top, width, height, thick, rotation);
        }

        public void DrawRect(int left, int top, int width, int height, int color, int thick, int rotation) 
        {
            DrawGraphicElement(eGrahicsElement.Rectangle, Color.FromArgb(color), left, top, width, height, thick, rotation);
        }

        public void DrawLine(int xStart, int yStart, int xEnd, int yEnd, int color, int thick)
        {
            DrawGraphicElement(eGrahicsElement.Line, Color.FromArgb(color), xStart, yStart, xEnd, yEnd, thick);
        }

        public void FillElipse(int left, int top, int width, int height, int color, int rotation) { }

        public void FillRect(int left, int top, int width, int height, int color, int rotation) { }

        public void SetPixel(int x, int y, int color) { }

        public void SoundSelect(int patch, int channel) 
        {
            if (_midi != null)
                _midi.Send(MidiMessage.ChangePatch(patch, channel).RawData);
        }

        public void SoundPlay(int note, int channel, int millisec, int velocity, int duration)
        {
            if (_midi != null)
                _midi.Send(new NoteOnEvent(millisec, channel, note, velocity, duration).GetAsShortMessage());
        }

        public void SoundStop()
        {
            if (_midi != null)
                _midi.Reset();
        }

        public void SoundVolume(int volume)
        {
            if (_midi != null)
                _midi.Volume = volume;
        }

        #endregion;

    }
}

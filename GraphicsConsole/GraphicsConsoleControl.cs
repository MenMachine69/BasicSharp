using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace GraphicsConsole
{
    public static class Tools
    {
        static bool _designModeDetected = false;
        static bool _designMode = false;

        /// <summary>
        /// Detects if the control is currently used in WinForms-Designer in Visual Studio (devenv)
        /// </summary>
        public static bool IsDesignMode
        {
            get
            {
                if (!_designModeDetected)
                    _designMode = (Process.GetCurrentProcess().ProcessName == "devenv");

                _designModeDetected = true;

                return _designMode;
            }
        }
    }

    /// <summary>
    /// A control that works like System.Console but supports graphics in multiple ways.
    /// </summary>
    [ToolboxItem(true)]
    public class GraphicsConsoleControl : Control
    {
        graphicsComposer _composer;
        SolidBrush _borderBrush;
        Color _borderColor;
        Task _composerTask;
        System.Timers.Timer _refreshScreenTimer;


        public GraphicsConsoleControl()
        {
            if (Tools.IsDesignMode == false)
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                SetStyle(ControlStyles.ResizeRedraw, true);
                SetStyle(ControlStyles.UserPaint, true);

                SetBorderColor(Color.LightSkyBlue);

                _composer = new graphicsComposer();
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (Tools.IsDesignMode == false)
            {
                _composerTask = Task.Factory.StartNew(_composer.Run, TaskCreationOptions.LongRunning);
                ////_composerTask.Start();

                _refreshScreenTimer = new System.Timers.Timer(1000 / 60); // 60 Frames pro Sekunde
                _refreshScreenTimer.Elapsed += _paintScreen;
            }
        }

        void _paintScreen(object sender, ElapsedEventArgs e)
        {
            if (InvokeRequired)
                Invoke(new Action(() => { Refresh(); }));
        }

        public void DrawLine(int xStart, int yStart, int xEnd, int yEnd, Color color)
        {
            _composer.DrawLine(xStart, yStart, xEnd, yEnd, color);
        }

        public void DrawRectangle(int xStart, int yStart, int width, int height, Color color)
        {
            _composer.DrawRectangle(xStart, yStart, width, height, color);
        }

        public void FillRectangle(int xStart, int yStart, int width, int height, Color color)
        {
            _composer.FillRectangle(xStart, yStart, width, height, color);
        }


        public void SetCursorPos(int row, int col)
        {
            _composer.SetCursorPos(row, col);
        }

        public void Write(string text)
        {
            _composer.Write(text);
        }

        public void Write(string text, Color foreColor, Color backColor)
        {
            _composer.Write(text, foreColor, backColor);
        }

        public void Write(byte ascii_code)
        {
            _composer.Write(ascii_code);
        }

        public void Write(byte ascii_code, Color foreColor, Color backColor)
        {
            _composer.Write(ascii_code, foreColor, backColor);
        }

        public void ClearText()
        {
            _composer.ClearText();
        }

        public void ClearCanvas()
        {
            _composer.ClearCanvas();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Tools.IsDesignMode == false)
                _refreshScreenTimer.Stop();
            
            base.OnPaint(e);

            if (Tools.IsDesignMode == false)
            {
                e.Graphics.Clear(_borderColor);

                if (Width < 640 || Height < 640)
                {
                    e.Graphics.Clear(Color.White);
                    e.Graphics.DrawString($"Area to small. Please Resize to display content. (Min: 640*640 Curr: {Width}*{Height})", SystemFonts.DefaultFont, SystemBrushes.ControlText, 10, 10);
                }
                else
                    e.Graphics.DrawImageUnscaled(_composer.Image, new Point((Width - 640) / 2, (Height - 640) / 2));

                _refreshScreenTimer.Start();
            }
        }

        public void SetBorderColor(Color color)
        {
            _borderColor = color;
            _borderBrush?.Dispose();

            _borderBrush = new SolidBrush(_borderColor);
        }
    }

    /// <summary>
    /// internal representation of the Console content to support background painting etc. 
    /// </summary>
    internal class graphicsComposer
    {
        Tuple<byte, Tuple<Color, Color>>[,] _textBuffer = new Tuple<byte, Tuple<Color, Color>>[40, 80]; // internal buffer for text layer
        Bitmap _pufferGraphics;    // Graphics output buffer
        Bitmap _pufferText;        // Text output Buffer 
        Bitmap _pufferBackground;  // Background Bitmap
        Bitmap _output;
        CharTable _font;
        int[] _currPos = new int[] { 1, 1 };
        bool _composing = false;

        Color _foreColor;
        Color _backColor;
        SolidBrush _foreBrush;
        SolidBrush _backBrush;


        bool _needrefresh;

        public graphicsComposer()
        {
            _font = new CharTable();

            Reset();
        }

        public Bitmap Image 
        {
            get { return _output; }
        }

        void _initTextBuffer()
        {
            for (int row = 0; row < 40; ++row)
                for (int col = 0; col < 80; ++col)
                    _textBuffer[row, col] = new Tuple<byte, Tuple<Color, Color>>(32, new Tuple<Color, Color>(Color.PowderBlue, Color.Transparent));
        }

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

        public void ClearCanvas()
        {
            _waitUntilComposed();

            using (Graphics g = Graphics.FromImage(_pufferGraphics))
                g.Clear(Color.Transparent);

            _needrefresh = true;
        }

        public void DrawLine(int xStart, int yStart, int xEnd, int yEnd, Color color)
        {
            _waitUntilComposed();

            using (Pen pen = new Pen(color))
            {
                using (Graphics g = Graphics.FromImage(_pufferGraphics))
                    g.DrawLine(pen, xStart - 1, yStart - 1, xEnd - 1, yEnd - 1);
            }
            _needrefresh = true;

        }

        public void DrawRectangle(int xStart, int yStart, int width, int height, Color color)
        {
            _waitUntilComposed();

            using (Pen pen = new Pen(color))
            {
                using (Graphics g = Graphics.FromImage(_pufferGraphics))
                    g.DrawRectangle(pen, new Rectangle(xStart - 1, yStart - 1, width, height));
            }
            _needrefresh = true;

        }

        public void FillRectangle(int xStart, int yStart, int width, int height, Color color)
        {
            _waitUntilComposed();

            using (Brush brush = new SolidBrush(color))
            {
                using (Graphics g = Graphics.FromImage(_pufferGraphics))
                    g.FillRectangle(brush, new Rectangle(xStart - 1, yStart - 1, width, height));
            }

            _needrefresh = true;
        }


        public void SetCursorPos(int row, int col)
        {
            _currPos[0] = row;
            _currPos[1] = col;
        }

        public void Write(string towrite)
        {
            Write(towrite, Color.Empty, Color.Empty);

        }

        void _waitUntilComposed()
        {
            while (_composing)
                System.Threading.Thread.Sleep(1);
        }

        public void Write(byte ascii_code)
        {
            Write(ascii_code, Color.Empty, Color.Empty);

        }

        public void Write(byte ascii_code, Color fore, Color back)
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
                _textBuffer[_currPos[0] - 1, _currPos[1] - 1] = new Tuple<byte, Tuple<Color, Color>>(ascii_code, new Tuple<Color, Color>((fore == Color.Empty ? Color.PowderBlue : fore), (back == Color.Empty ? Color.Transparent : back)));

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
                    _textBuffer[_currPos[0] - 1, _currPos[1] - 1] = new Tuple<byte, Tuple<Color, Color>>(_font.GetCharCode(c), new Tuple<Color, Color>((fore == Color.Empty ? Color.PowderBlue : fore), (back == Color.Empty ? Color.Transparent : back)));

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
                _textBuffer[39, col] = new Tuple<byte, Tuple<Color, Color>>(32, new Tuple<Color, Color>(Color.PowderBlue, Color.Transparent));

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

        public void Reset()
        {
            _initTextBuffer();
            SetBackColor(Color.Blue);
            SetForeColor(Color.PowderBlue);

            if (_pufferText == null)
                _pufferText = new Bitmap(640, 640);

            if (_pufferGraphics == null)
                _pufferGraphics = new Bitmap(640, 640);

            if (_output == null)
                _output = new Bitmap(650, 650);

            if (_pufferBackground != null)
                _pufferBackground.Dispose();

            _pufferBackground = null;

            _composeOutput();
        }

        void _composeOutput()
        {
            _composing = true;
            lock (_output)
            {
                using (Graphics canvas = Graphics.FromImage(_output))
                {
                    if (_pufferBackground != null)
                        canvas.DrawImageUnscaled(_pufferBackground, new Point(4, 4));
                    else
                        canvas.Clear(_backColor);

                    canvas.DrawImageUnscaled(_pufferGraphics, new Point(4, 4));
                    canvas.DrawImageUnscaled(_pufferText, new Point(4, 4));
                }
            }
            _composing = false;

        }

        public bool Cancel { get; set; }


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

        public void Run()
        {
            while (Cancel == false)
            {
                System.Threading.Thread.Sleep(1);

                if (_needrefresh)
                    _composeOutput();

                _needrefresh = false;
            }
        }
    }
}

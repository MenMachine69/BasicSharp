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

        public void SetPos(int row, int col)
        {
            _composer.SetPos(row, col);
        }

        public void Write(string text)
        {
            _composer.Write(text);
        }

        public void Write(string text, Color foreColor, Color backColor)
        {
            _composer.Write(text, foreColor, backColor);
        }

        public void ClearText()
        {
            _composer.ClearText();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Tools.IsDesignMode == false)
                _refreshScreenTimer.Stop();
            
            base.OnPaint(e);

            if (Tools.IsDesignMode == false)
            {
                e.Graphics.Clear(_borderColor);

                if (Width < 730 || Height < 530)
                {
                    e.Graphics.Clear(Color.White);
                    e.Graphics.DrawString($"Area to small. Please Resize to display content. (Min: 730*530 Curr: {Width}*{Height})", SystemFonts.DefaultFont, SystemBrushes.ControlText, 10, 10);
                }
                else
                    e.Graphics.DrawImageUnscaled(_composer.Image, new Point((Width - 730) / 2, (Height - 530) / 2));

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
        Tuple<char, Tuple<Color, Color>>[,] _textBuffer = new Tuple<char, Tuple<Color, Color>>[40, 80]; // internal buffer for text layer
        Bitmap _pufferGraphics;    // Graphics output buffer
        Bitmap _pufferText;        // Text output Buffer 
        Bitmap _pufferBackground;  // Background Bitmap
        Bitmap _output;
        consoleFont _font;
        int[] _currPos = new int[] { 1, 1 };
        bool _composing = false;

        Color _foreColor;
        Color _backColor;
        SolidBrush _foreBrush;
        SolidBrush _backBrush;

        bool _needrefresh;

        public graphicsComposer()
        {
            _font = new consoleFont();

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
                    _textBuffer[row, col] = new Tuple<char, Tuple<Color, Color>>(' ', new Tuple<Color, Color>(Color.PowderBlue, Color.Transparent));
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

        public void SetPos(int row, int col)
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
                    _textBuffer[_currPos[0] - 1, _currPos[1] - 1] = new Tuple<char, Tuple<Color, Color>>(c, new Tuple<Color, Color>((fore == Color.Empty ? Color.PowderBlue : fore), (back == Color.Empty ? Color.Transparent : back)));

                    var charTuple = _textBuffer[_currPos[0] - 1, _currPos[1] - 1];

                    // Text in das Bitmap übertragen...
                    using (Graphics g = Graphics.FromImage(_pufferText))
                    {
                        int xPos = (_currPos[1] - 1) * 9;
                        int yPos = (_currPos[0] - 1) * 13;

                        if (charTuple.Item2.Item2 != Color.Transparent)
                        {
                            using (SolidBrush brush = new SolidBrush(charTuple.Item2.Item2))
                                g.FillRectangle(brush, new Rectangle(new Point(xPos, yPos), new Size(9, 13)));
                        }
                        if (charTuple.Item1 != ' ')
                        {
                            if (charTuple.Item2.Item1 != _foreColor)
                            {
                                using (SolidBrush brush = new SolidBrush(charTuple.Item2.Item1))
                                    _font.GetChar(charTuple.Item1).Draw(brush, g, new Point(xPos, yPos), 1, 1);
                            }
                            else
                                _font.GetChar(charTuple.Item1).Draw(_foreBrush, g, new Point(xPos, yPos), 1, 1);
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
                _textBuffer[39, col] = new Tuple<char, Tuple<Color, Color>>(' ', new Tuple<Color, Color>(Color.PowderBlue, Color.Transparent));

            // _pufferText nach oben verschieben...
            using (Bitmap temp = _pufferText.Clone(new Rectangle(0, 13, 720, 507), _pufferText.PixelFormat))
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
                _pufferText = new Bitmap(720, 520);

            if (_pufferGraphics == null)
                _pufferGraphics = new Bitmap(720, 520);

            if (_output == null)
                _output = new Bitmap(730, 530);

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

    internal class consoleFont
    {
        consoleChar[] _chars = new consoleChar[128];
        Encoding _ascii = Encoding.ASCII;

        public consoleFont()
        {
            _initChars();
        }

        public consoleChar GetChar(char c)
        {
            byte ascii_code = _ascii.GetBytes(new char[] { c })[0];
            if (ascii_code >= 32 && ascii_code <= 126)
                return _chars[ascii_code - 32];

            return _chars[0];
        }

        public byte GetAsciiCode(char c)
        {
            return _ascii.GetBytes(new char[] { c })[0];
        }

        void _initChars()
        {
            _chars[0] = new consoleChar(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });// space :32
            _chars[1] = new consoleChar(new byte[] { 0x00, 0x00, 0x18, 0x18, 0x00, 0x00, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18 });// ! :33
            _chars[2] = new consoleChar(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x36, 0x36, 0x36, 0x36 });
            _chars[3] = new consoleChar(new byte[] { 0x00, 0x00, 0x00, 0x66, 0x66, 0xff, 0x66, 0x66, 0xff, 0x66, 0x66, 0x00, 0x00 });
            _chars[4] = new consoleChar(new byte[] { 0x00, 0x00, 0x18, 0x7e, 0xff, 0x1b, 0x1f, 0x7e, 0xf8, 0xd8, 0xff, 0x7e, 0x18 });
            _chars[5] = new consoleChar(new byte[] { 0x00, 0x00, 0x0e, 0x1b, 0xdb, 0x6e, 0x30, 0x18, 0x0c, 0x76, 0xdb, 0xd8, 0x70 });
            _chars[6] = new consoleChar(new byte[] { 0x00, 0x00, 0x7f, 0xc6, 0xcf, 0xd8, 0x70, 0x70, 0xd8, 0xcc, 0xcc, 0x6c, 0x38 });
            _chars[7] = new consoleChar(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x18, 0x1c, 0x0c, 0x0e });
            _chars[8] = new consoleChar(new byte[] { 0x00, 0x00, 0x0c, 0x18, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x18, 0x0c });
            _chars[9] = new consoleChar(new byte[] { 0x00, 0x00, 0x30, 0x18, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0x18, 0x30 });
            _chars[10] = new consoleChar(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x99, 0x5a, 0x3c, 0xff, 0x3c, 0x5a, 0x99, 0x00, 0x00 });
            _chars[11] = new consoleChar(new byte[] { 0x00, 0x00, 0x00, 0x18, 0x18, 0x18, 0xff, 0xff, 0x18, 0x18, 0x18, 0x00, 0x00 });
            _chars[12] = new consoleChar(new byte[] { 0x00, 0x00, 0x30, 0x18, 0x1c, 0x1c, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            _chars[13] = new consoleChar(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0x00 });
            _chars[14] = new consoleChar(new byte[] { 0x00, 0x00, 0x00, 0x38, 0x38, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            _chars[15] = new consoleChar(new byte[] { 0x00, 0x60, 0x60, 0x30, 0x30, 0x18, 0x18, 0x0c, 0x0c, 0x06, 0x06, 0x03, 0x03 });
            _chars[16] = new consoleChar(new byte[] { 0x00, 0x00, 0x3c, 0x66, 0xc3, 0xe3, 0xf3, 0xdb, 0xcf, 0xc7, 0xc3, 0x66, 0x3c });
            _chars[17] = new consoleChar(new byte[] { 0x00, 0x00, 0x7e, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x78, 0x38, 0x18 });
            _chars[18] = new consoleChar(new byte[] { 0x00, 0x00, 0xff, 0xc0, 0xc0, 0x60, 0x30, 0x18, 0x0c, 0x06, 0x03, 0xe7, 0x7e });
            _chars[19] = new consoleChar(new byte[] { 0x00, 0x00, 0x7e, 0xe7, 0x03, 0x03, 0x07, 0x7e, 0x07, 0x03, 0x03, 0xe7, 0x7e });
            _chars[20] = new consoleChar(new byte[] { 0x00, 0x00, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0xff, 0xcc, 0x6c, 0x3c, 0x1c, 0x0c });
            _chars[21] = new consoleChar(new byte[] { 0x00, 0x00, 0x7e, 0xe7, 0x03, 0x03, 0x07, 0xfe, 0xc0, 0xc0, 0xc0, 0xc0, 0xff });
            _chars[22] = new consoleChar(new byte[] { 0x00, 0x00, 0x7e, 0xe7, 0xc3, 0xc3, 0xc7, 0xfe, 0xc0, 0xc0, 0xc0, 0xe7, 0x7e });
            _chars[23] = new consoleChar(new byte[] { 0x00, 0x00, 0x30, 0x30, 0x30, 0x30, 0x18, 0x0c, 0x06, 0x03, 0x03, 0x03, 0xff });
            _chars[24] = new consoleChar(new byte[] { 0x00, 0x00, 0x7e, 0xe7, 0xc3, 0xc3, 0xe7, 0x7e, 0xe7, 0xc3, 0xc3, 0xe7, 0x7e });
            _chars[25] = new consoleChar(new byte[] { 0x00, 0x00, 0x7e, 0xe7, 0x03, 0x03, 0x03, 0x7f, 0xe7, 0xc3, 0xc3, 0xe7, 0x7e });
            _chars[26] = new consoleChar(new byte[] { 0x00, 0x00, 0x00, 0x38, 0x38, 0x00, 0x00, 0x38, 0x38, 0x00, 0x00, 0x00, 0x00 });
            _chars[27] = new consoleChar(new byte[] { 0x00, 0x00, 0x30, 0x18, 0x1c, 0x1c, 0x00, 0x00, 0x1c, 0x1c, 0x00, 0x00, 0x00 });
            _chars[28] = new consoleChar(new byte[] { 0x00, 0x00, 0x06, 0x0c, 0x18, 0x30, 0x60, 0xc0, 0x60, 0x30, 0x18, 0x0c, 0x06 });
            _chars[29] = new consoleChar(new byte[] { 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0x00, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00 });
            _chars[30] = new consoleChar(new byte[] { 0x00, 0x00, 0x60, 0x30, 0x18, 0x0c, 0x06, 0x03, 0x06, 0x0c, 0x18, 0x30, 0x60 });
            _chars[31] = new consoleChar(new byte[] { 0x00, 0x00, 0x18, 0x00, 0x00, 0x18, 0x18, 0x0c, 0x06, 0x03, 0xc3, 0xc3, 0x7e });
            _chars[32] = new consoleChar(new byte[] { 0x00, 0x00, 0x3f, 0x60, 0xcf, 0xdb, 0xd3, 0xdd, 0xc3, 0x7e, 0x00, 0x00, 0x00 });
            _chars[33] = new consoleChar(new byte[] { 0x00, 0x00, 0xc3, 0xc3, 0xc3, 0xc3, 0xff, 0xc3, 0xc3, 0xc3, 0x66, 0x3c, 0x18 });
            _chars[34] = new consoleChar(new byte[] { 0x00, 0x00, 0xfe, 0xc7, 0xc3, 0xc3, 0xc7, 0xfe, 0xc7, 0xc3, 0xc3, 0xc7, 0xfe });
            _chars[35] = new consoleChar(new byte[] { 0x00, 0x00, 0x7e, 0xe7, 0xc0, 0xc0, 0xc0, 0xc0, 0xc0, 0xc0, 0xc0, 0xe7, 0x7e });
            _chars[36] = new consoleChar(new byte[] { 0x00, 0x00, 0xfc, 0xce, 0xc7, 0xc3, 0xc3, 0xc3, 0xc3, 0xc3, 0xc7, 0xce, 0xfc });
            _chars[37] = new consoleChar(new byte[] { 0x00, 0x00, 0xff, 0xc0, 0xc0, 0xc0, 0xc0, 0xfc, 0xc0, 0xc0, 0xc0, 0xc0, 0xff });
            _chars[38] = new consoleChar(new byte[] { 0x00, 0x00, 0xc0, 0xc0, 0xc0, 0xc0, 0xc0, 0xc0, 0xfc, 0xc0, 0xc0, 0xc0, 0xff });
            _chars[39] = new consoleChar(new byte[] { 0x00, 0x00, 0x7e, 0xe7, 0xc3, 0xc3, 0xcf, 0xc0, 0xc0, 0xc0, 0xc0, 0xe7, 0x7e });
            _chars[40] = new consoleChar(new byte[] { 0x00, 0x00, 0xc3, 0xc3, 0xc3, 0xc3, 0xc3, 0xff, 0xc3, 0xc3, 0xc3, 0xc3, 0xc3 });
            _chars[41] = new consoleChar(new byte[] { 0x00, 0x00, 0x7e, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x7e });
            _chars[42] = new consoleChar(new byte[] { 0x00, 0x00, 0x7c, 0xee, 0xc6, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06 });
            _chars[43] = new consoleChar(new byte[] { 0x00, 0x00, 0xc3, 0xc6, 0xcc, 0xd8, 0xf0, 0xe0, 0xf0, 0xd8, 0xcc, 0xc6, 0xc3 });
            _chars[44] = new consoleChar(new byte[] { 0x00, 0x00, 0xff, 0xc0, 0xc0, 0xc0, 0xc0, 0xc0, 0xc0, 0xc0, 0xc0, 0xc0, 0xc0 });
            _chars[45] = new consoleChar(new byte[] { 0x00, 0x00, 0xc3, 0xc3, 0xc3, 0xc3, 0xc3, 0xc3, 0xdb, 0xff, 0xff, 0xe7, 0xc3 });
            _chars[46] = new consoleChar(new byte[] { 0x00, 0x00, 0xc7, 0xc7, 0xcf, 0xcf, 0xdf, 0xdb, 0xfb, 0xf3, 0xf3, 0xe3, 0xe3 });
            _chars[47] = new consoleChar(new byte[] { 0x00, 0x00, 0x7e, 0xe7, 0xc3, 0xc3, 0xc3, 0xc3, 0xc3, 0xc3, 0xc3, 0xe7, 0x7e });
            _chars[48] = new consoleChar(new byte[] { 0x00, 0x00, 0xc0, 0xc0, 0xc0, 0xc0, 0xc0, 0xfe, 0xc7, 0xc3, 0xc3, 0xc7, 0xfe });
            _chars[49] = new consoleChar(new byte[] { 0x00, 0x00, 0x3f, 0x6e, 0xdf, 0xdb, 0xc3, 0xc3, 0xc3, 0xc3, 0xc3, 0x66, 0x3c });
            _chars[50] = new consoleChar(new byte[] { 0x00, 0x00, 0xc3, 0xc6, 0xcc, 0xd8, 0xf0, 0xfe, 0xc7, 0xc3, 0xc3, 0xc7, 0xfe });
            _chars[51] = new consoleChar(new byte[] { 0x00, 0x00, 0x7e, 0xe7, 0x03, 0x03, 0x07, 0x7e, 0xe0, 0xc0, 0xc0, 0xe7, 0x7e });
            _chars[52] = new consoleChar(new byte[] { 0x00, 0x00, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0xff });
            _chars[53] = new consoleChar(new byte[] { 0x00, 0x00, 0x7e, 0xe7, 0xc3, 0xc3, 0xc3, 0xc3, 0xc3, 0xc3, 0xc3, 0xc3, 0xc3 });
            _chars[54] = new consoleChar(new byte[] { 0x00, 0x00, 0x18, 0x3c, 0x3c, 0x66, 0x66, 0xc3, 0xc3, 0xc3, 0xc3, 0xc3, 0xc3 });
            _chars[55] = new consoleChar(new byte[] { 0x00, 0x00, 0xc3, 0xe7, 0xff, 0xff, 0xdb, 0xdb, 0xc3, 0xc3, 0xc3, 0xc3, 0xc3 });
            _chars[56] = new consoleChar(new byte[] { 0x00, 0x00, 0xc3, 0x66, 0x66, 0x3c, 0x3c, 0x18, 0x3c, 0x3c, 0x66, 0x66, 0xc3 });
            _chars[57] = new consoleChar(new byte[] { 0x00, 0x00, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x3c, 0x3c, 0x66, 0x66, 0xc3 });
            _chars[58] = new consoleChar(new byte[] { 0x00, 0x00, 0xff, 0xc0, 0xc0, 0x60, 0x30, 0x7e, 0x0c, 0x06, 0x03, 0x03, 0xff });
            _chars[59] = new consoleChar(new byte[] { 0x00, 0x00, 0x3c, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x3c });
            _chars[60] = new consoleChar(new byte[] { 0x00, 0x03, 0x03, 0x06, 0x06, 0x0c, 0x0c, 0x18, 0x18, 0x30, 0x30, 0x60, 0x60 });
            _chars[61] = new consoleChar(new byte[] { 0x00, 0x00, 0x3c, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0x3c });
            _chars[62] = new consoleChar(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xc3, 0x66, 0x3c, 0x18 });
            _chars[63] = new consoleChar(new byte[] { 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            _chars[64] = new consoleChar(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x18, 0x38, 0x30, 0x70 });
            _chars[65] = new consoleChar(new byte[] { 0x00, 0x00, 0x7f, 0xc3, 0xc3, 0x7f, 0x03, 0xc3, 0x7e, 0x00, 0x00, 0x00, 0x00 });
            _chars[66] = new consoleChar(new byte[] { 0x00, 0x00, 0xfe, 0xc3, 0xc3, 0xc3, 0xc3, 0xfe, 0xc0, 0xc0, 0xc0, 0xc0, 0xc0 });
            _chars[67] = new consoleChar(new byte[] { 0x00, 0x00, 0x7e, 0xc3, 0xc0, 0xc0, 0xc0, 0xc3, 0x7e, 0x00, 0x00, 0x00, 0x00 });
            _chars[68] = new consoleChar(new byte[] { 0x00, 0x00, 0x7f, 0xc3, 0xc3, 0xc3, 0xc3, 0x7f, 0x03, 0x03, 0x03, 0x03, 0x03 });
            _chars[69] = new consoleChar(new byte[] { 0x00, 0x00, 0x7f, 0xc0, 0xc0, 0xfe, 0xc3, 0xc3, 0x7e, 0x00, 0x00, 0x00, 0x00 });
            _chars[70] = new consoleChar(new byte[] { 0x00, 0x00, 0x30, 0x30, 0x30, 0x30, 0x30, 0xfc, 0x30, 0x30, 0x30, 0x33, 0x1e });
            _chars[71] = new consoleChar(new byte[] { 0x7e, 0xc3, 0x03, 0x03, 0x7f, 0xc3, 0xc3, 0xc3, 0x7e, 0x00, 0x00, 0x00, 0x00 });
            _chars[72] = new consoleChar(new byte[] { 0x00, 0x00, 0xc3, 0xc3, 0xc3, 0xc3, 0xc3, 0xc3, 0xfe, 0xc0, 0xc0, 0xc0, 0xc0 });
            _chars[73] = new consoleChar(new byte[] { 0x00, 0x00, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x00, 0x00, 0x18, 0x00 });
            _chars[74] = new consoleChar(new byte[] { 0x38, 0x6c, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0x00, 0x00, 0x0c, 0x00 });
            _chars[75] = new consoleChar(new byte[] { 0x00, 0x00, 0xc6, 0xcc, 0xf8, 0xf0, 0xd8, 0xcc, 0xc6, 0xc0, 0xc0, 0xc0, 0xc0 });
            _chars[76] = new consoleChar(new byte[] { 0x00, 0x00, 0x7e, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x78 });
            _chars[77] = new consoleChar(new byte[] { 0x00, 0x00, 0xdb, 0xdb, 0xdb, 0xdb, 0xdb, 0xdb, 0xfe, 0x00, 0x00, 0x00, 0x00 });
            _chars[78] = new consoleChar(new byte[] { 0x00, 0x00, 0xc6, 0xc6, 0xc6, 0xc6, 0xc6, 0xc6, 0xfc, 0x00, 0x00, 0x00, 0x00 });
            _chars[79] = new consoleChar(new byte[] { 0x00, 0x00, 0x7c, 0xc6, 0xc6, 0xc6, 0xc6, 0xc6, 0x7c, 0x00, 0x00, 0x00, 0x00 });
            _chars[80] = new consoleChar(new byte[] { 0xc0, 0xc0, 0xc0, 0xfe, 0xc3, 0xc3, 0xc3, 0xc3, 0xfe, 0x00, 0x00, 0x00, 0x00 });
            _chars[81] = new consoleChar(new byte[] { 0x03, 0x03, 0x03, 0x7f, 0xc3, 0xc3, 0xc3, 0xc3, 0x7f, 0x00, 0x00, 0x00, 0x00 });
            _chars[82] = new consoleChar(new byte[] { 0x00, 0x00, 0xc0, 0xc0, 0xc0, 0xc0, 0xc0, 0xe0, 0xfe, 0x00, 0x00, 0x00, 0x00 });
            _chars[83] = new consoleChar(new byte[] { 0x00, 0x00, 0xfe, 0x03, 0x03, 0x7e, 0xc0, 0xc0, 0x7f, 0x00, 0x00, 0x00, 0x00 });
            _chars[84] = new consoleChar(new byte[] { 0x00, 0x00, 0x1c, 0x36, 0x30, 0x30, 0x30, 0x30, 0xfc, 0x30, 0x30, 0x30, 0x00 });
            _chars[85] = new consoleChar(new byte[] { 0x00, 0x00, 0x7e, 0xc6, 0xc6, 0xc6, 0xc6, 0xc6, 0xc6, 0x00, 0x00, 0x00, 0x00 });
            _chars[86] = new consoleChar(new byte[] { 0x00, 0x00, 0x18, 0x3c, 0x3c, 0x66, 0x66, 0xc3, 0xc3, 0x00, 0x00, 0x00, 0x00 });
            _chars[87] = new consoleChar(new byte[] { 0x00, 0x00, 0xc3, 0xe7, 0xff, 0xdb, 0xc3, 0xc3, 0xc3, 0x00, 0x00, 0x00, 0x00 });
            _chars[88] = new consoleChar(new byte[] { 0x00, 0x00, 0xc3, 0x66, 0x3c, 0x18, 0x3c, 0x66, 0xc3, 0x00, 0x00, 0x00, 0x00 });
            _chars[89] = new consoleChar(new byte[] { 0xc0, 0x60, 0x60, 0x30, 0x18, 0x3c, 0x66, 0x66, 0xc3, 0x00, 0x00, 0x00, 0x00 });
            _chars[90] = new consoleChar(new byte[] { 0x00, 0x00, 0xff, 0x60, 0x30, 0x18, 0x0c, 0x06, 0xff, 0x00, 0x00, 0x00, 0x00 });
            _chars[91] = new consoleChar(new byte[] { 0x00, 0x00, 0x0f, 0x18, 0x18, 0x18, 0x38, 0xf0, 0x38, 0x18, 0x18, 0x18, 0x0f });
            _chars[92] = new consoleChar(new byte[] { 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18 });
            _chars[93] = new consoleChar(new byte[] { 0x00, 0x00, 0xf0, 0x18, 0x18, 0x18, 0x1c, 0x0f, 0x1c, 0x18, 0x18, 0x18, 0xf0 });
            _chars[94] = new consoleChar(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x8f, 0xf1, 0x60, 0x00, 0x00, 0x00 }); // ~ ASC 126
        }
    }

    public struct consoleChar
    {
        byte[] _matrix;

        public consoleChar(byte[] matrix)
        {
            _matrix = matrix;
        }

        public void Draw(Brush brush, Graphics g, Point pt, int width, int height)
        {
            int pX = pt.X;
            int pY = pt.Y;

            for (int b = 12; b >= 0; --b)
            {

                for (int bit = 7; bit >= 0; --bit)
                {
                    if (_matrix[b].HasBit(bit))  // Bit ist gesetzt, Punkt zeichnen
                    {
                        g.FillRectangle(brush, new Rectangle(pX, pY, width, height));
                    }

                    // zum nächsten Bit verschieben
                    pX += width;
                }
                pX = pt.X;
                pY += height;
            }
        }
    }

    internal static class byteEx
    {
        public static bool HasBit(this Byte b, int bit) { return (b & (1 << bit)) != 0; }
    }
}

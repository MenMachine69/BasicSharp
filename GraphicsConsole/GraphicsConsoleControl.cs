using BasicConsole;
using BasicSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

        /// <summary>
        /// Loads an image from file without blocking the file
        /// </summary>
        /// <param name="file">file to load from</param>
        /// <returns>loaded image</returns>
        public static Image ImageFromFile(string file)
        {
            Image img = null;

            using (FileStream fstream = new FileStream(file, FileMode.Open, FileAccess.Read))
                img = Image.FromStream(fstream);

            return img;
        }
    }

    /// <summary>
    /// A control that works like System.Console but supports graphics in multiple ways.
    /// </summary>
    [ToolboxItem(true)]
    public class GraphicsConsoleControl : Control
    {
        composer _composer;
        SolidBrush _borderBrush;
        Color _borderColor;
        Task _composerTask;
        System.Timers.Timer _refreshScreenTimer;
        Char _lastchar = Char.MinValue;
        bool _inputMode = false;
        bool _inkeyMode = false;
        bool _removeLastChar = false;
        bool _cancelInput = false;
        Keys _lastInKey = Keys.None;


        public GraphicsConsoleControl()
        {
            if (Tools.IsDesignMode == false)
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                SetStyle(ControlStyles.ResizeRedraw, true);
                SetStyle(ControlStyles.UserPaint, true);

                SetBorderColor(Color.LightSkyBlue);

                _composer = new composer();
                _composer.Host = this;
                _composer.Started += OnStarted;
                _composer.Stopped += OnStopped;
            }
        }

        public event EventHandler Started;
        public event EventHandler Stopped;

        void OnStarted(object sender, EventArgs e)
        {
            Running = true;
            Started?.Invoke(sender, e);
        }

        void OnStopped(object sender, EventArgs e)
        {
            Running = false;
            Stopped?.Invoke(sender, e);
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

        public bool Running { get; private set; }

        public void Run(string code)
        {
            _lastchar = Char.MinValue;
            _inputMode = false;
            _inkeyMode = false;
            _removeLastChar = false;
            _lastInKey = Keys.None;
            _cancelInput = false;

            Running = true;
            _composer.Interpreter = new Interpreter(code);
            _composer.Start();
        }

        public void Stop()
        {
            _cancelInput = true;
            _composer.Stop();
        }

        public void Sleep(int millisec)
        {
            _composer.Sleep(millisec);
        }

        void _paintScreen(object sender, ElapsedEventArgs e)
        {
            if (InvokeRequired)
                Invoke(new Action(() => { Refresh(); }));
        }

        public void LoadSprite(string filename, string name)
        {
            _composer.LoadSprite(filename, name);
        }

        public void LoadSprite(Image img, string name)
        {
            _composer.LoadSprite(img, name);
        }

        public void DrawSprite(string name, int x, int y)
        {
            _composer.DrawSprite(name, x, y);
        }

        public void DrawSprite(string name, int x, int y, int width, int height)
        {
            _composer.DrawSprite(name, x, y, width, height);
        }

        public void RemoveSprite(string name, int x, int y)
        {
            _composer.RemoveSprite(name, x, y);
        }


        public void DrawLine(int xStart, int yStart, int xEnd, int yEnd, Color color)
        {
            _composer.DrawGraphicElement(eGrahicsElement.Line, color, xStart, yStart, xEnd, yEnd);
        }

        public void DrawRectangle(int xStart, int yStart, int width, int height, Color color)
        {
            _composer.DrawGraphicElement(eGrahicsElement.Rectangle, color, xStart, yStart, width, height);
        }

        public void FillRectangle(int xStart, int yStart, int width, int height, Color color)
        {
            _composer.DrawGraphicElement(eGrahicsElement.FillRectangle, color, xStart, yStart, width, height);
        }

        public void DrawEllipse(int xStart, int yStart, int width, int height, Color color)
        {
            _composer.DrawGraphicElement(eGrahicsElement.Ellipse, color, xStart, yStart, width, height);
        }

        public void FillEllipse(int xStart, int yStart, int width, int height, Color color)
        {
            _composer.DrawGraphicElement(eGrahicsElement.Ellipse, color, xStart, yStart, width, height);
        }

        public void DrawPoint(int xStart, int yStart, Color color)
        {
            _composer.DrawGraphicElement(eGrahicsElement.Point, color, xStart, yStart);
        }

        public void DrawPie(int xStart, int yStart, int width, int height, Color color)
        {
            _composer.DrawGraphicElement(eGrahicsElement.Pie, color, xStart, yStart, width, height);
        }

        public void DrawArc(int xStart, int yStart, int width, int height, Color color)
        {
            _composer.DrawGraphicElement(eGrahicsElement.Arc, color, xStart, yStart, width, height);
        }

        public void DrawBezier(int xStart, int yStart, int width, int height, Color color)
        {
            _composer.DrawGraphicElement(eGrahicsElement.Bezier, color, xStart, yStart, width, height);
        }


        public void SetCursorPos(int row, int col)
        {
            _composer.Locate(row, col);
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

        public void ClearCanvas(int layer)
        {
            _composer.ClearCanvas(layer);
        }

        public void SetCanvas(int canvas)
        {
            _composer.SetCanvas(canvas);
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

        public int ReadKey(int millisec)
        {
            int ret = (int)Keys.None;

            if (InvokeRequired)
                Invoke(new Action(() => { Focus(); }));

            _inkeyMode = true;
            _lastInKey = Keys.None;
            Stopwatch watch = new Stopwatch();
            if (millisec > 0)
                watch.Start();

            while (_inkeyMode)
            {
                Thread.Sleep(1);
                Application.DoEvents();

                if (_lastInKey != Keys.None)
                {
                    ret = (int)_lastInKey;
                    break;
                }

                if (millisec > 0 && watch.ElapsedMilliseconds > millisec)
                    break;
            }

            _inkeyMode = false;

            return ret;
        }

        public string ReadLine(Action<string> feedback)
        {
            string ret = "";
            _inputMode = true;

            if (InvokeRequired)
                Invoke(new Action(() => {
                    Focus();

                    while (_lastchar != '\r' && _cancelInput != true)
                    {
                        if (_removeLastChar)
                        {
                            if (ret.Length > 0)
                            {
                                _composer.Back();
                                ret = ret.Substring(0, ret.Length - 1);
                            }
                        }
                        else if (_lastchar != (char)0)
                        {
                            ret += _lastchar;
                            Write(_lastchar.ToString());
                        }

                        _lastchar = '\0';
                        _removeLastChar = false;

                        Thread.Sleep(1);
                        Application.DoEvents();
                    }
                }));

            if (_cancelInput)
                ret = "";

            _inputMode = false;

            return ret;
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            if (_inkeyMode)
            {
                _lastInKey = e.KeyCode;
                //e.Handled = true;
            }

            base.OnPreviewKeyDown(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (_inputMode)
            {
                if (e.KeyCode == Keys.Return)
                    _inputMode = false;
                else if (e.KeyCode == Keys.Back)
                    _removeLastChar = true;

                e.Handled = true;
            }

            //if (_inkeyMode)
            //{
            //    _lastInKey = e.KeyCode;
            //    e.Handled = true;
            //}

            base.OnKeyDown(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            _lastchar = e.KeyChar;
        }

    }
}

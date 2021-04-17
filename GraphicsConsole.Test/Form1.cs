using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphicsConsole.Test
{
    public partial class Form1 : Form
    {
        bool run = false;

        public Form1()
        {
            InitializeComponent();
        }

        int _cnt = 0;

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            int step = 5;
            int start = 120;

            while (start < 520)
            {
                graphicsConsole1.DrawLine(1,start,720,start,Color.White);
                start += step;
                step = step + 5;
            }

            start = 360;
            int end = 360;

            while (start <= 720)
            {
                graphicsConsole1.DrawLine(start, 120, end, 520, Color.White);
                start += 20;
                end += 40;
            }

            start = 340;
            end = 320;

            while (start >= 1)
            {
                graphicsConsole1.DrawLine(start, 120, end, 520, Color.White);
                start -= 20;
                end -= 40;
            }

            graphicsConsole1.Write("Hello World ! Das ist ein Text mit dem ATARI ST Font. Der kann auch Umlaute (äöüÄÖÜß).");

            graphicsConsole1.Write("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzäöüÄÖÜß+-*/.");

            graphicsConsole1.Write("Hello", Color.Red, Color.Yellow);
            graphicsConsole1.Write(" World!", Color.Yellow, Color.FromArgb(255, 0, 55));

            for (byte i = 32; i < 255; i++)
            {
                graphicsConsole1.Write(i);
            }


            //graphicsConsole1.Write("Hello world! " + DateTime.Now.ToShortTimeString());
        }
    }
}

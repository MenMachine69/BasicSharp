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
        public Form1()
        {
            InitializeComponent();
        }

        int _cnt = 0;

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ++_cnt;
            if (_cnt > 10)
            {
                graphicsConsole1.ClearText();
                _cnt = 0;
            }

            graphicsConsole1.Write("Hello", Color.Red, Color.Yellow);
            graphicsConsole1.Write(" World!", Color.Yellow, Color.FromArgb(255, 0, 55));

            //graphicsConsole1.Write("Hello world! " + DateTime.Now.ToShortTimeString());
        }
    }
}

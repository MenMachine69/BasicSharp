using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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
            graphicsConsole1.LoadSprite(@"C:\Visual Studio 2019\Projects\BasicSharp\Ressources\sprites\Dead (1).png", "step1");
            graphicsConsole1.LoadSprite(@"C:\Visual Studio 2019\Projects\BasicSharp\Ressources\sprites\Dead (2).png", "step2");
            graphicsConsole1.LoadSprite(@"C:\Visual Studio 2019\Projects\BasicSharp\Ressources\sprites\Dead (3).png", "step3");
            graphicsConsole1.LoadSprite(@"C:\Visual Studio 2019\Projects\BasicSharp\Ressources\sprites\Dead (4).png", "step4");
            graphicsConsole1.LoadSprite(@"C:\Visual Studio 2019\Projects\BasicSharp\Ressources\sprites\Dead (5).png", "step5");
            graphicsConsole1.LoadSprite(@"C:\Visual Studio 2019\Projects\BasicSharp\Ressources\sprites\Dead (6).png", "step6");
            graphicsConsole1.LoadSprite(@"C:\Visual Studio 2019\Projects\BasicSharp\Ressources\sprites\Dead (7).png", "step7");
            graphicsConsole1.LoadSprite(@"C:\Visual Studio 2019\Projects\BasicSharp\Ressources\sprites\Dead (8).png", "step8");
            graphicsConsole1.LoadSprite(@"C:\Visual Studio 2019\Projects\BasicSharp\Ressources\sprites\Dead (9).png", "step8");
            graphicsConsole1.LoadSprite(@"C:\Visual Studio 2019\Projects\BasicSharp\Ressources\sprites\Dead (10).png", "step8");

            string code = mleCode.Text;
                //"CLS\r\n" +
                //"PRINT \"Whats your Name? \"\r\n" +
                //"INPUT name\r\n" +
                //"PRINTL name\r\n";

                //"FOR i = 1 TO 3000\r\n" +
                //"DRAWSPRITE \"step1\", 100, 200, 200, 150\r\n" +
                //"SLEEP 10\r\n" +
                //"CLS\r\n" +
                //"DRAWSPRITE \"step2\", 100, 200, 200, 150\r\n" +
                //"SLEEP 10\r\n" +
                //"CLS\r\n" +
                //"DRAWSPRITE \"step3\", 100, 200, 200, 150\r\n" +
                //"SLEEP 10\r\n" +
                //"CLS\r\n" +
                //"DRAWSPRITE \"step4\", 100, 200, 200, 150\r\n" +
                //"SLEEP 10\r\n" +
                //"CLS\r\n" +
                //"DRAWSPRITE \"step5\", 100, 200, 200, 150\r\n" +
                //"SLEEP 10\r\n" +
                //"CLS\r\n" +
                //"DRAWSPRITE \"step6\", 100, 200, 200, 150\r\n" +
                //"SLEEP 10\r\n" +
                //"CLS\r\n" +
                //"DRAWSPRITE \"step7\", 100, 200, 200, 150\r\n" +
                //"SLEEP 10\r\n" +
                //"CLS\r\n" +
                //"DRAWSPRITE \"step8\", 100, 200, 200, 150\r\n" +
                //"SLEEP 10\r\n" +
                //"CLS\r\n" +
                //"DRAWSPRITE \"step9\", 100, 200, 200, 150\r\n" +
                //"SLEEP 10\r\n" +
                //"CLS\r\n" +
                //"DRAWSPRITE \"step10\", 100, 200, 200, 150\r\n" +
                //"SLEEP 10\r\n" +
                //"CLS\r\n" +
                //"NEXT i\r\n";
        

            graphicsConsole1.Run(code);


        //graphicsConsole1.Write("Hello world! " + DateTime.Now.ToShortTimeString());
        }

        private void graphicsConsole1_Started(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => {
                    pshStart.Enabled = false;
                    pshStop.Enabled = true;
                    lblRunning.Visible = true;
                }));
            }
        }

        private void graphicsConsole1_Stopped(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => {
                    pshStart.Enabled = true;
                    pshStop.Enabled = false;
                    lblRunning.Visible = false;
                }));
            }

            
        }

        private void pshStop_Click(object sender, EventArgs e)
        {
            graphicsConsole1.Stop();
        }
    }
}

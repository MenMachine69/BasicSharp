using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Test
{
    class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        static void Main(string[] args)
        {
            DrawInConsoleFromGraphics();
            DrawJpegImageFromGraphics();
            Console.Read();
        }

        static void DrawInConsoleFromGraphics()
        {
            Process process = Process.GetCurrentProcess();
            Graphics g = Graphics.FromHdc(GetDC(process.MainWindowHandle));

            // Create pen.
            Pen bluePen = new Pen(Color.Blue, 3);

            // Create rectangle.
            Rectangle rect = new Rectangle(0, 0, 200, 200);

            // Draw rectangle to screen.
            g.DrawRectangle(bluePen, rect);
            g.DrawLine(new Pen(Color.Red, 2), new Point(100, 100), new Point(0, 0));
            g.DrawString("Welcome to Dotnetvisio.blogspot.com", new Font("Arial", 14),
                         new SolidBrush(Color.Orange), new Point(10, 300));
            g.Save();
        }

        static void DrawJpegImageFromGraphics()
        {
            // Create bitmap
            using (Bitmap newImage = new Bitmap(400, 400))
            {

                // Crop and resize the image.               
                using (Graphics graphic = Graphics.FromImage(newImage))
                {
                    // Create pen.
                    Pen bluePen = new Pen(Color.Blue, 3);

                    // Create rectangle.
                    Rectangle rect = new Rectangle(0, 0, 200, 200);

                    // Crop and resize the image.
                    graphic.DrawRectangle(bluePen, rect);
                    graphic.DrawLine(new Pen(Color.Red, 2), new Point(100, 100),
                                     new Point(0, 0));
                    graphic.DrawString("Welcome to Dotnetvisio.blogspot.com",
                                     new Font("Arial", 14), new SolidBrush(Color.Orange),
                                     new Point(10, 300));
                }

                newImage.Save(AppDomain.CurrentDomain.BaseDirectory + "ImageFromGraphics.jpg", ImageFormat.Jpeg);
            }
        }
    }

    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        foreach (string file in Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "Tests"), "*.bas"))
    //        {
    //            Interpreter basic = new Interpreter(File.ReadAllText(file));
    //            try
    //            {
    //                basic.Exec();
    //            }
    //            catch (Exception e)
    //            {
    //                Console.WriteLine("BAD");
    //                Console.WriteLine(e.Message);
    //                continue;
    //            }
    //            Console.WriteLine("OK");
    //        }
    //        Console.Read();
    //    }
    //}
}

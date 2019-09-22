using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Battleship
{
    public partial class Form1 : Form
    {
        float X;
        float Y;
        float Heading;
        float Speed;

        public Form1()
        {
            InitializeComponent();
            X = pictureBox1.Width / 2;
            Y = pictureBox1.Height / 2;
            Heading = 0;
            Speed = 1.0F;
            timer1.Enabled = true;
        }

        private void DrawCircle(Graphics g)
        {
            g.FillEllipse(new SolidBrush(Color.Red), X, Y, 20, 20);
        }

        private Bitmap rotateImage(Bitmap b, float angle)
        {
            //create a new empty bitmap to hold rotated image
            Bitmap returnBitmap = new Bitmap(b.Width, b.Height);
            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(returnBitmap);
            //move rotation point to center of image
            g.TranslateTransform((float)b.Width / 2, (float)b.Height / 2);
            //rotate
            g.RotateTransform(angle);
            //move image back
            g.TranslateTransform(-(float)b.Width / 2, -(float)b.Height / 2);
            //draw passed in image onto graphics object
            g.DrawImage(b, new Point(0, 0));
            return returnBitmap;
        }

        private void DrawView()
        {
            Bitmap View = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(View);

            g.FillRectangle(new SolidBrush(Color.Blue), 0, 0, View.Width, View.Height);
            //DrawCircle(g);
            Bitmap BB63 = new Bitmap("missouri-s1.bmp");
            BB63.MakeTransparent(BB63.GetPixel(0, 0));

            BB63 = rotateImage(BB63, Heading);
            g.DrawImage(BB63, X-BB63.Width/2, Y - BB63.Height/2);
            
            pictureBox1.Image = View;
            
        }

        private void Timer1_Tick_1(object sender, EventArgs e)
        {
            // convert heading to degrees
            double Degrees = 90.0 - Heading;
            // convert degrees to radians
            double Radians = Degrees * (Math.PI / 180);
            // calculate x and y offset
            float dx = Speed * (float)Math.Cos(Radians);
            float dy = Speed * (float)Math.Sin(Radians);
            X += dx;
            Y -= dy;
            DrawView();
        }
    }
}

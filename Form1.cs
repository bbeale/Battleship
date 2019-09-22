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
        class Ship
        {
            float x;
            float y;
            float h;
            float s;
            Bitmap original;
            public Ship(float X, float Y, float Heading, float Speed, string Filename)
            {
                x = X;
                y = Y;
                h = Heading;
                s = Speed;
                original = new Bitmap(Filename);
                original.MakeTransparent(original.GetPixel(0, 0));
            }

            public float X
            {
                get { return x; }
                set { x = value; }
            }

            public float Y
            {
                get { return y; }
                set { y = value; }
            }

            public float Heading
            {
                get { return h; }
                set { h = value; }
            }

            public float Speed
            {
                get { return s; }
                set { s = value; }
            }

            public Bitmap image
            {
                get { return original; }
            }
        }

        /*float X;
        float Y;
        float Heading;
        float Speed;*/

        Ship Missouri;

        public Form1()
        {
            InitializeComponent();
            Missouri = new Ship(pictureBox1.Width / 2, pictureBox1.Height / 2, 75, 1.0F, "missouri-s1.bmp");

            /*X = pictureBox1.Width / 2;
            Y = pictureBox1.Height / 2;
            Heading = 0;
            Speed = 1.0F*/;
            timer1.Enabled = true;
        }

/*        private void DrawCircle(Graphics g)
        {
            g.FillEllipse(new SolidBrush(Color.Red), X, Y, 20, 20);
        }*/

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
            //Bitmap BB63 = new Bitmap("missouri-s1.bmp");
            //BB63.MakeTransparent(BB63.GetPixel(0, 0));

            Bitmap BB63 = rotateImage(Missouri.image, Missouri.Heading);
            g.DrawImage(BB63, Missouri.X-BB63.Width/2, Missouri.Y - BB63.Height/2);
            
            pictureBox1.Image = View;
            
        }

        private void Timer1_Tick_1(object sender, EventArgs e)
        {
            // convert heading to degrees
            double Degrees = 90.0 - Missouri.Heading;
            // convert degrees to radians
            double Radians = Degrees * (Math.PI / 180);
            // calculate x and y offset
            float dx = Missouri.Speed * (float)Math.Cos(Radians);
            float dy = Missouri.Speed * (float)Math.Sin(Radians);
            Missouri.X += dx;
            Missouri.Y -= dy;
            DrawView();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            pictureBox1.Width = this.Width - 16;
            pictureBox1.Height = this.Height - 36;
            if (pictureBox1.Height < 1)
                timer1.Enabled = false;
            else
                timer1.Enabled = true;
        }
    }
}

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
            float dh; // desired heading
            float s;
            Bitmap original;
            public Ship(float X, float Y, float Heading, float Speed, string Filename)
            {
                x = X;
                y = Y;
                h = Heading;
                dh = h;
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
            }

            public float DesiredHeading
            {
                get { return dh; }
                set { dh = value; }
            }

            public float Speed
            {
                get { return s; }
                set { s = value; }
            }

            public float BitmapWidth
            {
                get { return original.Width; }
            }

            public float BitmapHeight
            {
                get { return original.Height; }
            }

            public Bitmap image
            {
                get { return rotateImage(original, h); }
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

            public void Move()
            {
                //check heading and change if needed
                if (h != dh)
                {
                    if (h < dh)
                    {
                        h += Math.Min(0.2F, dh - h);
                    }
                    else
                    {
                        h -= Math.Min(0.2F, h - dh);
                    }
                }
                // convert heading to degrees
                double Degrees = 90.0 - h;
                // convert degrees to radians
                double Radians = Degrees * (Math.PI / 180);
                // calculate x and y offset
                float dx = s * (float)Math.Cos(Radians);
                float dy = s * (float)Math.Sin(Radians);
                x += dx;
                y -= dy;
            }
        }

        Ship Missouri;

        public Form1()
        {
            InitializeComponent();
            Missouri = new Ship(pictureBox1.Width / 2, pictureBox1.Height / 2, 75, 1.0F, "missouri-s1.bmp");
            timer1.Enabled = true;
        }

        private void DrawCircle(Graphics g, float X, float Y)
        {
            g.FillEllipse(new SolidBrush(Color.Red), X - 3, Y - 3, 6, 6);
        }

        private void DrawView()
        {
            Bitmap View = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(View);
            g.FillRectangle(new SolidBrush(Color.Blue), 0, 0, View.Width, View.Height);
            g.DrawImage(Missouri.image, Missouri.X - Missouri.BitmapWidth / 2, Missouri.Y - Missouri.BitmapHeight / 2);
            DrawCircle(g, Missouri.X, Missouri.Y);
            pictureBox1.Image = View;
            
        }

        private void Timer1_Tick_1(object sender, EventArgs e)
        {
            Missouri.Move();
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

        private void PictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if ((e.X >= pictureBox1.Width-100) && (e.Y <= 100))
            {
                Missouri.DesiredHeading += 45;
                return;
            }

            if (((e.X <= 100) && (e.Y <= 100)))
            {
                Missouri.DesiredHeading -= 45;
                return;
            }
            if (e.Y < 50)
            {
                Missouri.DesiredHeading = Missouri.Heading;
            }
        }
    }
}

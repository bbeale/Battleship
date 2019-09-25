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
            string name;
            float x;
            float y;
            float h;
            float dh; // desired heading
            float s;
            float ds; // desired speed
            int engine; // 0 = full stop, 1 = ahead 1 quarter, 2 = ahead 1 half, 3 = ahead 3 quarters, 4 = ahead full 
            Bitmap original;
            public Ship(string Name, float X, float Y, float Heading, float Speed, string Filename)
            {
                name = Name;
                x = X;
                y = Y;
                h = Heading;
                dh = h;
                s = Speed;
                engine = 4;
                ds = Speed;
                original = new Bitmap(Filename);
                original.MakeTransparent(original.GetPixel(0, 0));
            }

            public string Name
            {
                get { return name; }
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

            public float DesiredSpeed
            {
                get { return ds; }
                set { ds = value; }
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
                // check speed and change if needed
                if (s != ds)
                {
                    if (ds < s)
                    {
                        s -= Math.Min(0.001F, s - ds);
                    }
                    else
                    {
                        s += Math.Min(0.001F, ds - s);
                    }
                }
                // calculate x and y offset
                float dx = s * (float)Math.Cos(Radians);
                float dy = s * (float)Math.Sin(Radians);
                x += dx;
                y -= dy;
            }

            public void EngineUp()
            {
                if ((engine >= 0)&&(engine < 4))
                    ++engine;
                if (s < 0.25F)
                    engine = 1;
                else
                    if (s < 0.5F)
                        engine = 2;
                    else
                    if (s < 0.75F)
                        engine = 3;
                    else
                        engine = 4;
                switch (engine)
                {
                    case 0: ds = 0; break;
                    case 1: ds = 0.25F; break;
                    case 2: ds = 0.5F; break;
                    case 3: ds = 0.75F; break;
                    case 4: ds = 1.0F; break;
                }
            }

            public void EngineDown()
            {
                if (engine > 0)
                    --engine;
                if (s <= 0.25F)
                    engine = 0;
                else
                    if (s <= 0.5F)
                        engine = 1;
                    else
                        if (s <= 0.75F)
                            engine = 2;
                        else
                            engine = 3;
                switch (engine)
                {
                    case 0: ds = 0; break;
                    case 1: ds = 0.25F; break;
                    case 2: ds = 0.5F; break;
                    case 3: ds = 0.75F; break;
                    case 4: ds = 1.0F; break;
                }
            }

               public void EngineHold()
            {
                engine = -1;
                ds = s;
            }
        }

        System.Collections.ArrayList Ships;

        Ship SelectedShip;
        int SelectedShipIndex;
        int HeadsUpX;
        int HeadsUpY;
        Color HeadsUpColor;

        public Form1()
        {
            InitializeComponent();
            HeadsUpX = 100;
            HeadsUpY = 100;
            HeadsUpColor = Color.Orange;
            Ships = new System.Collections.ArrayList();
            Ships.Add(new Ship("Missouri", pictureBox1.Width / 2, pictureBox1.Height / 2, 75, 1.0F, "missouri-s1.bmp"));
            Ships.Add(new Ship("Iowa", pictureBox1.Width / 2, pictureBox1.Height / 2 + 152, 95, 0.25F, "missouri-s1.bmp"));
            SelectedShipIndex = 0;
            SelectedShip = (Ship)Ships[SelectedShipIndex];
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
            
            // draw ocean background
            g.FillRectangle(new SolidBrush(Color.Blue), 0, 0, View.Width, View.Height);

            // draw HUD
            System.Drawing.Font font = new System.Drawing.Font("Sans Serif", 15.0F);
            SolidBrush brush = new SolidBrush(HeadsUpColor);
            g.DrawString(SelectedShip.Name, font, brush, new PointF(HeadsUpX, HeadsUpY));
            g.DrawString(SelectedShip.Heading.ToString("f2"), font, brush, new PointF(HeadsUpX, HeadsUpY + 25));
            g.DrawString(SelectedShip.Speed.ToString("f3"), font, brush, new PointF(HeadsUpX, HeadsUpY + 50));

            // draw ships
            foreach (Ship ship in Ships)
            {
                g.DrawImage(ship.image, ship.X - ship.BitmapWidth / 2, ship.Y - ship.BitmapHeight / 2);
                DrawCircle(g, ship.X, ship.Y);
            }

            // display view
            pictureBox1.Image = View;
            
        }

        private void Timer1_Tick_1(object sender, EventArgs e)
        {
            foreach (Ship ship in Ships)
                ship.Move();

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
                SelectedShip.DesiredHeading += 45;
                return;
            }

            if (((e.X <= 100) && (e.Y <= 100)))
            {
                SelectedShip.DesiredHeading -= 45;
                return;
            }
            if (e.Y < 50)
            {
                SelectedShip.DesiredHeading = SelectedShip.Heading;
                return;
            }

            if ((e.X <= 100) && (e.Y >= pictureBox1.Height - 100))
            {
                SelectedShip.EngineDown();
                return;
            }

            if ((e.X >= pictureBox1.Width - 100) && (e.Y >= pictureBox1.Height - 100))
            {
                SelectedShip.EngineUp();
                return;
            }

            if (e.Y >= pictureBox1.Height - 50)
            {
                SelectedShip.EngineHold();
                return;
            }

            if (e.X >= pictureBox1.Width - 50)
            {
                if (SelectedShipIndex < Ships.Count - 1)
                    ++SelectedShipIndex;
                
                else
                    SelectedShipIndex = 0;
                                    
                SelectedShip = (Ship)Ships[SelectedShipIndex];
            }
        }
    }
}

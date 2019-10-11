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
            bool ff;    // true = friend, false = foe
            float x;
            float y;
            float h;
            float dh; // desired heading
            float s;
            float ds; // desired speed
            int engine; // 0 = full stop, 1 = ahead 1 quarter, 2 = ahead 1 half, 3 = ahead 3 quarters, 4 = ahead full 
            Bitmap original;
            public Ship(string Name, float X, float Y, float Heading, float Speed, string Filename, bool FriendOrFoe)
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
                ff = FriendOrFoe;
            }

            public string Name
            {
                get { return name; }
            }

            public bool FriendOrFoe
            {
                get { return ff; }
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
                y += dy;
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
        
        Ship SelectedShip;
        int SelectedShipIndex;
        System.Collections.ArrayList Ships;

        int ViewX, ViewY;
        bool DragOn;
        int DragOriginX, DragOriginY;

        int HeadsUpX;
        int HeadsUpY;
        Color HeadsUpColor;

        int MinimapX, MinimapY;
        int MinimapPixelWidth, MinimapPixelHeight;

        // cartesian units
        float MinimapWidth;
        float MinimapHeight;
        Color MinimapColor;
        Color MinimapSelectedShipColor;
        Color MinimapEnemyShipColor;
        Color MinimapShipColor;

        int HourGlass;
        int HighlightState; // 1 = waxing, 2 = waning, 3 = hidden

        Bitmap View;
        Graphics g;

        public Form1()
        {
            InitializeComponent();
            HeadsUpX = 100;
            HeadsUpY = 100;
            HeadsUpColor = Color.Orange;
            Ships = new System.Collections.ArrayList();
            Ships.Add(new Ship("Carrier", 0, 0, 75, 0.25F, "carrier.bmp", true));
            Ships.Add(new Ship("Iowa", 0, -250, 90, 0.25F, "missouri-s1.bmp", true));
            Ships.Add(new Ship("Enemy 1", 2500, 2500, 215, 1.0F, "hood-s1.bmp", false));
            Ships.Add(new Ship("Enemy 2", 2750, 2750, 215, 1.0F, "hood-s1.bmp", false));
            SelectedShipIndex = 0;
            SelectedShip = (Ship)Ships[SelectedShipIndex];

            ViewX = 0;
            ViewY = 0;
            DragOn = false;

            View = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(View);

            MinimapX = 0;
            MinimapY = 0;
            MinimapPixelWidth = 200;
            MinimapPixelHeight = 200;
            MinimapWidth = 10000;
            MinimapHeight = 10000;
            MinimapColor = Color.DarkBlue;
            MinimapSelectedShipColor = Color.DarkGreen;
            MinimapShipColor = Color.LightBlue;
            MinimapEnemyShipColor = Color.Red;
            HourGlass = 100;
            HighlightState = 3;

            timer1.Enabled = true;
        }

        private void DrawCircle(Graphics g, float X, float Y)
        {
            g.FillEllipse(new SolidBrush(Color.Red), X - 3, Y - 3, 6, 6);
        }

        private void DrawView()
        {
            // draw ocean background
            g.FillRectangle(new SolidBrush(Color.Blue), 0, 0, View.Width, View.Height);

            // draw HUD
            System.Drawing.Font font = new System.Drawing.Font("Sans Serif", 15.0F);
            SolidBrush brush = new SolidBrush(HeadsUpColor);
            g.DrawString(SelectedShip.Name, font, brush, new PointF(HeadsUpX, HeadsUpY));
            g.DrawString(SelectedShip.Heading.ToString("f2"), font, brush, new PointF(HeadsUpX, HeadsUpY + 25));
            g.DrawString(SelectedShip.Speed.ToString("f3"), font, brush, new PointF(HeadsUpX, HeadsUpY + 50));
            g.DrawString("(" + SelectedShip.X.ToString("f1") + "," + SelectedShip.Y.ToString("f1") + ")", font, brush, new PointF(HeadsUpX, HeadsUpY + 75));
            g.DrawString("View: ("+ViewX.ToString()+", "+ViewY.ToString()+")", font, brush, new PointF(HeadsUpX, HeadsUpY + 100));

            // minimap
            Bitmap Minimap = new Bitmap(MinimapPixelWidth, MinimapPixelHeight);
            Graphics gm = Graphics.FromImage(Minimap);
            gm.FillRectangle(new SolidBrush(MinimapColor), 0, 0, MinimapPixelWidth, MinimapPixelHeight);

            // draw ships on minimap
            float UnitsPerPixel = MinimapWidth / MinimapPixelWidth;
            foreach (Ship s in Ships)
            {
                int x = Convert.ToInt32(MinimapPixelWidth / 2.0F + s.X / UnitsPerPixel);
                int y = Convert.ToInt32(MinimapPixelHeight / 2.0F - s.Y / UnitsPerPixel);
                if (s.FriendOrFoe == true)
                {
                    if (s == SelectedShip)
                        gm.FillRectangle(new SolidBrush(MinimapSelectedShipColor), x, y, 4, 4);
                    else
                        gm.FillRectangle(new SolidBrush(MinimapShipColor), x, y, 6, 6);
                }
                else
                    gm.FillRectangle(new SolidBrush(MinimapEnemyShipColor), x, y, 6, 6);
            }

            // draw frame on minimap
            int xc = ViewX - View.Width / 2;
            int x1 = Convert.ToInt32(MinimapPixelWidth / 2.0F + xc / UnitsPerPixel);
            int yc = ViewY + View.Height / 2;
            int y1 = Convert.ToInt32(MinimapPixelHeight / 2.0F - yc / UnitsPerPixel);
            int xw = Convert.ToInt32(View.Width / UnitsPerPixel);
            int yw = Convert.ToInt32(View.Height / UnitsPerPixel);
            gm.DrawRectangle(new Pen(Color.Green), x1, y1, xw, yw);

            // draw minimap onto main view
            MinimapX = View.Width - MinimapPixelWidth - 100;
            MinimapY = 100;
            g.DrawImage(Minimap, MinimapX, MinimapY);

            // highlight selected ship
            --HourGlass;
            if (HourGlass == 0)
            {
                switch (HighlightState)
                {
                    case 1:     // waxing
                        HourGlass = 10;
                        HighlightState = 2;
                        break;
                    case 2:     // waning
                        HourGlass = 10;
                        HighlightState = 3;
                        break;
                    case 3:     // hidden
                        HourGlass = 10;
                        HighlightState = 1;
                        break;
                }
            }
            switch (HighlightState)
            {
                case 1:     // waxing
                    g.DrawEllipse(new Pen(Color.FromArgb(100 - HourGlass * 10, Color.GreenYellow),3), SelectedShip.X - ViewX - 125 + pictureBox1.Width/2, ViewY - SelectedShip.Y - 125 + pictureBox1.Height / 2, 250, 250);
                    
                    break;
                case 2:     // waning
                    g.DrawEllipse(new Pen(Color.FromArgb(HourGlass * 10, Color.GreenYellow), 3), SelectedShip.X - ViewX - 125 + pictureBox1.Width / 2, ViewY - SelectedShip.Y - 125 + pictureBox1.Height / 2, 250, 250);

                    break;
            }

            // draw ships
            foreach (Ship ship in Ships)
            {
                g.DrawImage(ship.image, ship.X - ViewX - ship.BitmapWidth / 2 + pictureBox1.Width / 2, ViewY - ship.Y - ship.BitmapHeight / 2 + pictureBox1.Height/2);
                //DrawCircle(g, ship.X - ViewX, ship.Y + ViewY);
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

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            DragOn = true;
            DragOriginX = e.X;
            DragOriginY = e.Y;
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            DragOn = false;
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (DragOn == true)
            {
                ViewX += DragOriginX - e.X;
                ViewY -= DragOriginY - e.Y;
                DragOriginX = e.X;
                DragOriginY = e.Y;
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            pictureBox1.Width = this.Width - 16;
            pictureBox1.Height = this.Height - 36;
            // re declare graphics and bitmap
            View = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(View);

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
                int PossibleIndex = SelectedShipIndex;
                do
                {
                    if (PossibleIndex < Ships.Count - 1)
                        ++PossibleIndex;
                    else
                        PossibleIndex = 0;
                } while (((Ship)Ships[PossibleIndex]).FriendOrFoe == false);
                SelectedShipIndex = PossibleIndex;                         
                SelectedShip = (Ship)Ships[SelectedShipIndex];
            }

            if ((e.X >= MinimapX) && (e.X <=(MinimapX+ MinimapPixelWidth))&& (e.Y>=MinimapY)&&(e.Y <= (MinimapY + MinimapPixelHeight)))
            {
                int px = e.X - (MinimapX + MinimapPixelWidth / 2);
                int py = e.Y - (MinimapY + MinimapPixelHeight / 2);
                float UnitsPerPixel = MinimapWidth / MinimapPixelWidth;
                ViewX = Convert.ToInt32(px * UnitsPerPixel);
                ViewY = -1 * Convert.ToInt32(py * UnitsPerPixel);
            }
        }
    }
}

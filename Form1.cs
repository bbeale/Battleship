﻿using System;
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
        class Foam
        {
            double x, y; // global cartesian location
            int age;
            int ma; // max age
            float r;
            double dx, dy; // direction
            public Foam(double X, double Y, float Radius, double Dx, double Dy, int Life)
            {
                x = X;
                y = Y;
                age = 0;
                ma = Life;
                r = Radius;
                dx = Dx;
                dy = Dy;
            }
            public double X
            {
                get { return x; }
            }
            public double Y
            {
                get { return y; }
            }
            public void Tick()
            {
                ++age;
                // move foam
                x += dx;
                y += dy;
            }
            public void Draw(Graphics g, float bx, float by, float Scale) // Need global view coordinates or bitmap coordinates
            {
                int alpha = 255 - Convert.ToInt32(255 * ((float)age / ma));
                if (alpha < 0) alpha = 0;
                if (alpha > 255) alpha = 255;
                SolidBrush b = new SolidBrush(Color.FromArgb(alpha, Color.White));
                float r1 = r * Scale;
                float d = 2 * r;
                g.FillEllipse(b, bx - r1, by - r1, d, d);
            }
            public bool Visible
            {
                get
                {
                    if (age < ma)
                        return true;
                    else
                        return false;
                }
            }
        } // end class Foam

        class Explosion
        {
            double x;
            double y;
            int frame;
            double size;
            public Explosion(double X, double Y, double Size)
            {
                x = X;
                y = Y;
                size = Size;
                frame = 1;
            }
            public double X // Read Only
            {
                get { return x; }
            }
            public double Y
            {
                get { return y; }
            }
            public int Frame
            {
                get { return frame; }
                set { frame = value; }
            }
            public double Size
            {
                get { return size; }
            }
            public void Grow()
            {
                ++frame;
            }
        }

        class Shell // Munition, Torpedo, Bomb, Projectile, Shot, Round, Ordnance, Ammunition, Weapon, Rocket, Missle
        {
            string type; // "16 in/50 caliber Mark 7"
            int t; // time of impact
            double x, y; // impact location in global coordinates
            int damage;
            public Shell(string Type, int Time, double X, double Y, int Damage)
            {
                type = Type;
                t = Time;
                x = X;
                y = Y;
                damage = Damage;
            }
            public string Type
            {
                get { return type; }
            }
            public int Time
            {
                get { return t; }
            }
            public double X
            {
                get { return x; }
            }
            public double Y
            {
                get { return y; }
            }
            public int Damage
            {
                get { return damage; }
            }
        }

        class Gun
        {
            bool loaded;
            string type;
            double range; // in cartesian units
            float velocity; // in cartesian untis
            int loadtime;
            int rttl; // Remaining Time To Load
            Ship parent;
            Ship target;
            // number of shells?
            // bitmap of gun
            // location relative to ship center
            // orientation of gun
            // firing animation
            public Gun(string Type, Ship Parent, double Range, float Velocity, int LoadDuration)
            {
                type = Type;
                range = Range; // 2000.0
                velocity = Velocity; // 8.0
                loadtime = LoadDuration; // 200
                rttl = 0;
                loaded = true;
                parent = Parent;
                target = null;
            }
            public bool Loaded
            {
                get { return loaded; }
                set { loaded = value; }
            }
            public Ship Target
            {
                get { return target; }
                set { target = value; }
            }
            public double Range // Read-Only
            {
                get { return range; }
            }
            public void Operate(int Clock, System.Collections.ArrayList Shells)
            {
                if (loaded)
                {
                    if (target != null)
                    {
                        double d = Math.Sqrt((parent.X - target.X) * (parent.X - target.X) + (parent.Y - target.Y) * (parent.Y - target.Y)); // in cartesian units
                        if (d < range) // Fire!
                        {
                            int flighttime = Convert.ToInt32(d / velocity);
                            // predict location (d=rt)
                            // Determine cartesian impact coordinates
                            double degrees = -1 * (target.Heading - 90.0F); // target ship cartesain heading
                            double radians = degrees * (Math.PI / 180.0);
                            double ix = target.X + target.Speed * flighttime * Math.Cos(radians);
                            double iy = target.Y + target.Speed * flighttime * Math.Sin(radians);
                            // Create Shell, start Reload
                            Shells.Add(new Shell("16in/50 caliber Mark 7", Clock + flighttime, ix, iy, 200));
                            loaded = false;
                            rttl = loadtime;
                        }
                    }
                }
                else
                {
                    --rttl;
                    if (rttl == 0)
                        loaded = true;
                }
            }
        }

        class Ship
        {
            string name;
            int hp;
            bool ff;    // true = friend, false = foe
            Gun gun;
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
                hp = 1000;
                gun = new Gun("16 in/50 caliber Mark 7", this, 2000, 8.0F, 200);
            }

            public string Name
            {
                get { return name; }
            }

            public int HP
            {
                get { return hp; }
                set { hp = value; }
            }

            public bool FriendOrFoe
            {
                get { return ff; }
            }

            public Gun Gun
            {
                get { return gun; }
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
        // float MinimapHeight;
        Color MinimapColor;
        Color MinimapSelectedShipColor;
        Color MinimapEnemyShipColor;
        Color MinimapShipColor;

        int HourGlass;
        int HighlightState; // 1 = waxing, 2 = waning, 3 = hidden

        int Clock;

        System.Collections.ArrayList Shells;
        System.Collections.ArrayList Explosions;
        System.Collections.ArrayList Wake;
        Random R;

        Bitmap View;
        Graphics g;

        public Form1()
        {
            InitializeComponent();
            HeadsUpX = 100;
            HeadsUpY = 100;
            HeadsUpColor = Color.Orange;
            Ships = new System.Collections.ArrayList();
            Ships.Add(new Ship("Nimitz", 0, 0, 75, 0.25F, "carrier.bmp", true));
            Ships.Add(new Ship("Missouri", 0, -250, 90, 0.25F, "missouri-s2.bmp", true));
            Ships.Add(new Ship("ToonHood", 2500, 2500, 215, 1.0F, "hood-s1.bmp", false));
            Ships.Add(new Ship("ToonMissouri", 2750, 2750, 215, 1.0F, "missouri-s1.bmp", false));
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
            // MinimapHeight = 10000;
            MinimapColor = Color.DarkBlue;
            MinimapSelectedShipColor = Color.DarkGreen;
            MinimapShipColor = Color.LightBlue;
            MinimapEnemyShipColor = Color.Red;

            Shells = new System.Collections.ArrayList();
            Clock = 0;

            HourGlass = 100;
            HighlightState = 3;

            Explosions = new System.Collections.ArrayList();
            Wake = new System.Collections.ArrayList();
            R = new Random();

            timer1.Enabled = true;
        }

        private float ConvertCartesianXtoBitmapX(Bitmap b, double CartesianX)
        {
            return b.Width / 2.0F + (float)(CartesianX - ViewX);
        }

        private float ConvertCartesianYtoBitmapY(Bitmap b, double CartesianY)
        {
            return b.Height / 2.0F - (float)(CartesianY - ViewY);
        }

        private void DrawCircle(Graphics g, float X, float Y)
        {
            g.FillEllipse(new SolidBrush(Color.Red), X - 3, Y - 3, 6, 6);
        }

        private void DrawExplosion(Graphics g, Explosion E)
        {
            double r = Math.Sqrt(E.Size * E.Frame);
            int green = 255 - (E.Frame - 1) * 50; // assumes 5 frames
            g.FillEllipse(new SolidBrush(Color.FromArgb(255, green, 0)), (float)(E.X - ViewX + pictureBox1.Width / 2 - r), (float)(ViewY - E.Y + pictureBox1.Height / 2 - r), (float)(2 * r), (float)(2 * r));
        }

        private void DrawView()
        {
            // draw ocean background
            g.FillRectangle(new SolidBrush(Color.Blue), 0, 0, View.Width, View.Height);
            
            // Draw wake
            foreach (Foam f in Wake)
            {
                float BitmapX = ConvertCartesianXtoBitmapX(View, f.X);
                float BitmapY = ConvertCartesianYtoBitmapY(View, f.Y);
                f.Draw(g, BitmapX, BitmapY, 1.0F);
            }

            foreach (Shell shell in Shells)
            {
                DrawCircle(g, (float)(shell.X - ViewX + pictureBox1.Width / 2), (float)(ViewY - shell.Y + pictureBox1.Height / 2));
            }

            // draw HUD
            Font font = new Font("Sans Serif", 15.0F);
            SolidBrush brush = new SolidBrush(HeadsUpColor);
            g.DrawString(SelectedShip.Name, font, brush, new PointF(HeadsUpX, HeadsUpY));
            g.DrawString(SelectedShip.Heading.ToString("f2"), font, brush, new PointF(HeadsUpX, HeadsUpY + 25));
            g.DrawString(SelectedShip.Speed.ToString("f3"), font, brush, new PointF(HeadsUpX, HeadsUpY + 50));
            g.DrawString("(" + SelectedShip.X.ToString("f1") + "," + SelectedShip.Y.ToString("f1") + ")", font, brush, new PointF(HeadsUpX, HeadsUpY + 75));
            g.DrawString("HP " + SelectedShip.HP.ToString(),font, brush, new PointF(HeadsUpX, HeadsUpY + 100));
            g.DrawString("View: (" + ViewX.ToString() + ", " + ViewY.ToString() + ")", font, brush, new PointF(HeadsUpX, HeadsUpY + 125));

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
                g.DrawImage(ship.image, ship.X - ViewX - ship.BitmapWidth / 2 + pictureBox1.Width / 2, ViewY - ship.Y - ship.BitmapHeight / 2 + pictureBox1.Height / 2);
                //DrawCircle(g, ship.X - ViewX, ship.Y + ViewY);
            }

            // draw explosions
            System.Collections.ArrayList DeadExplosions = new System.Collections.ArrayList();
            foreach (Explosion E in Explosions)
            {
                DrawExplosion(g, E);
                E.Grow();
                if (E.Frame == 6)
                {
                    DeadExplosions.Add(E);
                }
            }

            foreach (Explosion E in DeadExplosions)
                Explosions.Remove(E);

            // display view
            pictureBox1.Image = View;
        }

        private void IndicateMiss(double X, double Y)
        {
            // Add initial animation?
            for (int i = 0; i < 18; i++) // wake
            {
                double a = (i * 20) * Math.PI / 180.0; // angle in radians
                //double r = R.Next(100) * 0.05; // radius 0 to 5
                double r = 3.0 + R.Next(10) * 0.1;
                double x = r * Math.Cos(a);
                double y = r * Math.Sin(a);
                float Radius = (R.Next(30) + 10) / 20.0F;
                // Note: fast dx, dy looks like explosion
                double dx = 0.035 * Math.Cos(a);
                double dy = 0.035 * Math.Sin(a);
                Wake.Add(new Foam(X + x, Y + y, Radius, dx, dy, R.Next(300) + 50));
            }
            Wake.Add(new Foam(X, Y, 5.0F, 0, 0, 12)); // blast
        }

        private void Timer1_Tick_1(object sender, EventArgs e)
        {
            ++Clock; 
            foreach (Ship ship in Ships)
            {
                ship.Move();
                ship.Gun.Operate(Clock, Shells);
            }

            // assign targets
            foreach (Ship ship in Ships)
            {
                double distance = 10000.0;
                ship.Gun.Target = null;

                foreach (Ship target in Ships)
                {
                    if (ship.FriendOrFoe != target.FriendOrFoe)
                    {
                        double d = Math.Sqrt((ship.X - target.X) * (ship.X - target.X) + (ship.Y - target.Y) * (ship.Y - target.Y));
                        if (d < distance)
                        {
                            distance = d;
                            ship.Gun.Target = target;
                        }
                    }
                }
            }

            // process in-flight shells
            System.Collections.ArrayList DeadShells = new System.Collections.ArrayList();
            foreach (Shell s in Shells)
            {
                if (Clock >= s.Time)    // hits impact point
                {
                    bool Miss = true;
                    foreach (Ship ship in Ships)
                    {
                        double d = Math.Sqrt((ship.X - s.X) * (ship.X - s.X) + (ship.Y - s.Y) * (ship.Y - s.Y));
                        if (d <= 10)    // hit! 
                        {
                            ship.HP -= s.Damage;
                            Explosions.Add(new Explosion(s.X, s.Y, 75));
                            Miss = false;
                        }
                    }

                    DeadShells.Add(s); 
                    if (Miss)
                        IndicateMiss(s.X, s.Y);
                }
            }

            foreach (Shell s in DeadShells)
            {
                Shells.Remove(s);
            }

            // remove sunken ships 
            System.Collections.ArrayList DeadShips = new System.Collections.ArrayList();
            foreach (Ship ship in Ships)
            {
                if (ship.HP <= 0) 
                    DeadShips.Add(ship);
            }
                
            foreach (Ship ship in DeadShips)
            {
                if (ship == SelectedShip)
                    GoToNextShip();
                Ships.Remove(ship);
            }

            // draw ship wakes
            foreach (Ship s in Ships)
            {
                int Rate = 1000000; // Rate = Infinity
                if (s.Speed > 0.05)
                {
                    Rate = Convert.ToInt32(1 / s.Speed);
                }
                if (Clock % Rate == 0)
                {
                    double a = -1 * (s.Heading - 90);
                    a = (Math.PI / 180) * a; // convert to radians
                    // Starboard wake
                    float x = -100 * (float)Math.Cos(a + 0.03);
                    float y = -100 * (float)Math.Sin(a + 0.03);
                    float sp = (400 - R.Next(200)) / 10000.0F; // Convert to function of speed of ship
                    double dx = sp * Math.Cos(a + Math.PI / 2);
                    double dy = sp * Math.Sin(a + Math.PI / 2);
                    float Radius = (R.Next(50) + 1) / 25.0F;
                    int Life = 400 + R.Next(200);
                    Wake.Add(new Foam(s.X + x, s.Y + y, Radius, -dx, -dy, Life));
                    // Port wake
                    x = -100 * (float)Math.Cos(a - 0.03);
                    y = -100 * (float)Math.Sin(a - 0.03);
                    Radius = (R.Next(50) + 1) / 25.0F;
                    // Recompute speed?
                    Life = 400 + R.Next(200);
                    Wake.Add(new Foam(s.X + x, s.Y + y, Radius, dx, dy, Life));
                }
            }

            // age foam
            System.Collections.ArrayList DeadFoam = new System.Collections.ArrayList();
            foreach (Foam f in Wake)
            {
                f.Tick();
                if (f.Visible == false)
                    DeadFoam.Add(f);
            }

            foreach (Foam f in DeadFoam)
                Wake.Remove(f);

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

        private void GoToNextShip()
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
                GoToNextShip();
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

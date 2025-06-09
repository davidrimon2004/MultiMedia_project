using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace project
{
    public class CAdvImgActor
    {
        public Bitmap wrld;
        public Rectangle rcDst, rcSrc;
    }
    public class CMultiImgActor
    {
        public int X, Y;
        public List<Bitmap> frames;
        public int iFrame;
        //Dir right or Left 
        public int Dir;
        //state Running/ walking or standing
        public int state;
        public bool gravity;
    }
    public class CImageActor
    {
        public int x, y;
        public Bitmap img;
    }
    public partial class Form1 : Form
    {
        Bitmap off;
        Timer tt = new Timer();
        CAdvImgActor wrld = new CAdvImgActor();
        CMultiImgActor Eliot = new CMultiImgActor();
        List<CImageActor> gun = new List<CImageActor>();
        int ct = 0;
        int ctTick = 0;
        Bitmap iff;
        int ctjump = 0;
        int y = 260;
        int speed = 10;
        int speed2 = 10;
        int igun; //0->right , //1->left
        //Rectangle temp;
        //Rectangle temp2;
        public Form1()
        {
            // WindowState= FormWindowState.Maximized;
            this.Load += Form1_Load;
            this.Paint += Form1_Paint;
            this.KeyDown += Form1_KeyDown;
            tt.Interval = 1000 / 60; // 60 FPS
            tt.Start();
            tt.Tick += Tt_Tick;
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            off = new Bitmap(ClientSize.Width, ClientSize.Height);
            create_world();
            create_Hero();
            create_gun();
        }
        private void Tt_Tick(object sender, EventArgs e)
        {
            if (Eliot.gravity)
            {
                ctjump++;
                if (ctjump > 5 && ctjump<=15)
                {
                    Eliot.Y += 2;
                    
                }
                else if(ctjump == 16)
                {
                    ctjump = 0;
                    Eliot.gravity = false;
                }
            }
            igun = Eliot.state;
            movegun();
            ctTick++;
            DrawDubb(this.CreateGraphics());
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode) //scrolling logic
            {
                case Keys.D:
                    Eliot.state = 0;
                    if (wrld.rcSrc.X + wrld.rcSrc.Width <= wrld.wrld.Width && Eliot.X >= (ClientSize.Width / 2) - 100)
                    {
                         speed = 0;
                         wrld.rcSrc.X += 3;
                    }
                    else
                    {
                        speed = 3;
                    }
                    if (Eliot.X <= ClientSize.Width - Eliot.frames[Eliot.iFrame].Width)
                    {
                        // Eliot.X += 2;
                        if (Eliot.X <= ClientSize.Width)
                        {
                            Eliot.X += speed;

                        }
                        if (Eliot.iFrame < 3)
                        {
                            if (ct % 4 == 0)
                            {
                                Eliot.iFrame++;

                            }
                            ct++;
                        }
                        else
                        {
                            Eliot.iFrame = 1;
                        }
                    }
                    break;

                case Keys.A:
                    // Eliot.state = 1;
                    if (wrld.rcSrc.X >= 0)
                    {
                        if (Eliot.X <= ClientSize.Width / 2)
                        {
                            wrld.rcSrc.X -= 3;
                            speed2 = 0;
                        }

                    }
                    else
                    {
                        speed2 = 3;
                    }
                    if (Eliot.X >= 0)
                    {
                        Eliot.X -= speed2;
                    }
                    if (Eliot.X >= 0)
                    {
                        // Eliot.X -= 2;
                        if (Eliot.state == 0)
                        {
                            Eliot.iFrame = 4;
                        }
                        Eliot.state = 1;
                        if (Eliot.iFrame < 7)
                        {
                            if (ct % 4 == 0)
                            {
                                Eliot.iFrame++;
                            }
                            ct++;
                        }
                        else
                        {
                            Eliot.iFrame = 4;

                        }
                    }
                    break;

                case Keys.W:
                    if (wrld.rcSrc.Y >= 0 && !Eliot.gravity &&Eliot.Y>=y)
                    {
                         Eliot.Y -= 20; Eliot.gravity = true;
                    }
                    else if (Eliot.Y < y)
                    {
                        Eliot.Y = y;
                        Eliot.gravity = false;
                    }
                    break;

                case Keys.S:
                    if (wrld.rcSrc.Y + wrld.rcSrc.Height <= wrld.wrld.Height)
                        wrld.rcSrc.Y += 2;
                    break;

                case Keys.Q:
                    if (wrld.rcSrc.Y >= 0 && !Eliot.gravity &&Eliot.Y>=y)
                    {
                       
                        Eliot.Y -= 20;
                        //Eliot.X -= 10;
                        Eliot.gravity = true;
                    }
                    else if (Eliot.Y < y)
                    {
                        Eliot.Y = y;
                        Eliot.gravity = false;
                    }
                    if (wrld.rcSrc.X >= 0)
                    {
                        if (Eliot.X <= ClientSize.Width / 2 && Eliot.Y >= y)
                        {
                            wrld.rcSrc.X -= 10;
                            speed2 = 0;
                        }

                    }
                    else
                    {
                        speed2 = 10;
                    }
                    if (Eliot.X >= 0 )
                    {
                        Eliot.X -= speed2;
                    }
                    if (Eliot.X >= 0)
                    {
                        // Eliot.X -= 2;
                        Eliot.state = 1;
                        if (Eliot.iFrame < 7)
                        {
                            if (ct % 4 == 0)
                            {
                                Eliot.iFrame++;
                            }
                            ct++;
                        }
                        else
                        {
                            Eliot.iFrame = 4;

                        }
                    }
                    break;

                case Keys.E:
                   
                    if (wrld.rcSrc.X + wrld.rcSrc.Width <= wrld.wrld.Width && Eliot.X >= (ClientSize.Width / 2) - 100 && Eliot.Y >= y)
                    {
                        speed = 0;
                        wrld.rcSrc.X += 10;
                    }
                    else
                    {
                        speed = 10;
                    }
                    if (Eliot.X <= ClientSize.Width - Eliot.frames[Eliot.iFrame].Width)
                    {
                        // Eliot.X += 2;
                        if (Eliot.X <= ClientSize.Width && Eliot.Y >= y)
                        {
                            Eliot.X += speed;

                        }
                        if (wrld.rcSrc.Y >= 0 && !Eliot.gravity && Eliot.Y>=y)
                        {

                            Eliot.Y -= 20;
                            Eliot.X += speed;
                            Eliot.gravity = true;
                        }
                        else if (Eliot.Y < y)
                        {
                            Eliot.Y = y;
                            Eliot.gravity = false;
                        }
                        if (Eliot.iFrame < 3)
                        {
                            if (ct % 4 == 0)
                            {
                                Eliot.iFrame++;

                            }
                            ct++;
                        }
                        else
                        {
                            Eliot.iFrame = 1;
                        }
                    }
                    break;

            }
            movegun();
            DrawDubb(this.CreateGraphics());
        }
        void movegun()
        {
            switch (igun)
            {
                case 0:
                    gun[0].x = Eliot.X + Eliot.frames[0].Width - 17;
                    break;
                    
                case 1:
                    gun[1].x = Eliot.X - 30;
                    break;
            }
            gun[igun].y = Eliot.Y + 15;
        }
        void create_world()
        {
            wrld.wrld = new Bitmap("Background/529660ac1a23f254ab7edacd8336809a9f3106f9c2571c08edba961e468b7bc5.jpg"); // we will change the image later
            wrld.rcDst = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
            wrld.rcSrc = new Rectangle(0, wrld.wrld.Height - (wrld.wrld.Height / 2), wrld.wrld.Width / 3, wrld.wrld.Height - (wrld.wrld.Height / 2));
            //temp = new Rectangle(0, wrld.wrld.Height - (wrld.wrld.Height / 2), 0, wrld.wrld.Height - (wrld.wrld.Height / 2));
            //temp2 = new Rectangle(0, 0, 0, ClientSize.Height);
        }
        void create_gun()
        {
            CImageActor pnn = new CImageActor();
            pnn.x = Eliot.X + Eliot.frames[0].Width - 17;
            pnn.y = Eliot.Y + 15;
            pnn.img = new Bitmap("guns/gun.jpeg");
            pnn.img.MakeTransparent(pnn.img.GetPixel(0, 0));
            gun.Add(pnn);

            pnn = new CImageActor();
            pnn.y = Eliot.Y + 15;
            pnn.img = new Bitmap("guns/gun2.jpeg");
            pnn.x = Eliot.X - 30;
            pnn.img.MakeTransparent(pnn.img.GetPixel(0, 0));
            gun.Add(pnn);
        }
        void create_Hero()
        {
            Eliot.frames = new List<Bitmap>();
            Eliot.X = 150;
            Eliot.Y = 260;
            //if Hero walks forward state =0
            Bitmap img = new Bitmap("Heros/Frames/walk/HeroW1.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Eliot.frames.Add(img);
            img = new Bitmap("Heros/Frames/walk/HeroW2.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Eliot.frames.Add(img);
            img = new Bitmap("Heros/Frames/walk/HeroW3.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Eliot.frames.Add(img);
            img = new Bitmap("Heros/Frames/walk/HeroW4.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Eliot.frames.Add(img);
            //if Hero walks back state =1
            img = new Bitmap("Heros/Frames/walk/HeroW1rev.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Eliot.frames.Add(img);
            img = new Bitmap("Heros/Frames/walk/HeroW2rev.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Eliot.frames.Add(img);
            img = new Bitmap("Heros/Frames/walk/HeroW3rev.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Eliot.frames.Add(img);
            img = new Bitmap("Heros/Frames/walk/HeroW4rev.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Eliot.frames.Add(img);
            //if hero jumps(bases odam) hero state =2 

            // if hero jumps(bases wara) hero state =3
            Eliot.state = 0;
            Eliot.iFrame = 0;


        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawDubb(e.Graphics);
        }
        void DrawScene(Graphics g)
        {
            g.Clear(Color.White);
            g.DrawImage(wrld.wrld, wrld.rcDst, wrld.rcSrc, GraphicsUnit.Pixel);
            //if (wrld.rcSrc.X + wrld.rcSrc.Width > wrld.wrld.Width)//sa7
            //{
            //    temp.Width = -(wrld.wrld.Width - (wrld.rcSrc.X + wrld.rcSrc.Width));
            //    temp2.Width = temp.Width * 2;
            //    temp2.X = ClientSize.Width - temp2.Width;
            //    g.DrawImage(wrld.wrld, temp2, temp, GraphicsUnit.Pixel);
            //    if (temp2.Width == ClientSize.Width)
            //    {
            //        wrld.rcDst = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
            //        wrld.rcSrc = new Rectangle(0, wrld.wrld.Height - (wrld.wrld.Height / 2), wrld.wrld.Width / 3, wrld.wrld.Height - (wrld.wrld.Height / 2));
            //    }
            //}
            g.DrawImage(Eliot.frames[Eliot.iFrame], Eliot.X, Eliot.Y);
            g.DrawImage(gun[igun].img, gun[igun].x, gun[igun].y, 50, 50);

        }
        void DrawDubb(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }
    }
}

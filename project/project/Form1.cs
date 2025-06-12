using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
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
        public bool islife;
        public int dir;
        public int W, H;
    }
    public partial class Form1 : Form
    {
        Bitmap off;
        Timer tt = new Timer();
        CAdvImgActor wrld = new CAdvImgActor();
        CMultiImgActor Eliot = new CMultiImgActor();
        CImageActor singleBullet = new CImageActor();
        List<CImageActor> gun = new List<CImageActor>();
        List<CImageActor> bullets = new List<CImageActor>();
        List<CImageActor> fixedObjects = new List<CImageActor>();
        List<CImageActor> fixedwalls = new List<CImageActor>();
        List<CMultiImgActor> enemies = new List<CMultiImgActor>();
        int ct = 0;
        int ctTick = 0;
        Bitmap iff;
        int ctjump = 0;
        int y = 260;
        int speed = 10;
        int speed2 = 10;
        int igun;//0->right , //1->left
        CImageActor pnn2 = new CImageActor();
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
            create_Tiles();
            Create_Enemy();
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
            enemyhandle();
            move_enemy();
            movegun();
            MoveBullets();
            this.Text = enemies[0].iFrame.ToString();
            Console.WriteLine(enemies[0].iFrame.ToString());
            CheckB();
            ctTick++;
            DrawDubb(this.CreateGraphics());

        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode) //scrolling logic
            {
                case Keys.D:
                    Eliot.state = 0;
                    if (wrld.rcSrc.X + wrld.rcSrc.Width <= wrld.wrld.Width - 3 && Eliot.X >= (ClientSize.Width / 2) - 100)
                    {
                         speed = 0;
                         wrld.rcSrc.X += 6;
                    }
                    else
                    {
                        speed = 6;
                    }
                    if (Eliot.X  <= ClientSize.Width - Eliot.frames[Eliot.iFrame].Width)
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
                            Eliot.iFrame = 0;
                        }
                    }
                    break;

                case Keys.A:
                    if(Eliot.state==0)
                    {
                        Eliot.iFrame = 4;

                    }
                    Eliot.state = 1;
                    if (wrld.rcSrc.X > 3)
                    {
                        if (Eliot.X <= ClientSize.Width / 2)
                        {
                            wrld.rcSrc.X -= 6;
                            speed2 = 0;
                        }

                    }
                    else
                    {
                        speed2 = 6;
                    }
                    if (Eliot.X >= 0)
                    {
                        Eliot.X -= speed2;
                    }
                    
                    if (Eliot.X >= 0)
                    { // Eliot.X += 2;
                        
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
                    igun = 1;
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
                    igun = 0;
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
                    case Keys.Space:
                        Create_Bullets();
                    break;
                case Keys.X:
                    if(!singleBullet.islife)
                    {
                        Create_SB(); 
                    }
                    break;
                case Keys.L:
                    Eliot.Y -= 10;
                    wrld.rcSrc.Y -= 15;
                    break;
            }
            movegun();
            MoveBullets();
            CheckB();
            move_enemy();
            enemyhandle();
            DrawDubb(this.CreateGraphics());
        }
        void Create_Enemy()
        {
            CMultiImgActor pnn = new CMultiImgActor();
            pnn.frames = new List<Bitmap>();
            for(int i = 0;i<8;i++)
            {
                Bitmap img = new Bitmap("enemies/ship_frames/" +(i + 1) + ".png");
                img.MakeTransparent(img.GetPixel(0, 0));
                pnn.frames.Add(img);
            }
            pnn.X = 5+ wrld.rcSrc.X;
            pnn.Y = -150;
            pnn.Dir = 1;
            enemies.Add(pnn);
        }
        void enemyhandle()
        {
            if (enemies[0].X >= 400 + wrld.rcSrc.X || enemies[0].X<=5+wrld.rcSrc.X)
            {
                enemies[0].Dir *= -1;
            }
            if(enemies[0].X <= 5 + wrld.rcSrc.X || enemies[0].iFrame == 3)
            {
                enemies[0].iFrame = 0; 
            }
            if(enemies[0].X >= 400 + wrld.rcSrc.X || enemies[0].iFrame == 7)
            {
                enemies[0].iFrame = 4;
            }
        }
        void move_enemy()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (ctTick % 5 == 0)
                {
                    enemies[i].iFrame++;
                }
                else
                {
                    enemies[i].X += (enemies[i].Dir * 5);
                }
            }
        }
        void CheckB()
        {
            int XS = wrld.rcSrc.X * 19 / 10;
            int YS = wrld.rcSrc.Y * 19 / 10;
            
            for (int i = 0; i < bullets.Count; i++)
            {
                CImageActor ptrav = bullets[i];
                if (ptrav.x >= ClientSize.Width || ptrav.x < -10)
                {
                    bullets.Remove(ptrav);
                }
                for (int j = 0; j < enemies.Count; j++)
                {
                    CMultiImgActor enemy = enemies[j];
                    if ((ptrav.x >= enemy.X - XS && (ptrav.y <= enemy.Y - YS && ptrav.y >= y) && ptrav.dir == 1)
                        || (ptrav.x <= enemy.X - XS && (ptrav.y <= enemy.Y - YS && ptrav.y >= y) && ptrav.dir == -1)) 
                    {
                        bullets.Remove(ptrav);
                        break;
                    }
                }
                for (int j = 0; j < fixedObjects.Count; j++)
                {
                    CImageActor fixedObj = fixedObjects[j];
                    if ((ptrav.x >= fixedObj.x -XS && (ptrav.y <= fixedObj.y - YS && ptrav.y >= y) && ptrav.dir == 1)
                        || (ptrav.x <= fixedObj.x - XS  && (ptrav.y <= fixedObj.y - YS && ptrav.y >= y) && ptrav.dir == -1))
                    {
                        bullets.Remove(ptrav);
                        break;
                    }
                }
            }
            if (singleBullet.islife)
            {
                CImageActor ptrav = singleBullet;
                if (ptrav.x >= ClientSize.Width || ptrav.x < -10)
                {
                    singleBullet.islife = false;
                }
                for (int j = 0; j < enemies.Count; j++)
                {
                    CMultiImgActor enemy = enemies[j];
                    if ((ptrav.x >= enemy.X - XS && (ptrav.y <= enemy.Y - YS && ptrav.y >= y) && ptrav.dir == 1)
                        || (ptrav.x <= enemy.X - XS && (ptrav.y <= enemy.Y - YS && ptrav.y >= y) && ptrav.dir == -1))
                    {
                        singleBullet.islife = false;
                        break;
                    }
                }
                for (int j = 0; j < fixedObjects.Count; j++)
                {
                    CImageActor fixedObj = fixedObjects[j];
                    if ((ptrav.x >= fixedObj.x - XS && (ptrav.y <= fixedObj.y - YS && ptrav.y >= y) && ptrav.dir == 1)
                        || (ptrav.x <= fixedObj.x - XS && (ptrav.y <= fixedObj.y - YS && ptrav.y >= y) && ptrav.dir == -1))
                    {
                        singleBullet.islife = false;
                        break;
                    }
                }
            }
        }
        void Create_SB()
        {
            switch (igun)
            {
                case 0:
                    singleBullet.x = gun[0].x + 50;
                    singleBullet.y = gun[0].y-15;
                    singleBullet.img = new Bitmap("guns/bulletS.png");
                    singleBullet.dir = 1;
                    break;
                case 1:
                    singleBullet.x = gun[1].x - 10;
                    singleBullet.y = gun[1].y-15;
                    singleBullet.img = new Bitmap("guns/bulletS2.png");
                    singleBullet.dir = -1;
                    break;
            }
            singleBullet.islife = true;
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
        void Create_Bullets()
        {
            CImageActor pnn = new CImageActor();
            switch (igun)
            {
                case 0:
                    pnn.x = gun[0].x + 50;
                    pnn.y = gun[0].y;
                    pnn.img = new Bitmap("guns/bullet.png");
                    pnn.dir = 1;
                    break;
                case 1:
                    pnn.x = gun[1].x - 10;
                    pnn.y = gun[1].y;
                    pnn.img = new Bitmap("guns/bullet2.png");
                    pnn.dir = -1;
                    break;
            }
            pnn.islife = true;
            bullets.Add(pnn);
        }
        void MoveBullets()
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].x += (10 * bullets[i].dir);
            }
            if (singleBullet.islife)
            {
                singleBullet.x += (10 * singleBullet.dir);
            }
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
            img = new Bitmap("Heros/Frames/jump/Hero1.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Eliot.frames.Add(img);
            img = new Bitmap("Heros/Frames/jump/Hero2.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Eliot.frames.Add(img);
            img = new Bitmap("Heros/Frames/jump/Hero3.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Eliot.frames.Add(img);
            img = new Bitmap("Heros/Frames/jump/Hero4.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Eliot.frames.Add(img);
            // if hero jumps(bases wara) hero state =3
            img = new Bitmap("Heros/Frames/jump/Hero1rev.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Eliot.frames.Add(img);
            img = new Bitmap("Heros/Frames/jump/Hero2rev.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Eliot.frames.Add(img);
            img = new Bitmap("Heros/Frames/jump/Hero3rev.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Eliot.frames.Add(img);
            img = new Bitmap("Heros/Frames/jump/Hero4rev.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Eliot.frames.Add(img);

            Eliot.state = 0;
            Eliot.iFrame = 0;


        }
        void create_Tiles()
        {
            CImageActor pnn = new CImageActor();
            pnn.img = new Bitmap("Assets/Screenshot 2025-06-11 002908.png");
            pnn.W = 160;
            pnn.H = 40;
            pnn.x = 850 + wrld.rcSrc.X;
            pnn.y = 465 + wrld.rcSrc.Y;
            fixedObjects.Add(pnn);
            for (int i = 0; i < 3; i++)
            {
                pnn = new CImageActor();
                pnn.img = new Bitmap("Assets/Screenshot 2025-06-11 002908.png");
                pnn.W = 160;
                pnn.H = 40;
                pnn.x = fixedObjects[fixedObjects.Count - 1].x + pnn.W;
                pnn.y = fixedObjects[fixedObjects.Count - 1].y;
                fixedObjects.Add(pnn);
            }
            pnn = new CImageActor();
            pnn.img = new Bitmap("Assets/Screenshot 2025-06-11 002908.png");
            pnn.W = 160;
            pnn.H = 40;
            pnn.x = fixedObjects[fixedObjects.Count - 1].x + pnn.W * 2;
            pnn.y = fixedObjects[fixedObjects.Count - 1].y;
            fixedObjects.Add(pnn);
            for (int i = 0; i < 4; i++)
            {
                pnn = new CImageActor();
                pnn.img = new Bitmap("Assets/Screenshot 2025-06-11 002908.png");
                pnn.W = 160;
                pnn.H = 40;
                pnn.x = fixedObjects[fixedObjects.Count - 1].x + pnn.W;
                pnn.y = fixedObjects[fixedObjects.Count - 1].y;
                fixedObjects.Add(pnn);
            }
            pnn = new CImageActor();
            pnn.img = new Bitmap("Assets/Screenshot 2025-06-11 002908.png");
            pnn.W = 160;
            pnn.H = 40;
            pnn.x = fixedObjects[fixedObjects.Count - 1].x - 30;
            pnn.y = fixedObjects[fixedObjects.Count - 1].y - pnn.H;
            fixedObjects.Add(pnn);

            for (int i = 0; i < 4; i++)
            {
                pnn = new CImageActor();
                pnn.img = new Bitmap("Assets/Screenshot 2025-06-11 002908.png");
                pnn.W = 160;
                pnn.H = 40;
                pnn.x = fixedObjects[fixedObjects.Count - 1].x - pnn.W * 3 / 2;
                pnn.y = fixedObjects[fixedObjects.Count - 1].y - pnn.H * 2;
                fixedObjects.Add(pnn);
            }


            for (int i = 0; i < 4; i++)
            {
                pnn = new CImageActor();
                pnn.img = new Bitmap("Assets/Screenshot 2025-06-11 002908.png");
                pnn.W = 160;
                pnn.H = 40;
                pnn.x = fixedObjects[fixedObjects.Count - 1].x - pnn.W;
                pnn.y = fixedObjects[fixedObjects.Count - 1].y;
                fixedObjects.Add(pnn);
            }

            pnn = new CImageActor();
            pnn.img = new Bitmap("Assets/Screenshot 2025-06-11 002908.png");
            pnn.W = 160;
            pnn.H = 40;
            pnn.x = fixedObjects[fixedObjects.Count - 1].x - pnn.W;
            pnn.y = fixedObjects[fixedObjects.Count - 1].y - 200;
            fixedObjects.Add(pnn);
            for (int i = 0; i < 2; i++)
            {
                pnn = new CImageActor();
                pnn.img = new Bitmap("Assets/Screenshot 2025-06-11 002908.png");
                pnn.W = 160;
                pnn.H = 40;
                pnn.x = fixedObjects[fixedObjects.Count - 1].x - pnn.W;
                pnn.y = fixedObjects[fixedObjects.Count - 1].y;
                fixedObjects.Add(pnn);
            }

            pnn = new CImageActor();
            pnn.x = wrld.wrld.Width * 18 / 10 + wrld.rcSrc.X;
            pnn.y = fixedObjects[fixedObjects.Count - 1].y - 30;
            pnn.img = new Bitmap("Assets/Screenshot 2025-06-11 002908.png");
            pnn.W = 160;
            pnn.H = 40;
            fixedObjects.Add(pnn);
            for (int i = 0; i < 3; i++)
            {
                pnn = new CImageActor();
                pnn.img = new Bitmap("Assets/Screenshot 2025-06-11 002908.png");
                pnn.W = 160;
                pnn.H = 40;
                pnn.x = fixedObjects[fixedObjects.Count - 1].x - pnn.W;
                pnn.y = fixedObjects[fixedObjects.Count - 1].y;
                fixedObjects.Add(pnn);
            }


            pnn = new CImageActor();
            pnn.x = 0 + wrld.rcSrc.X;
            pnn.y = 465 + wrld.rcSrc.Y;
            pnn.img = new Bitmap("Assets/Screenshot 2025-06-11 002908.png");
            pnn.W = 160;
            pnn.H = 40;
            fixedObjects.Add(pnn);
            for (int i = 0; i < 3; i++)
            {
                pnn = new CImageActor();
                pnn.img = new Bitmap("Assets/Screenshot 2025-06-11 002908.png");
                pnn.W = 160;
                pnn.H = 40;
                pnn.x = fixedObjects[fixedObjects.Count - 1].x + pnn.W;
                pnn.y = fixedObjects[fixedObjects.Count - 1].y;
                fixedObjects.Add(pnn);
            }

            pnn = new CImageActor();
            pnn.x = (160 * 4) + wrld.rcSrc.X;
            pnn.y = 465 + wrld.rcSrc.Y;
            pnn.img = new Bitmap("Assets/smallbrick.png");
            pnn.W = 50;
            pnn.H = 40;
            fixedwalls.Add(pnn);
            for (int i = 0; i < 9; i++)
            {
                pnn = new CImageActor();
                pnn.img = new Bitmap("Assets/smallbrick.png");
                pnn.W = 50;
                pnn.H = 40;
                pnn.x = fixedwalls[fixedwalls.Count - 1].x;
                pnn.y = fixedwalls[fixedwalls.Count - 1].y - pnn.H;
                fixedwalls.Add(pnn);
            }


        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawDubb(e.Graphics);
        }
        void DrawScene(Graphics g)
        {
            g.Clear(Color.White);
            g.DrawImage(wrld.wrld, wrld.rcDst, wrld.rcSrc, GraphicsUnit.Pixel);
            g.DrawImage(Eliot.frames[Eliot.iFrame], Eliot.X, Eliot.Y);
            g.DrawImage(gun[igun].img, gun[igun].x, gun[igun].y, 50, 50);
            int XS= wrld.rcSrc.X *19/10;
            int YS= wrld.rcSrc.Y*19/10;
            for (int i=0;i<bullets.Count;i++)
            {
                    g.DrawImage(bullets[i].img, bullets[i].x, bullets[i].y, 50, 50);
            }
            for(int i=0;i<fixedObjects.Count;i++)
            {
                g.DrawImage(fixedObjects[i].img, fixedObjects[i].x - XS, fixedObjects[i].y - YS, fixedObjects[i].W, fixedObjects[i].H);
            }
            for (int i = 0; i < fixedwalls.Count; i++)
            {
                g.DrawImage(fixedwalls[i].img, fixedwalls[i].x - XS, fixedwalls[i].y - YS, fixedwalls[i].W, fixedwalls[i].H);
            }
            if(singleBullet.islife)
            {
                g.DrawImage(singleBullet.img, singleBullet.x, singleBullet.y,70,70);
            }
            for(int i=0;i<enemies.Count;i++)
            {
                CMultiImgActor ptrav = enemies[i];
                g.DrawImage(ptrav.frames[ptrav.iFrame], ptrav.X - XS, ptrav.Y - YS);
            }
        }
        void DrawDubb(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }
    }
}

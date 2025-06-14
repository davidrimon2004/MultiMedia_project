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
using System.Xml.Linq;

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
        public int HP;
        public int W, H;
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
        List<CImageActor> gun = new List<CImageActor>();
        List<CImageActor> bullets = new List<CImageActor>();
        List<CImageActor> fixedObjects = new List<CImageActor>();
        List<CImageActor> fixedwalls = new List<CImageActor>();
        List<CMultiImgActor> bomb = new List<CMultiImgActor>();
        List<CMultiImgActor> enemies = new List<CMultiImgActor>();
        CImageActor singleBullet = new CImageActor();
        CImageActor Elevator = new CImageActor();
        CMultiImgActor Laser=new CMultiImgActor();
        CMultiImgActor HP=new CMultiImgActor();
        CMultiImgActor Boss=new CMultiImgActor();
        CMultiImgActor enemy2 = new CMultiImgActor();
        CImageActor ladder=new CImageActor();
        int ct = 0;
        int ctTick = 0;
        Bitmap iff;
        int ctjump = 0;
        int y = 260;
        int speed = 40;
        int speed2 = 40;
        int igun;//0->right , //1->left
        CImageActor pnn2 = new CImageActor();
        int ctLaser;
        bool laserdmg = false;
        int dirElevtorX;
        bool elevator;
        bool elevator2;
        int oldHP = 5,cttickDMG=0;
        bool flagjump = false;
        bool bossdmg = false;
        bool ladderFlag = false;
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
            y = Eliot.Y + wrld.rcSrc.Y;
            Create_Enemy();
            Create_Elevator();
            create_laser();
            create_HPBAr();
            createBoss();
            create_enemy2();
            CreateLadder();
        }
        private void Tt_Tick(object sender, EventArgs e)
        {
           
            if (Eliot.gravity&& flagjump)
            {

                if (Eliot.state == 2)
                {
                    Eliot.iFrame = 7;
                }
                if (Eliot.state == 3)
                {
                    Eliot.iFrame = 11;
                }
                if (Eliot.state == 2 || Eliot.state == 3)
                {
                    ctjump++;
                    if (ctjump < 6)
                    {
                        Eliot.Y -= 5;
                        if (Eliot.state == 2)
                        {
                            if (Eliot.iFrame < 12)
                            {
                                Eliot.iFrame++;
                            }
                            else
                            {
                                Eliot.iFrame = 8;
                            }
                        }
                        else if (Eliot.state == 3)
                        {
                            if (Eliot.iFrame < 16)
                            {
                                Eliot.iFrame++;
                            }
                            else
                            {
                                Eliot.iFrame = 12;
                            }
                        }
                        wrld.rcSrc.Y -= 6;
                    }
                    else if (ctjump < 11)
                    {

                        Eliot.Y += 5;
                        if (Eliot.state == 2)
                        {
                            if (Eliot.iFrame < 11)
                            {
                                Eliot.iFrame++;
                            }
                            else
                            {
                                Eliot.iFrame = 8;
                            }
                        }
                        else if (Eliot.state == 3)
                        {
                            if (Eliot.iFrame < 15)
                            {
                                Eliot.iFrame++;
                            }
                            else
                            {
                                Eliot.iFrame = 12;
                            }
                        }
                        wrld.rcSrc.Y += 6;
                    }
                    if (ctjump >=10)
                    {
                        if (Eliot.state == 2)
                        {
                            Eliot.state = 0;
                            Eliot.iFrame = 0;
                        }
                        else if (Eliot.state == 3)
                        {
                            Eliot.state = 1;
                            Eliot.iFrame = 4;
                        }
                        Eliot.gravity = false;
                        flagjump = false;
                        ctjump = 0;
                    }
                }
            }
           

            if (Eliot.state%2==0)
            {
                igun = 0;
            }
            else
            {
                igun = 1;
            }

            if (ctTick % 30 == 0)
            {
                Create_Bomb();
            }
            if (Eliot.HP < oldHP)
            {
                //for (int i = 0; i < 3; i++)
                //{
                animateDMG();
                //}
                oldHP--;
            }
            animateLaser();
            movegun();
            MoveBullets();
            this.Text = bullets.Count.ToString();
            enemyhandle();
            move_enemy();
            CheckB();
            animateBoss();
            AnimateEnemy2();
            Handle_bomb();
            ElevatorHandle();
            ctTick++;
            DrawDubb(this.CreateGraphics());
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            int YS = wrld.rcSrc.Y;
            int XS = wrld.rcSrc.X;
            LadderHandle();
            if (!elevator)
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
                                    Eliot.iFrame = 0;
                                }
                            }
                            break;

                        case Keys.A:
                            if (Eliot.state == 0)
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
                            {
                                Eliot.X -= 2;

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
                        flagjump = true;
                        ctjump = 0;
                        if (!Eliot.gravity && !ladderFlag)
                        {
                            //if (wrld.rcSrc.Y >= 0 && !Eliot.gravity && Eliot.Y >= y - YS)
                            //{
                            //    //wrld.rcSrc.Y -= 20;
                            //    //Eliot.Y -= 50;
                            //    Eliot.gravity = true;
                            //}

                            if (!Eliot.gravity)
                            {
                                if (Eliot.state == 0)
                                {
                                    Eliot.state = 2;
                                }
                                if (Eliot.state == 1)
                                {
                                    Eliot.state = 3;
                                }
                                Eliot.gravity = true;

                            }
                            else if (Eliot.Y < y - YS)
                            {
                                //Eliot.Y = y - YS;
                                //wrld.rcSrc.Y += 20;
                                // Eliot.gravity = false;
                            }
                        }
                        else if (ladderFlag)
                        {
                            Eliot.Y -= 4;
                            wrld.rcSrc.Y -= 4;
                            if (Eliot.state == 0)
                            {
                                Eliot.iFrame = 8;
                            }
                            else if (Eliot.state == 1)
                            {
                                Eliot.iFrame = 12;
                            }
                        }

                        break;

                    case Keys.S:

                            if (Eliot.Y < y - YS)
                            {
                                Eliot.Y += 2;

                            }

                            if (wrld.rcSrc.Y + wrld.rcSrc.Height <= wrld.rcDst.Height)
                                wrld.rcSrc.Y += 2;
                            break;

                        case Keys.Q:
                            igun = 1;
                            if (wrld.rcSrc.Y >= 0 && !Eliot.gravity && Eliot.Y >= y - YS)
                            {

                                Eliot.Y -= 50;
                                wrld.rcSrc.Y -= 20;
                                Eliot.gravity = true;
                            }
                            else if (Eliot.Y < y - YS)
                            {
                                Eliot.Y = y - YS;
                                wrld.rcSrc.Y += 20;
                                Eliot.gravity = false;
                            }
                            if (wrld.rcSrc.X >= 0)
                            {
                                if (Eliot.X >= ClientSize.Width / 2 && Eliot.Y <= y - YS)
                                {
                                    speed2 = 40;

                                }
                                else
                                {
                                    if (wrld.rcSrc.X > 80)
                                    {
                                        wrld.rcSrc.X -= 80;
                                        speed2 = 0;
                                    }
                                    else if (Eliot.X > 40)
                                    {
                                        speed2 = 40;
                                    }
                                    else
                                    {
                                        speed2 = 0;
                                    }
                                }

                            }

                            if (Eliot.X >= 0)
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
                            if (wrld.rcSrc.X + wrld.rcSrc.Width <= wrld.wrld.Width && Eliot.X >= (ClientSize.Width / 2) - 100 && Eliot.Y >= y - YS)
                            {
                                speed = 0;
                                wrld.rcSrc.X += 80;
                            }
                            else
                            {
                                speed = 40;
                            }
                            if (Eliot.X <= ClientSize.Width - Eliot.frames[Eliot.iFrame].Width)
                            {
                                // Eliot.X += 2;
                                if (wrld.rcSrc.Y >= 0 && !Eliot.gravity && Eliot.Y >= y - YS)
                                {
                                    wrld.rcSrc.Y -= 20;
                                    Eliot.Y -= 50;
                                    Eliot.X += speed;
                                    Eliot.gravity = true;
                                }
                                else if (Eliot.Y < y - YS)
                                {
                                    Eliot.Y = y - YS;
                                    wrld.rcSrc.Y += 20;
                                    Eliot.gravity = false;
                                }
                                if (Eliot.X <= ClientSize.Width && Eliot.Y >= y - YS && !Eliot.gravity)
                                {
                                    Eliot.X += speed;

                                }

                                if (!Eliot.gravity)
                                {
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
                            }
                            break;

                        case Keys.Space:
                            Create_Bullets();
                            break;
                        case Keys.X:
                            if (!singleBullet.islife)
                            {
                                Create_SB();
                            }
                            break;
                        case Keys.L:
                            Eliot.Y -= 10;
                            wrld.rcSrc.Y -= 10;
                            break;
                    }
                
            }
            Console.WriteLine(speed2.ToString());
            movegun();
            MoveBullets();
            animateLaser();
            LaserDMG();
            CheckB();
            enemyhandle();
            if (Eliot.HP < oldHP)
            {
                for (int i = 0; i < 3; i++)
                { animateDMG(); }
                oldHP--;
            }
            move_enemy();
            Handle_bomb();
            ElevatorHandle();

            DrawDubb(this.CreateGraphics());
        }
        void animateDMG()
        {
            //MessageBox.Show(Eliot.frames.Count.ToString());
            for (int i = 0; i < 4; i++)
            {
                Eliot.iFrame = 16 + i;
            }
        }
        void Create_Enemy()
        {
            CMultiImgActor pnn = new CMultiImgActor();
            pnn.frames = new List<Bitmap>();
            for (int i = 0; i < 8; i++)
            {
                Bitmap img = new Bitmap("enemies/ship_frames/" + (i + 1) + ".png");
                img.MakeTransparent(img.GetPixel(0, 0));
                pnn.frames.Add(img);
            }
            pnn.X = 5 + wrld.rcSrc.X;
            pnn.Y = -150;
            pnn.Dir = 1;
            enemies.Add(pnn);
        }
        void enemyhandle()
        {
            int XS = wrld.rcSrc.X ;
            if (enemies[0].X >= 400  || enemies[0].X <= 5 )
            {
                enemies[0].Dir *= -1;
            }
            if (enemies[0].X <= 5  || enemies[0].iFrame == 3)
            {
                enemies[0].iFrame = 0;
            }
            if (enemies[0].X >= 400 || enemies[0].iFrame == 7)
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
        void createBoss()
        {
            Boss.frames = new List<Bitmap>();
            Boss.iFrame = 0;
            //boss state =0 idle
            Boss.state = 0;
            Bitmap img = new Bitmap("enemies/Boss/idle/frame_092_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0,0));
            Boss.frames.Add(img);
            img = new Bitmap("enemies/Boss/idle/frame_093_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/idle/frame_094_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/idle/frame_095_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/idle/frame_096_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/idle/frame_097_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/idle/frame_098_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/idle/frame_099_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/idle/frame_100_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/idle/frame_101_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/idle/frame_102_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/idle/frame_103_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/idle/frame_104_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/idle/frame_105_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/idle/frame_106_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/idle/frame_107_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img);
            img = new Bitmap("enemies/Boss/idle/frame_108_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/idle/frame_109_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img);
            //this part ends at i=17

            //boss state = 1 shoots when Eliot is close

            img = new Bitmap("enemies/Boss/shooting/frame_072_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img);
            img = new Bitmap("enemies/Boss/shooting/frame_073_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img);
            img = new Bitmap("enemies/Boss/shooting/frame_074_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img);
            img = new Bitmap("enemies/Boss/shooting/frame_075_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img);
            img = new Bitmap("enemies/Boss/shooting/frame_076_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img);
            img = new Bitmap("enemies/Boss/shooting/frame_077_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img);
            img = new Bitmap("enemies/Boss/shooting/frame_090_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img);
            img = new Bitmap("enemies/Boss/shooting/frame_091_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img);
            // this part ends at i=25

            //state=2 boss is getting shot at
            img = new Bitmap("enemies/Boss/shot/frame_017_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img);
            img = new Bitmap("enemies/Boss/shot/frame_018_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/shot/frame_019_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/shot/frame_020_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/shot/frame_021_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/shot/frame_022_delay-0.08s.gif");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/shot/frame_023_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/shot/frame_024_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/shot/frame_025_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/shot/frame_026_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/shot/frame_027_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/shot/frame_028_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/shot/frame_029_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/shot/frame_030_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/shot/frame_031_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/shot/frame_032_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            img = new Bitmap("enemies/Boss/shot/frame_034_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/shot/frame_035_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/shot/frame_036_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/shot/frame_037_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/shot/frame_038_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/shot/frame_039_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img);
            img = new Bitmap("enemies/Boss/shot/frame_040_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img);
            // this ends at i= 47

            //state =3 Boss is defeated
            img = new Bitmap("enemies/Boss/K.O/frame_164_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img);
            img = new Bitmap("enemies/Boss/K.O/frame_165_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/K.O/frame_166_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/K.O/frame_167_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/K.O/frame_168_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/K.O/frame_169_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/K.O/frame_170_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/K.O/frame_171_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/K.O/frame_172_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/K.O/frame_173_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/K.O/frame_174_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img); img = new Bitmap("enemies/Boss/K.O/frame_175_delay-0.08s.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Boss.frames.Add(img);
            //this ends at i=59;
            //list.Count should be 60
            Boss.X = 1770 + wrld.rcSrc.Width;
            Boss.Y=435+wrld.rcSrc.Height;
            Boss.H = 270;
            Boss.W = 270;
            Boss.HP = 4;
        }
        void animateBoss()
        {
            int XS = wrld.rcSrc.X * 19 / 10;
            int YS = wrld.rcSrc.Y * 19 / 10;
            if (ctTick % 2 == 0)
            {
                if (Boss.state == 0)
                {
                    Boss.iFrame++;
                    if (Boss.iFrame > 17)
                    {
                        Boss.iFrame = 0;
                    }
                }
                if (Boss.state == 1)
                {
                    if (Boss.iFrame < 18 || Boss.iFrame > 25)
                    {
                        Boss.iFrame = 18;
                    }
                    else
                    {
                        Boss.iFrame++;
                    }
                }
                if (Boss.state == 2)
                {
                    if (Boss.iFrame < 25 || Boss.iFrame > 47)
                    {
                        Boss.iFrame = 25;
                    }
                    else
                    {
                        Boss.iFrame++;
                    }
                }
                if (Boss.state == 3)
                {
                    if (Boss.iFrame < 47)
                    {
                        Boss.iFrame = 48;
                    }
                    else if(Boss.iFrame<Boss.frames.Count-1)
                    {
                        Boss.iFrame++;
                    }
                }
            }
            if(Eliot.X>Boss.X-XS-300 && Eliot.Y>Boss.Y-YS&& Boss.state!=3)
            {
                Boss.state = 1;
                if(!Eliot.gravity&&Boss.iFrame==24)
                {
                    Eliot.HP--;
                }
            }
            else if(Eliot.X<Boss.X-XS-300 && Boss.state != 3)
            {
                Boss.state = 0;
            }
            for(int i=0;i<bullets.Count;i++)
            {
                if (bullets[i].x > Boss.X - XS  && bullets[i].y>Boss.Y-YS && Boss.state != 3)
                {
                    Boss.state = 2;
                    bossdmg = true;
                    if(bossdmg==true)
                    {
                        Boss.HP--;
                        bossdmg=false;
                    }
                }
            }
            if(singleBullet.x>Boss.X-XS&&singleBullet.y>Boss.Y-YS && Boss.state != 3)
            {
                Boss.state = 2;
                bossdmg = true;
                if (bossdmg == true)
                {
                    Boss.HP--;
                    bossdmg = false;
                }
            }
            if (Boss.HP == 0)
            {
                Boss.state = 3;
            }

        }
        void create_enemy2()
        { 
            enemy2.frames = new List<Bitmap>();
            Bitmap img;
            for (int i = 1; i < 10; i++)
            {
                img = new Bitmap("enemies/enemy2/" + i + ".png");
                img.MakeTransparent(img.GetPixel(0, 0));
                enemy2.frames.Add(img);
            }
            enemy2.state = 0;
            enemy2.X = 500 + wrld.rcSrc.Width;
            enemy2.Y= -50+wrld.rcSrc.Height;
            enemy2.W = 200;
            enemy2.H = 200;
            //state 0 ,,,,, still
            //state 1 ......walking
            //state 2 still and shooting
        }
        void AnimateEnemy2()
        {
            int XS = wrld.rcSrc.X * 19 / 10;
            int YS = wrld.rcSrc.Y * 19 / 10;
            if (Eliot.X < enemy2.X + 400 )
            {
                enemy2.state = 1;
            }
            else if(Eliot.X>enemy2.X-400)
            {
                enemy2.state = 2;
            }
            if (ctTick % 3 == 0)
            {
                if (enemy2.state == 1)
                {
                    enemy2.iFrame++;
                    if (enemy2.iFrame > 8)
                    {
                        enemy2.iFrame = 0;
                    }
                    if (enemy2.X > 600)
                    {
                        enemy2.state = 0;
                    }
                    enemy2.X += 10;
                }
                if (enemy2.state == 2)
                {
                    enemy2.iFrame++;
                    if (enemy2.iFrame > 8)
                    {
                        enemy2.iFrame = 0;
                    }
                    if (enemy2.X < 500)
                    {
                        enemy2.state = 0;

                    }
                    enemy2.X -= 10;
                }
            }
        }
        void CreateLadder()
        {
            ladder.img = new Bitmap("Ladders/ladder.png");
            ladder.y = 450 + wrld.rcSrc.Y;
            ladder.x = 720 + wrld.rcSrc.X;
            ladder.W = 200;
            ladder.H = 220;
        }
        void LadderHandle()
        {
            int YS = wrld.rcSrc.Y * 19 / 10;
            int XS = wrld.rcSrc.X * 19 / 10;
            if ((Eliot.X >= ladder.x - XS && Eliot.X <= ladder.x + ladder.W - XS)
                && Eliot.Y + Eliot.frames[0].Height - 20 >= ladder.y - YS)
            {
                ladderFlag = true;

            }
            else
            {
                ladderFlag = false;
                y = Eliot.Y;
            }
            //Console.WriteLine((Eliot.Y + Eliot.frames[0].Height-20).ToString() + "," + (ladder.y-YS).ToString());
        }
        void Create_Bomb()
        {
            CMultiImgActor pnn = new CMultiImgActor();
            pnn.frames= new List<Bitmap>();

            Bitmap img = new Bitmap("enemies/ship_frames/Bomb/bomb.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            pnn.frames.Add(img);

            img = new Bitmap("enemies/ship_frames/Bomb/bomb2.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            pnn.frames.Add(img);

            pnn.X = enemies[0].X + enemies[0].frames[0].Width / 2   ;
            pnn.Y = enemies[0].Y + enemies[0].frames[0].Height / 2 ;
            bomb.Add(pnn);
        }
        void Handle_bomb()
        {
            for (int i = 0; i < bomb.Count; i++)
            {
                if (bomb[i].Y<= 200)
                {
                     bomb[i].Y += 3;
                }
                else
                {
                    bomb.RemoveAt(i);
                    break;
                }
                if (bomb[i].Y>=197)
                {
                    bomb[i].iFrame = 1;
                }
                int XS = wrld.rcSrc.X * 19 / 10;
                int YS = wrld.rcSrc.Y * 19 / 10;
                if (Eliot.X <= bomb[i].X - XS + 80 && Eliot.X >= bomb[i].X - 20 - XS &&   Eliot.Y <= bomb[i].Y +60 - YS)
                {
                    
                    if (Eliot.state == 0)
                    {
                        Eliot.X -= 100;
                        Eliot.HP--;
                    }
                    else
                    {
                        Eliot.X += 100;
                        Eliot.HP--;

                    }
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
        void animateLaser()
        {
            if (Laser.state == 1)
            {
                if (ctTick % 2 == 0)
                {
                    if (Laser.iFrame <= 6)
                    {
                        Laser.iFrame++;
                    }
                    else if (Laser.iFrame == 7)
                    {
                        Laser.iFrame=0;
                    }
                }
               
            }
            else if(ctLaser==100 && Laser.state == 0)
            {
                Laser.state = 1;
                ctLaser = 0;
            }
            ctLaser++;
            if (ctLaser==100 && Laser.state==1)
            {
                ctLaser = 0;
                Laser.state = 0;
                laserdmg=false;
            }
        }
        void LaserDMG()
        {
            int XS = wrld.rcSrc.X * 19 / 10;
            int YS = wrld.rcSrc.Y * 19 / 10;
            if (Eliot.X <= Laser.X - XS + 80 && Eliot.X >= Laser.X - 20 - XS && Laser.state == 1 && laserdmg == false && Eliot.Y >=Laser.Y-YS)
            {
               // MessageBox.Show("I'm hit");
                laserdmg = true;
                if (Eliot.state==0)
                {
                    Eliot.X -= 100;
                    Eliot.HP--;
                    laserdmg = false;
                }
                else 
                {
                    Eliot.X += 100;
                    Eliot.HP--;
                    laserdmg = false;
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
        void create_laser()
        {
            Laser.frames = new List<Bitmap>();
            Bitmap img = new Bitmap("Laser/ezgif-splitl/frame_0_delay-0.05s.png");
            Laser.frames.Add(img);
            img = new Bitmap("Laser/ezgif-splitl/frame_1_delay-0.05s.png");
            Laser.frames.Add(img);
            img = new Bitmap("Laser/ezgif-splitl/frame_2_delay-0.05s.png");
            Laser.frames.Add(img);
            img = new Bitmap("Laser/ezgif-splitl/frame_3_delay-0.05s.png");
            Laser.frames.Add(img);
            img = new Bitmap("Laser/ezgif-splitl/frame_4_delay-0.05s.png");
            Laser.frames.Add(img);
            img = new Bitmap("Laser/ezgif-splitl/frame_5_delay-0.05s.png");
            Laser.frames.Add(img);
            img = new Bitmap("Laser/ezgif-splitl/frame_6_delay-0.05s.png");
            Laser.frames.Add(img); 
            img = new Bitmap("Laser/ezgif-splitl/frame_7_delay-0.05s.png");
            Laser.frames.Add(img);
            Laser.iFrame = 0;
            //state 1 = laser is active state 0= laser is passive
            Laser.state = 1;
            Laser.X = 1750 + wrld.rcSrc.X;
            Laser.Y = 500 +wrld.rcSrc.Y;
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
        void Create_Elevator()
        {
            Elevator.img = new Bitmap("Assets/elvator.png");
            Elevator.img.MakeTransparent(Elevator.img.GetPixel(0, 0));
            Elevator.x = 510 + wrld.rcSrc.X;
            Elevator.y = 100 + wrld.rcSrc.Y;
            Elevator.W = 150;
            Elevator.H = 90;
        }
        void ElevatorHandle()
        {
            int XS = wrld.rcSrc.X * 19/10;
            int YS = wrld.rcSrc.Y * 19/10;
            if (Elevator.x == 50 + wrld.rcSrc.X && !elevator2)
            {
                Elevator.dir = -1;
                dirElevtorX = 0;
            }
            if((Eliot.X <=Elevator.x +Elevator.W - 60 -XS && Eliot.X>=Elevator.x - XS)
                && !elevator
                && (Eliot.Y <= Elevator.y - Eliot.frames[Eliot.iFrame].Height+ 10 - YS) && !elevator2)
            {
                dirElevtorX = -1;
                Elevator.dir = 0;
                elevator = true;
                
            }
            if(Elevator.y <= 250 )
            {
                y = Eliot.Y;
                Elevator.dir = 0;
                elevator = false;
                elevator2 = true;
            }
            Elevator.x += (5 * dirElevtorX);
            Elevator.y += (4*Elevator.dir);
            if(elevator)
            {       
                Eliot.X = Elevator.x + 80 -XS;
                if(wrld.rcSrc.X > 0) 
                { 
                     wrld.rcSrc.X += (2 * dirElevtorX);
                }
                if(wrld.rcSrc.Y>0)
                {
                    wrld.rcSrc.Y += (2 * Elevator.dir);
                }
                
                Eliot.Y = Elevator.y - Eliot.frames[Eliot.iFrame].Height-YS +10;
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
            // if Eliot is taking Dmg
            img = new Bitmap("Heros/Frames/DMG/0.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Eliot.frames.Add(img);
            img = new Bitmap("Heros/Frames/DMG/1.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Eliot.frames.Add(img);
            img = new Bitmap("Heros/Frames/DMG/2.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Eliot.frames.Add(img);
            img = new Bitmap("Heros/Frames/DMG/3.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Eliot.frames.Add(img);
            img = new Bitmap("Heros/Frames/K.O/KO.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            Eliot.frames.Add(img);
            Eliot.HP = 5;
            Eliot.state = 0;
            Eliot.iFrame = 0;


        }
        void create_HPBAr()
        {
            HP.frames = new List<Bitmap>();
            Bitmap img = new Bitmap("Health bar/HP/0.png");
            HP.frames.Add (img);
            img = new Bitmap("Health bar/HP/1.png");
           
            HP.frames.Add(img); img = new Bitmap("Health bar/HP/2.png");
            HP.frames.Add(img); img = new Bitmap("Health bar/HP/3.png");
            HP.frames.Add(img); img = new Bitmap("Health bar/HP/4.png");
            HP.frames.Add(img); img = new Bitmap("Health bar/HP/5.png");
            HP.frames.Add(img);
            HP.X = this.Width - (50 + HP.frames[0].Width);
            HP.Y = 50;
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
            if (Eliot.HP >= 0 && cttickDMG <4)
            {
                int XS = wrld.rcSrc.X * 19 / 10;
                int YS = wrld.rcSrc.Y * 19 / 10;
                g.Clear(Color.White);
                g.DrawImage(wrld.wrld, wrld.rcDst, wrld.rcSrc, GraphicsUnit.Pixel);
                g.DrawImage(ladder.img, ladder.x - XS, ladder.y - YS, ladder.W, ladder.H);
                if (Eliot.HP == 0)
                {
                    g.DrawImage(Eliot.frames[Eliot.frames.Count - 1], Eliot.X, Eliot.Y + Eliot.frames[0].Height / 2);
                    cttickDMG++;
                }
                else
                { 
                    g.DrawImage(Eliot.frames[Eliot.iFrame], Eliot.X, Eliot.Y);
                    g.DrawImage(gun[igun].img, gun[igun].x, gun[igun].y, 50, 50);
                }
              
                for (int i = 0; i < bullets.Count; i++)
                {
                    g.DrawImage(bullets[i].img, bullets[i].x, bullets[i].y, 50, 50);
                }
                for (int i = 0; i < fixedObjects.Count; i++)
                {
                    g.DrawImage(fixedObjects[i].img, fixedObjects[i].x - XS, fixedObjects[i].y - YS, fixedObjects[i].W, fixedObjects[i].H);
                }
                for (int i = 0; i < fixedwalls.Count; i++)
                {
                    g.DrawImage(fixedwalls[i].img, fixedwalls[i].x - XS, fixedwalls[i].y - YS, fixedwalls[i].W, fixedwalls[i].H);
                }
                if (singleBullet.islife)
                {
                    g.DrawImage(singleBullet.img, singleBullet.x, singleBullet.y, 70, 70);
                }

                for (int i = 0; i < enemies.Count; i++)
                {
                    g.DrawImage(enemies[i].frames[enemies[i].iFrame], enemies[i].X - XS, enemies[i].Y - YS);
                }

                if (Laser.state == 1)
                {
                    g.DrawImage(Laser.frames[Laser.iFrame], Laser.X - XS, Laser.Y - YS, 130, 150);
                }

                for (int i = 0; i < bomb.Count; i++)
                {

                    g.DrawImage(bomb[i].frames[bomb[i].iFrame], bomb[i].X - XS, bomb[i].Y - YS, 70, 70);
                }
                g.DrawImage(Boss.frames[Boss.iFrame], Boss.X-XS, Boss.Y-YS, Boss.W, Boss.H);
                g.DrawImage(Elevator.img, Elevator.x - XS, Elevator.y - YS, Elevator.W, Elevator.H);
                g.DrawImage(HP.frames[Eliot.HP], HP.X, HP.Y);
                g.DrawImage(enemy2.frames[enemy2.iFrame],enemy2.X-XS,enemy2.Y-YS,enemy2.W,enemy2.H);
            }
            else if(cttickDMG==4)
            {
                this.BackColor = Color.Black;
                string text = "Game Over!";
                using (Font font = new Font("Arial", 48, FontStyle.Bold))
                using (Brush brush = new SolidBrush(Color.Red))
                {
                    SizeF textSize = g.MeasureString(text, font);
                    float x = (ClientSize.Width - textSize.Width) / 2;
                    float y = (ClientSize.Height - textSize.Height) / 2;
                    g.DrawString(text, font, brush, x, y);
                }
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

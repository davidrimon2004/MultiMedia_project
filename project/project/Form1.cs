using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
    public partial class Form1 : Form
    {
        Bitmap off;
        Timer tt= new Timer();
        CAdvImgActor wrld = new CAdvImgActor();
        public Form1()
        {
            WindowState= FormWindowState.Maximized;
            this.Load += Form1_Load;
            this.Paint += Form1_Paint;
            this.KeyDown += Form1_KeyDown;
            tt.Interval = 1000 / 60; // 60 FPS
            tt.Start();
            tt.Tick += Tt_Tick;
            InitializeComponent();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode) //scrolling logic
            {
                case Keys.D:
                    if (wrld.rcSrc.X + wrld.rcSrc.Width <= wrld.wrld.Width)
                    {
                        wrld.rcSrc.X += 2;
                    }
                    break;
                case Keys.A:
                    if (wrld.rcSrc.X >= 0)
                    {
                        wrld.rcSrc.X -= 2;
                    }
                    break;
                case Keys.W:
                    if (wrld.rcSrc.Y >= 0)
                        wrld.rcSrc.Y -= 2;
                    break;
                case Keys.S:
                    if (wrld.rcSrc.Y + wrld.rcSrc.Height <= wrld.wrld.Height)
                        wrld.rcSrc.Y += 2;
                    break;
            }
            DrawDubb(this.CreateGraphics());
        }

        private void Tt_Tick(object sender, EventArgs e)
        {
            DrawDubb(this.CreateGraphics());
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawDubb(e.Graphics);
        }
        void create_world()
        {
            wrld.wrld = new Bitmap("world.png"); // we will change the image later
            wrld.rcDst = new Rectangle(0,0, ClientSize.Width, ClientSize.Height);
            wrld.rcSrc = new Rectangle(0, wrld.wrld.Height - (wrld.wrld.Height / 2), wrld.wrld.Width / 3, wrld.wrld.Height - (wrld.wrld.Height / 2));
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            off = new Bitmap(ClientSize.Width,ClientSize.Height);
            create_world();
        }
        void DrawScene(Graphics g)
        {
            g.Clear(Color.White);
            g.DrawImage(wrld.wrld, wrld.rcDst, wrld.rcSrc, GraphicsUnit.Pixel);
        }
        void DrawDubb(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }
    }
}

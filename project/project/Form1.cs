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
    public partial class Form1 : Form
    {
        Bitmap off;
        Timer tt= new Timer();
        public Form1()
        {
            WindowState= FormWindowState.Maximized;
            this.Load += Form1_Load;
            this.Paint += Form1_Paint;
            tt.Interval = 1000 / 60; // 60 FPS
            tt.Start();
            tt.Tick += Tt_Tick;
            InitializeComponent();
        }

        private void Tt_Tick(object sender, EventArgs e)
        {
            DrawDubb(this.CreateGraphics());
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawDubb(e.Graphics);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            off = new Bitmap(ClientSize.Width,ClientSize.Height);
        }
        void DrawScene(Graphics g)
        {
            g.Clear(Color.White);
        }
        void DrawDubb(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }
    }
}

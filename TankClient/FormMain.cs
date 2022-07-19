using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CrazyTank
{
    public partial class FormMain : Form
    {
        public static int mapWidth = 40;
        public static int mapHeight = 30;

        private System.Timers.Timer paintTimer;
        private int timerSpan = 50;

        public Timer timer2 = new Timer();
        public Timer timer1 = new Timer();

        GameImages img = new GameImages();
        Controller ctrl;

        float[] color = new float[3];

        public FormMain()
        {
            InitializeComponent();
            Random s_Random = new Random(Guid.NewGuid().GetHashCode());
            timer2.Interval = s_Random.Next(1000, 5000);
            timer1.Interval = s_Random.Next(1000, 5000);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.GameStage.Width = mapWidth * Tile.size.Width;
            this.GameStage.Height = mapHeight * Tile.size.Height;
            this.MinimumSize = GameStage.Size;
            this.BackColor = Color.Black;

            picTank.Image = GameImages.tankPic[0][0];

            ctrl = new Controller(GameStage.Width, GameStage.Height);
        }

        private void bulletTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            GameStage.Invalidate();
        }

        private void FormMain_KeyPress(object sender, KeyPressEventArgs e)
        {
            ctrl.KeyPress(sender, e);
        }

        private void FormMain_KeyUp(object sender, KeyEventArgs e)
        {
            ctrl.KeyUp(sender, e);
            if (ctrl.myTank.Life == 0)
            {
                MessageBox.Show("You've lose!");
                Close();
            }
        }

        private void GameStage_Paint(object sender, PaintEventArgs e)
        {
            ctrl.Paint(e.Graphics);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            lblState.Text = "Connecting...";
            if (ctrl.NetClientConnect(txtIP.Text, Convert.ToInt32(txtPort.Text),color, txtName.Text))
            {
                pnlConnect.Enabled = false; //hide pane
                pnlConnect.Visible = false;
                this.Focus(); //focus main form

                paintTimer = new System.Timers.Timer(timerSpan);
                paintTimer.Elapsed += new System.Timers.ElapsedEventHandler(bulletTimer_Elapsed);
                paintTimer.AutoReset = true;
                paintTimer.Start();

                timer2.Tick += new EventHandler(ctrl.CoronaMove2);
                timer2.Start();

                timer1.Tick += new EventHandler(ctrl.coronaShoot);
                timer1.Start();
            }
            else
            {
                lblState.Text = "Connection failed";
            }
        }

        private void trbAll_Scroll(object sender, EventArgs e)
        {
            color[0] = (float)trbRed.Value / 100;
            color[1] = (float)trbGreen.Value / 100;
            color[2] = (float)trbBlue.Value / 100;
            
            picTank.Image = GameImages.Recoloring(GameImages.tankPic[0][0], color);
        }
    }
}

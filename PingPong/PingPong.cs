using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PingPong
{
    public partial class PingPong : Form
    {
        private Bitmap Backbuffer;
        private Timer GameTimer;
        private long tickCount = 0;

        private PongPlayer player1;
        private PongPlayer player2;
        private PongBall ball;

        public PingPong()
        {
            InitializeComponent();

            SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.DoubleBuffer, true);

            // Form
            BackColor = Color.Black;

            // Timer
            GameTimer = new Timer
            {
                Enabled = true,
                Interval = 20
            };
            GameTimer.Tick += new System.EventHandler(GameTimer_Tick);

            // Events
            ResizeEnd += new EventHandler(FormCreateBackBuffer);
            Load += new EventHandler(FormCreateBackBuffer);
            Paint += new PaintEventHandler(FormPaint);
            KeyDown += new KeyEventHandler(KeyIsDown);
            KeyUp += new KeyEventHandler(KeyIsUp);


            // Create objects
            player1 = new PongPlayer(this, "Left");
            player2 = new PongPlayer(this, "Right");
            ball = new PongBall(this);
        }

        private void FormCreateBackBuffer(object sender, EventArgs e)
        {
            if (Backbuffer != null)
            {
                Backbuffer.Dispose();
            }

            Backbuffer = new Bitmap(ClientSize.Width, ClientSize.Height);
        }

        private void FormPaint(object sender, PaintEventArgs e)
        {
            if (Backbuffer != null)
            {
                e.Graphics.DrawImageUnscaled(Backbuffer, Point.Empty);
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            // Do game logic
            Draw();
            tickCount++;
        }

        private void Draw()
        {
            if (Backbuffer != null)
            {
                using var g = Graphics.FromImage(Backbuffer);
                g.Clear(Color.Black);
                g.TextRenderingHint = TextRenderingHint.AntiAlias;

                g.FillRectangle(Brushes.White, player1.Shape);
                g.FillRectangle(Brushes.White, player2.Shape);

                g.FillEllipse(Brushes.White, ball.Shape);

                Invalidate();
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
            }

            if (e.KeyCode == Keys.Down)
            {
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            
        }
    }

    public class GameObject
    {
        public Form form { get; set; }
        public Size Size { get; set; }
        public Point Location { get; set; }
        public Rectangle Shape { get; set; }
        public int XVelocity { get; set; }
        public int YVelocity { get; set; }
        public double XAcceleration { get; set; }
        public double YAcceleration { get; set; }
        public double VelocityLimit { get; set; }
    }

    public class PongPlayer : GameObject
    {
        public string Position { get; set; }

        public PongPlayer(Form form, string startPosition)
        {
            Position = startPosition;
            this.form = form;

            Size = new Size(10, 30);

            if (Position == "Left")
            {
                Location = new Point(10, 10);
            }
            else if (Position == "Right")
            {
                Location = new Point(10, form.Width - 20);
            }

            Shape = new Rectangle(Location, Size);
        }

        public void Move()
        {

        }
    }

    public class PongBall : GameObject
    {
        public PongBall(Form form)
        {
            this.form = form;

            Size = new Size(10, 10);

            Location = new Point((form.Height / 2) - 5, (form.Width / 2) - 5);

            Shape = new Rectangle(Location, Size);
        }

        public void Move()
        {

        }
    }
}
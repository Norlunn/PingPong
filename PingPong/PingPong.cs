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
        private Font font = new Font("Segoe UI", 48);

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
            player1.Move();
            player2.Move();
            ball.Move();

            CheckCollision();
            CheckOutOfBounds();

            Draw();
            tickCount++;
        }

        private void CheckCollision()
        {
            if (player1.Shape.IntersectsWith(ball.Shape) || 
                player2.Shape.IntersectsWith(ball.Shape))
            {
                ball.XVelocity *= -1;
            }
        }

        private void CheckOutOfBounds()
        {
            if (ball.Location.X < 0)
            {
                player2.Score += 1;
                ball.Start();
            }

            if (ball.Location.X > (this.ClientSize.Width + 10))
            {
                player1.Score += 1;
                ball.Start();
            }
        }

        private void Draw()
        {
            if (Backbuffer != null)
            {
                using var g = Graphics.FromImage(Backbuffer);
                
                g.Clear(Color.Black);
                g.TextRenderingHint = TextRenderingHint.AntiAlias;

                // Draw middle line
                g.DrawLine(Pens.White, new Point(this.ClientSize.Width / 2, 0), new Point(this.ClientSize.Width / 2, this.ClientSize.Height));

                // Draw text

                g.DrawString(player1.Score + "", font, Brushes.White, new Point((this.ClientSize.Width / 2) - 100, 10));
                g.DrawString(player2.Score + "", font, Brushes.White, new Point((this.ClientSize.Width / 2) + 50, 10));

                // Draw players
                g.FillRectangle(Brushes.White, player1.Shape);
                g.FillRectangle(Brushes.White, player2.Shape);

                // Draw ping pong ball
                g.FillEllipse(Brushes.White, ball.Shape);

                Invalidate();
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                player2.YVelocity = -5;
            }

            if (e.KeyCode == Keys.Down)
            {
                player2.YVelocity = 5;
            }

            if (e.KeyCode == Keys.A)
            {
                player1.YVelocity = -5;
            }

            if (e.KeyCode == Keys.Z)
            {
                player1.YVelocity = 5;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                player2.YVelocity = 0;
            }

            if (e.KeyCode == Keys.A || e.KeyCode == Keys.Z)
            {
                player1.YVelocity = 0;
            }
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
    }

    public class PongPlayer : GameObject
    {
        public string Position { get; set; }
        public int Score = 0;

        public PongPlayer(Form form, string startPosition)
        {
            Position = startPosition;
            this.form = form;

            Size = new Size(8, 30);

            if (Position == "Left")
            {
                Location = new Point(10, 10);
            }
            else if (Position == "Right")
            {
                Location = new Point(form.ClientSize.Width - 30, form.ClientSize.Height - 40);
            }

            Shape = new Rectangle(Location, Size);
        }

        public void Move()
        {
            int newY = Location.Y + YVelocity;
            if (newY > 0 && newY < (form.ClientSize.Height - Size.Height))
            {
                Location = new Point(Location.X, newY);
            }

            Shape = new Rectangle(Location, Size);
        }
    }

    public class PongBall : GameObject
    {
        public PongBall(Form form)
        {
            this.form = form;

            Size = new Size(15, 15);

            Start();

            Shape = new Rectangle(Location, Size);
        }

        public void Move()
        {
            int newX = Location.X + XVelocity;
            int newY = Location.Y + YVelocity;

            if (newY < 0 || newY > form.ClientSize.Height)
            {
                YVelocity *= -1;
                newY = Location.Y + YVelocity;
            }

            Location = new Point(newX, newY);

            Shape = new Rectangle(Location, Size);
        }

        public void Start()
        {
            Location = new Point((form.ClientSize.Width / 2) - 5, (form.ClientSize.Height / 2) - 5);

            Random rnd = new Random();
            if (rnd.Next(0, 2) == 1)
            {
                XVelocity = 5;
            }
            else
            {
                XVelocity = -5;
            }

            if (rnd.Next(0, 2) == 1)
            {
                YVelocity = 5;
            }
            else
            {
                YVelocity = -5;
            }
        }
    }
}
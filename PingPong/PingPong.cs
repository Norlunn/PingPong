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
        private PongPlayer player1;
        private PongPlayer player2;
        private PongBall ball;
        private Font font = new Font("Segoe UI", 48);
        private Font smallFont = new Font("Segoe UI", 24);
        private int scoreLimit = 10;
        private int _speedMultiplier = 2;
        public int SpeedMultiplier
        {
            get
            {
                return _speedMultiplier;
            }
            set
            {
                _speedMultiplier = value;
                if (ball.XVelocity < 0)
                    ball.XVelocity = ball.BaseVelocity * value * -1;
                else
                    ball.XVelocity = ball.BaseVelocity * value;

                if (ball.YVelocity < 0)
                    ball.YVelocity = ball.BaseVelocity * value * -1;
                else
                    ball.YVelocity = ball.BaseVelocity * value;
            }
        }

        private bool showHelp = false;
        private bool gameWon = false;
        private long gameTicks = 0;

        public PingPong()
        {
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Text = "PingPong";

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
                Interval = 10
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
            ball = new PongBall(this, SpeedMultiplier);
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
            if (!gameWon && !showHelp)
            {
                // Do game logic
                player1.Move();
                player2.Move();
                ball.Move();

                CheckCollision();
                CheckOutOfBounds();
            }

            Draw();
            gameTicks++;
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
                ball.Start(SpeedMultiplier);
            }

            if (ball.Location.X > (this.ClientSize.Width + 10))
            {
                player1.Score += 1;
                ball.Start(SpeedMultiplier);
            }

            if (player1.Score >= scoreLimit || player2.Score >= scoreLimit)
            {
                gameWon = true;
            }
        }

        private void Draw()
        {
            if (Backbuffer != null)
            {
                using var g = Graphics.FromImage(Backbuffer);
                
                g.Clear(Color.Black);
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                if (gameWon)
                {
                    if (player1.Score > player2.Score)
                    {
                        g.DrawString("Player to the left won!", font, Brushes.White, new Point(95, 100));
                    }
                    else
                    {
                        g.DrawString("Player to the right won!", font, Brushes.White, new Point(95, 100));
                    }

                    g.DrawString("Press 'R' to play again", smallFont, Brushes.White, new RectangleF(
                        new PointF(100, 200),
                        new SizeF(900, 200)
                    ));
                }
                else if (showHelp)
                {
                    g.DrawString("Help", font, Brushes.White, new Point(95, 100));
                    g.DrawString("Left player keyboard: A (up), Z (down)", smallFont, Brushes.White, new RectangleF(
                        new PointF(100, 200),
                        new SizeF(900, 200)
                    ));
                    g.DrawString("Right player keyboard: Arrow up (up), Arrow down (down)", smallFont, Brushes.White, new RectangleF(
                        new PointF(100, 260),
                        new SizeF(900, 200)
                    ));
                    g.DrawString("Press 1, 2, or 3 to change game speed", smallFont, Brushes.White, new RectangleF(
                        new PointF(100, 320),
                        new SizeF(900, 200)
                    ));
                }
                else
                {
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

                    if (gameTicks < 400)
                    {
                        g.DrawString("Press 'H' for help", smallFont, Brushes.White, new RectangleF(
                        new PointF((this.ClientSize.Width / 2) - 130, this.ClientSize.Height - 50),
                        new SizeF(260, 50)
                    ));
                    }
                }
                Invalidate();
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                player2.YVelocity = player2.BaseVelocity * SpeedMultiplier * -1;
            }

            if (e.KeyCode == Keys.Down)
            {
                player2.YVelocity = player2.BaseVelocity * SpeedMultiplier;
            }

            if (e.KeyCode == Keys.A)
            {
                player1.YVelocity = player1.BaseVelocity * SpeedMultiplier * -1;
            }

            if (e.KeyCode == Keys.Z)
            {
                player1.YVelocity = player1.BaseVelocity * SpeedMultiplier;
            }

            if (e.KeyCode == Keys.D1)
            {
                SpeedMultiplier = 1;
            }

            if (e.KeyCode == Keys.D2)
            {
                SpeedMultiplier = 2;
            }

            if (e.KeyCode == Keys.D3)
            {
                SpeedMultiplier = 3;
            }

            if (e.KeyCode == Keys.H)
            {
                showHelp = !showHelp;
            }

            if (gameWon && e.KeyCode == Keys.R)
            {
                gameWon = false;
                showHelp = false;
                player1 = new PongPlayer(this, "Left");
                player2 = new PongPlayer(this, "Right");
                gameTicks = 0;
                ball = new PongBall(this, SpeedMultiplier);
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

        public int BaseVelocity { get; set; }
    }

    public class PongPlayer : GameObject
    {
        public string Position { get; set; }
        public int Score = 0;

        public PongPlayer(Form form, string startPosition)
        {
            Position = startPosition;
            this.form = form;

            Size = new Size(8, 45);

            if (Position == "Left")
            {
                Location = new Point(10, 10);
            }
            else if (Position == "Right")
            {
                Location = new Point(form.ClientSize.Width - 30, form.ClientSize.Height - 55);
            }

            Shape = new Rectangle(Location, Size);
            
            BaseVelocity = 3;
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
        public PongBall(Form form, int speedMultiplier = 2)
        {
            this.form = form;

            Size = new Size(15, 15);
            
            BaseVelocity = 3;
            
            Start(speedMultiplier);
            
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

        public void Start(int speedMultiplier = 2)
        {
            Location = new Point((form.ClientSize.Width / 2) - 5, (form.ClientSize.Height / 2) - 5);

            Random rnd = new Random();
            if (rnd.Next(0, 2) == 1)
            {
                XVelocity = BaseVelocity * speedMultiplier;
            }
            else
            {
                XVelocity = -1 * BaseVelocity * speedMultiplier;
            }

            if (rnd.Next(0, 2) == 1)
            {
                YVelocity = BaseVelocity * speedMultiplier;
            }
            else
            {
                YVelocity = -1 * BaseVelocity * speedMultiplier;
            }
        }
    }
}
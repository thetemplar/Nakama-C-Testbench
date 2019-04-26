using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NakamaMinimalGame
{
    public sealed partial class Game : Form
    {
        private Pen Player1Pen;
        private Pen Player2Pen;

        private int player1Pos = 0;
        private int player2Pos = 180;

        private bool _movesCw;
        private bool _movesCcw;

        private Ball _ball;

        private string _debug = "";

        class Ball
        {
            public class Vector
            {
                public double X;
                public double Y;

                public Vector(double x, double y)
                {
                    X = x;
                    Y = y;
                }

                public double DistanceTo(Vector v)
                {
                    return Math.Sqrt(Math.Pow((v.X - X), 2) + Math.Pow((v.Y - Y), 2));
                }
            }
            public Vector Pos;

            private Vector _mid;
            private int _radius;
            private Vector _dir;

            public Ball(int MidX, int MidY, int Radius)
            {
                Random r = new Random();
                _mid = new Vector(MidX, MidY);
                _radius = Radius;
                _dir = new Vector((r.NextDouble() - 0.5)*2, (r.NextDouble() - 0.5) * 2);
                Pos = new Vector(MidX, MidY);
            }

            public double BallAngle()
            {
                return Math.Atan2(Pos.Y - _mid.Y, Pos.X - _mid.X) * 180 / Math.PI;
            }

            public void Move()
            {
                Pos.X += _dir.X * 7;
                Pos.Y += _dir.Y * 7;
            }
            public void Reverse()
            {
                _dir.X *= -1f;
                _dir.Y *= -1f;
            }
            public void Deflect(double angle)
            {

            }
        }
        public Game()
        {
            InitializeComponent();
            Player1Pen = new Pen(Color.DeepSkyBlue, 5);
            Player2Pen = new Pen(Color.PaleVioletRed, 5);

            Timer t = new Timer {Interval = 33, Enabled = true};
            t.Tick += OnTick;

            DoubleBuffered = true;

            _ball = new Ball(260, 260, 250);
        }

        private void OnTick(object sender, EventArgs e)
        {
            _ball.Move();

            if (_movesCw)
                player1Pos += 2;
            if (_movesCcw)
                player1Pos -= 2;

            if (player1Pos > 359)
                player1Pos = 0;
            if (player1Pos < 0)
                player1Pos = 360;

            double angle = _ball.BallAngle();
            if (angle < 0)
                angle += 360;
            double dist = angle - player1Pos;
            _debug = ("player:" + player1Pos + " - ball:" + angle + " - dist: " + dist);

            if (_ball.Pos.DistanceTo(new Ball.Vector(260, 260)) > 250)
            {
                if (Math.Abs(dist) < 10)
                    _ball.Deflect(dist);
                else
                    _ball = new Ball(260, 260, 250);
            }


            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_ball == null) return;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            g.DrawArc(Player1Pen, 10, 10, 510, 510, player1Pos - 10 , 10);
            g.DrawArc(Player1Pen, 10, 10, 510, 510, player1Pos , 10);
            g.DrawArc(Player2Pen, 10, 10, 510, 510, player2Pos - 10, 10);
            g.DrawArc(Player2Pen, 10, 10, 510, 510, player2Pos, 10);

            g.FillEllipse(Brushes.Black, (int)(_ball.Pos.X + 2), (int)(_ball.Pos.Y + 2), 4, 4);

            g.DrawString(_debug, this.Font, Brushes.DimGray, 3, 3);
        }

        private void Game_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.S:
                    _movesCw = true;
                    return;
                case Keys.W:
                    _movesCcw = true;
                    return;
            }
        }

        private void Game_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.S:
                    _movesCw = false;
                    return;
                case Keys.W:
                    _movesCcw = false;
                    return;
            }
        }
    }
}

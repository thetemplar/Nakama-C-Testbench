using Nakama;
using Nakama.TinyJson;
using Newtonsoft.Json.Linq;
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
        public int Player2Pos = -1;

        private bool _movesCw;
        private bool _movesCcw;

        private Ball _ball;
        
        private int _lastPlayerContact;
        private int _points1;
        private int _points2;

        private ISocket _socket;
        private ISession _session;
        private string _matchId;

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
                _dir = GetVectorFromAngle(r.NextDouble() * 360d);
                Pos = new Vector(MidX, MidY);
            }

            public Vector GetVectorFromAngle(double angle)
            {
                return new Vector(Math.Cos(angle * Math.PI / 180), Math.Sin(angle * Math.PI / 180));
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
            public void Deflect(double angle, double dist)
            {
                _dir = GetVectorFromAngle(angle + dist * 5);
                Reverse();
            }
        }

        enum OpCodes
        {
            Dict = 1
        }
        Dictionary<string, string> sendingDict = new Dictionary<string, string> ();
        
        public Game(ISession session, ISocket socket, string matchId)
        {
            InitializeComponent();
            DoubleBuffered = true;

            _socket = socket;
            _session = session;
            _matchId = matchId;

            this.Text = _session.Username;

            _socket.OnMatchState += _socket_OnMatchState;

            sendingDict.Add(_session.UserId, player1Pos.ToString("0.0000"));

            Init();
            
            var timer = new System.Threading.Timer((e) =>
            {
                sendingDict[_session.UserId] = player1Pos.ToString("0.0000");
                var msg = sendingDict.ToJson();
                _socket.SendMatchStateAsync(_matchId, (long)OpCodes.Dict, msg);
            }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(20));
        }

        private void _socket_OnMatchState(object sender, IMatchState e)
        {
            if (e.OpCode != (long)OpCodes.Dict) throw new NotImplementedException();

            var content = Encoding.UTF8.GetString(e.State);
            dynamic data = JArray.Parse(content);
        }

        void Init()
        {
            Player1Pen = new Pen(Color.DeepSkyBlue, 5);
            Player2Pen = new Pen(Color.PaleVioletRed, 5);

            Timer t = new Timer { Interval = 33, Enabled = true };
            t.Tick += OnTick;

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
            double distPlayer1 = angle - player1Pos;
            double distPlayer2 = angle - Player2Pos;

            if (_ball.Pos.DistanceTo(new Ball.Vector(260, 260)) > 250)
            {
                if (Math.Abs(distPlayer1) < 10)
                {
                    _ball.Deflect(player1Pos, distPlayer1);
                    _lastPlayerContact = 1;
                }
                else if (Player2Pos != -1 && Math.Abs(distPlayer2) < 10)
                {
                    _ball.Deflect(Player2Pos, distPlayer2);
                    _lastPlayerContact = 2;
                }
                else
                {
                    _ball = new Ball(260, 260, 250);
                    _lastPlayerContact = 0;
                    if (_lastPlayerContact == 1)
                        _points1++;
                    if (_lastPlayerContact == 2)
                        _points2++;
                }
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
            if (Player2Pos != -1)
            {
                g.DrawArc(Player2Pen, 10, 10, 510, 510, Player2Pos - 10, 10);
                g.DrawArc(Player2Pen, 10, 10, 510, 510, Player2Pos, 10);
            }

            g.FillEllipse(Brushes.Black, (int)(_ball.Pos.X + 2), (int)(_ball.Pos.Y + 2), 4, 4);
            g.FillEllipse(Brushes.Gray, (int)(260 + 1), (int)(260 + 1), 2, 2);

            g.DrawString("Points ->   Player1: " + _points1 + "   -   Player2: " + _points2, this.Font, Brushes.Black, 3, 3);
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

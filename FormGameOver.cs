using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SpaceShooter
{
    public class FormGameOver : Form
    {
        private bool   _won;
        private int    _score;
        private int    _level;
        private Timer  _anim = new Timer();
        private int    _tick = 0;
        private Star[] _stars = new Star[100];
        private Random _rng   = new Random();

        private Button _btnRestart, _btnMenu, _btnScores;

        public FormGameOver(bool won, int score, int level)
        {
            _won   = won;
            _score = score;
            _level = level;

            Text            = won ? "VICTORY!" : "GAME OVER";
            ClientSize      = new Size(500, 400);
            BackColor       = Color.Black;
            DoubleBuffered  = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox     = false;
            StartPosition   = FormStartPosition.CenterScreen;

            InitStars();
            BuildButtons();

            _anim.Interval = GameConfig.TICK_MS;
            _anim.Tick += (s, e) => { _tick++; UpdateStars(); Invalidate(); };
            _anim.Start();
        }

        void InitStars()
        {
            for (int i = 0; i < _stars.Length; i++)
                _stars[i] = new Star
                {
                    X = _rng.Next(0, 500), Y = _rng.Next(0, 400),
                    Size = (float)(_rng.NextDouble() * 2 + 0.5),
                    Brightness = (float)_rng.NextDouble(),
                    Speed = (float)(_rng.NextDouble() * 1.5 + 0.3),
                    Color = Color.White,
                };
        }

        void UpdateStars()
        {
            foreach (var s in _stars)
            {
                s.Y += s.Speed;
                if (s.Y > 405) { s.Y = -5; s.X = _rng.Next(0, 500); }
            }
        }

        void BuildButtons()
        {
            int cx = 250;
            _btnRestart = MakeBtn("↺  PLAY AGAIN",  cx - 150, 290, 140);
            _btnMenu    = MakeBtn("⌂  MAIN MENU",   cx + 10,  290, 140);
            _btnScores  = MakeBtn("🏆  SCOREBOARD",  cx - 70,  340, 140);

            _btnRestart.Click += (s, e) => { _anim.Stop(); DialogResult = DialogResult.No;  Close(); };
            _btnMenu.Click    += (s, e) => { _anim.Stop(); DialogResult = DialogResult.Cancel; Close(); };
            _btnScores.Click  += (s, e) => { _anim.Stop(); SaveAndShowBoard(); };

            Controls.Add(_btnRestart);
            Controls.Add(_btnMenu);
            Controls.Add(_btnScores);
        }

        void SaveAndShowBoard()
        {
            if (_won)
            {
                string name = Microsoft.VisualBasic.Interaction.InputBox(
                    "Enter your name:", "Scoreboard", "Player");
                if (!string.IsNullOrWhiteSpace(name))
                    ScoreManager.Add(new ScoreEntry(name, _score, _level));
            }
            new FormScoreboard().ShowDialog(this);
            _anim.Start();
        }

        Button MakeBtn(string text, int x, int y, int w)
        {
            var btn = new Button
            {
                Text = text, Location = new Point(x, y), Size = new Size(w, 38),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Consolas", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(200, 220, 255),
                BackColor = Color.FromArgb(15, 25, 55),
                Cursor = Cursors.Hand,
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(60, 90, 180);
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 50, 110);
            return btn;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            DrawHelper.DrawNebula(g, 500, 400, _tick);
            DrawHelper.DrawStars(g, _stars);

            DrawMainText(g);
            DrawStats(g);
        }

        void DrawMainText(Graphics g)
        {
            string title   = _won ? "VICTORY!" : "GAME OVER";
            Color  topC    = _won ? Color.FromArgb(80, 255, 160)  : Color.FromArgb(255, 80, 80);
            Color  botC    = _won ? Color.FromArgb(0, 180, 100)   : Color.FromArgb(180, 30, 30);

            float pulse = (float)(Math.Sin(_tick * 0.05) * 0.2 + 0.8);

            using (var f = new Font("Impact", 54, FontStyle.Bold))
            {
                SizeF sz = g.MeasureString(title, f);
                float x  = (500 - sz.Width) / 2;
                float y  = 40;

                // Glow
                for (int d = 6; d >= 1; d--)
                    using (var b = new SolidBrush(Color.FromArgb((int)(15 * pulse), _won ? Color.Cyan : Color.Red)))
                        g.DrawString(title, f, b, x - d, y - d);

                // Main
                using (var b = new LinearGradientBrush(
                    new PointF(x, y), new PointF(x, y + sz.Height),
                    Color.FromArgb((int)(255 * pulse), topC),
                    Color.FromArgb((int)(200 * pulse), botC)))
                    g.DrawString(title, f, b, x, y);
            }

            // Subtitle
            string sub = _won ? "All enemies destroyed!" : "Your ship was destroyed!";
            using (var f = new Font("Consolas", 14, FontStyle.Italic))
            {
                SizeF sz = g.MeasureString(sub, f);
                using (var b = new SolidBrush(Color.FromArgb(180, 200, 200, 200)))
                    g.DrawString(sub, f, b, (500 - sz.Width) / 2, 115);
            }
        }

        void DrawStats(Graphics g)
        {
            int y = 155;
            DrawStat(g, "Level Completed", $"{_level}",  y);       y += 38;
            DrawStat(g, "Final Score",     $"{_score:D6}", y);

            // Decorative line
            using (var p = new Pen(Color.FromArgb(50, 80, 140, 255), 1f))
                g.DrawLine(p, 60, 145, 440, 145);

            using (var p = new Pen(Color.FromArgb(50, 80, 140, 255), 1f))
                g.DrawLine(p, 60, 240, 440, 240);
        }

        void DrawStat(Graphics g, string label, string value, int y)
        {
            using (var fl = new Font("Consolas", 14, FontStyle.Regular))
            using (var fv = new Font("Consolas", 14, FontStyle.Bold))
            using (var bl = new SolidBrush(Color.FromArgb(160, 180, 180, 200)))
            using (var bv = new SolidBrush(Color.FromArgb(230, 220, 255, 80)))
            {
                g.DrawString(label + ":", fl, bl, 70, y);
                SizeF sv = g.MeasureString(value, fv);
                g.DrawString(value, fv, bv, 430 - sv.Width, y);
            }
        }
    }
}

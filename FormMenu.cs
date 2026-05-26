using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SpaceShooter
{
    public class FormMenu : Form
    {
        private Timer  _anim  = new Timer();
        private int    _tick  = 0;
        private Star[] _stars = new Star[150];
        private Random _rng   = new Random();

        private Button _btnLevel1, _btnLevel2, _btnScores, _btnQuit;
        private Panel  _menuPanel;

        public FormMenu()
        {
            Text            = "Space Shooter — Main Menu";
            ClientSize      = new Size(GameConfig.FORM_WIDTH, GameConfig.FORM_HEIGHT);
            BackColor       = Color.Black;
            DoubleBuffered  = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox     = false;
            StartPosition   = FormStartPosition.CenterScreen;

            InitStars();
            BuildControls();

            _anim.Interval = GameConfig.TICK_MS;
            _anim.Tick += (s, e) => { _tick++; UpdateStars(); Invalidate(); };
            _anim.Start();
        }

        void InitStars()
        {
            for (int i = 0; i < _stars.Length; i++)
                _stars[i] = new Star
                {
                    X = _rng.Next(0, GameConfig.FORM_WIDTH),
                    Y = _rng.Next(0, GameConfig.FORM_HEIGHT),
                    Size = (float)(_rng.NextDouble() * 2.5 + 0.5),
                    Brightness = (float)_rng.NextDouble(),
                    Speed = (float)(_rng.NextDouble() * 1.2 + 0.3),
                    Color = Color.White,
                };
        }

        void UpdateStars()
        {
            foreach (var s in _stars)
            {
                s.Y += s.Speed;
                if (s.Y > GameConfig.FORM_HEIGHT + 5) { s.Y = -5; s.X = _rng.Next(0, GameConfig.FORM_WIDTH); }
                s.Brightness = Math.Max(0.1f, Math.Min(1f, s.Brightness + (float)(_rng.NextDouble() * 0.06 - 0.03)));
            }
        }

        void BuildControls()
        {
            _menuPanel = new Panel
            {
                BackColor = Color.Transparent,
                Size = new Size(320, 300),
                Location = new Point(GameConfig.FORM_WIDTH / 2 - 160, GameConfig.FORM_HEIGHT / 2 - 20),
            };

            _btnLevel1 = MakeButton("▶  LEVEL 1 — RECRUIT",  0);
            _btnLevel2 = MakeButton("▶  LEVEL 2 — VETERAN",  70);
            _btnScores = MakeButton("🏆  SCOREBOARD",          140);
            _btnQuit   = MakeButton("✕  QUIT",                210);

            _btnLevel1.Click += (s, e) => StartGame(1);
            _btnLevel2.Click += (s, e) => StartGame(2);
            _btnScores.Click += (s, e) => { _anim.Stop(); new FormScoreboard().ShowDialog(this); _anim.Start(); };
            _btnQuit.Click   += (s, e) => Application.Exit();

            _menuPanel.Controls.Add(_btnLevel1);
            _menuPanel.Controls.Add(_btnLevel2);
            _menuPanel.Controls.Add(_btnScores);
            _menuPanel.Controls.Add(_btnQuit);
            Controls.Add(_menuPanel);
        }

        Button MakeButton(string text, int offsetY)
        {
            var btn = new Button
            {
                Text      = text,
                Size      = new Size(300, 52),
                Location  = new Point(10, offsetY),
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Consolas", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(200, 200, 255),
                BackColor = Color.FromArgb(20, 30, 60),
                Cursor    = Cursors.Hand,
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(80, 100, 200);
            btn.FlatAppearance.BorderSize  = 1;
            btn.FlatAppearance.MouseOverBackColor  = Color.FromArgb(40, 60, 130);
            btn.FlatAppearance.MouseDownBackColor  = Color.FromArgb(60, 90, 180);
            return btn;
        }

        void StartGame(int level)
        {
            _anim.Stop();
            var game = new FormGame(level);
            game.ShowDialog(this);
            // Check if level completed and show scoreboard prompt
            if (game.DialogResult == DialogResult.Yes)
            {
                // Prompt for name, save score
                string name = Microsoft.VisualBasic.Interaction.InputBox(
                    "You won! Enter your name for the scoreboard:", "WINNER!", "Player");
                if (!string.IsNullOrWhiteSpace(name))
                    ScoreManager.Add(new ScoreEntry(name, GetScoreFromForm(game), level));
                new FormScoreboard().ShowDialog(this);
            }
            _anim.Start();
        }

        // Retrieve score from closed form (passed via Tag)
        int GetScoreFromForm(Form f)
        {
            return f.Tag is int s ? s : 0;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            DrawHelper.DrawNebula(g, ClientSize.Width, ClientSize.Height, _tick / 2);
            DrawHelper.DrawStars(g, _stars);

            DrawTitle(g);
            DrawSubtitle(g);
        }

        void DrawTitle(Graphics g)
        {
            float pulse = (float)(Math.Sin(_tick * 0.04) * 0.15 + 0.85);
            string title = "SPACE SHOOTER";
            using (var f = new Font("Impact", 58, FontStyle.Bold))
            {
                SizeF sz = g.MeasureString(title, f);
                float x  = (ClientSize.Width - sz.Width) / 2;
                float y  = 55;

                // Glow
                for (int d = 8; d >= 1; d--)
                {
                    using (var b = new SolidBrush(Color.FromArgb((int)(20 * pulse), 80, 140, 255)))
                        g.DrawString(title, f, b, x - d, y - d);
                }

                // Shadow
                using (var b = new SolidBrush(Color.FromArgb(160, 0, 0, 80)))
                    g.DrawString(title, f, b, x + 4, y + 4);

                // Main gradient text (drawn as filled rect clip trick)
                using (var b = new LinearGradientBrush(
                    new PointF(x, y), new PointF(x, y + sz.Height),
                    Color.FromArgb((int)(255 * pulse), 160, 220, 255),
                    Color.FromArgb((int)(200 * pulse), 40, 80, 255)))
                    g.DrawString(title, f, b, x, y);
            }
        }

        void DrawSubtitle(Graphics g)
        {
            string sub = "DESTROY ENEMIES • DODGE METEORS • SURVIVE";
            using (var f = new Font("Consolas", 13, FontStyle.Regular))
            {
                SizeF sz = g.MeasureString(sub, f);
                float x  = (ClientSize.Width - sz.Width) / 2;
                int   a  = (int)(120 + Math.Sin(_tick * 0.05) * 60);
                using (var b = new SolidBrush(Color.FromArgb(a, 180, 220, 255)))
                    g.DrawString(sub, f, b, x, 135);
            }

            // Decorative line
            using (var p = new Pen(Color.FromArgb(60, 80, 140, 255), 1f))
            {
                g.DrawLine(p, 80, 165, ClientSize.Width - 80, 165);
                g.DrawLine(p, 80, 168, ClientSize.Width - 80, 168);
            }

            // Draw a small demo ship
            DrawHelper.DrawPlayerShip(g, new Rectangle(ClientSize.Width / 2 - 30, 185, 60, 70));
        }
    }
}

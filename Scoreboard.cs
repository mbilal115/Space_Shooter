using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SpaceShooter
{
    // ─── Score persistence ────────────────────────────────────────────────────────
    public static class ScoreManager
    {
        private static readonly string _path =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                         "SpaceShooterScores.txt");

        private static List<ScoreEntry> _cache;

        public static List<ScoreEntry> Load()
        {
            if (_cache != null) return _cache;
            _cache = new List<ScoreEntry>();
            if (!File.Exists(_path)) return _cache;
            foreach (var line in File.ReadAllLines(_path))
            {
                var parts = line.Split('|');
                if (parts.Length < 3) continue;
                if (int.TryParse(parts[1], out int score) && int.TryParse(parts[2], out int lvl))
                    _cache.Add(new ScoreEntry(parts[0], score, lvl));
            }
            return _cache;
        }

        public static void Add(ScoreEntry entry)
        {
            Load();
            _cache.Add(entry);
            Save();
        }

        static void Save()
        {
            var lines = _cache.Select(e => $"{e.Name}|{e.Score}|{e.Level}");
            File.WriteAllLines(_path, lines);
        }

        public static List<ScoreEntry> TopN(int n = 15)
        {
            return Load().OrderByDescending(e => e.Score).Take(n).ToList();
        }
    }

    // ─── Scoreboard Form ─────────────────────────────────────────────────────────
    public class FormScoreboard : Form
    {
        private Timer  _anim  = new Timer();
        private int    _tick  = 0;
        private Star[] _stars = new Star[100];
        private Random _rng   = new Random();

        private List<ScoreEntry> _entries;
        private Button _btnClose;

        public FormScoreboard()
        {
            _entries = ScoreManager.TopN(12);

            Text            = "Scoreboard";
            ClientSize      = new Size(560, 520);
            BackColor       = Color.Black;
            DoubleBuffered  = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox     = false;
            StartPosition   = FormStartPosition.CenterScreen;

            InitStars();

            _btnClose = new Button
            {
                Text = "✕  CLOSE",
                Size = new Size(140, 38),
                Location = new Point(210, 468),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Consolas", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(200, 220, 255),
                BackColor = Color.FromArgb(20, 30, 65),
                Cursor = Cursors.Hand,
            };
            _btnClose.FlatAppearance.BorderColor = Color.FromArgb(60, 90, 180);
            _btnClose.FlatAppearance.BorderSize = 1;
            _btnClose.Click += (s, e) => Close();
            Controls.Add(_btnClose);

            _anim.Interval = GameConfig.TICK_MS;
            _anim.Tick += (s, e) => { _tick++; UpdateStars(); Invalidate(); };
            _anim.Start();
        }

        void InitStars()
        {
            for (int i = 0; i < _stars.Length; i++)
                _stars[i] = new Star
                {
                    X = _rng.Next(0, 560), Y = _rng.Next(0, 520),
                    Size = (float)(_rng.NextDouble() * 2 + 0.5),
                    Brightness = (float)_rng.NextDouble(),
                    Speed = (float)(_rng.NextDouble() * 0.8 + 0.2),
                    Color = Color.White,
                };
        }

        void UpdateStars()
        {
            foreach (var s in _stars)
            {
                s.Y += s.Speed;
                if (s.Y > 525) { s.Y = -5; s.X = _rng.Next(0, 560); }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            DrawHelper.DrawNebula(g, 560, 520, _tick);
            DrawHelper.DrawStars(g, _stars);

            DrawTitle(g);
            DrawTable(g);
        }

        void DrawTitle(Graphics g)
        {
            string title = "🏆  SCOREBOARD";
            using (var f = new Font("Impact", 36, FontStyle.Bold))
            {
                SizeF sz = g.MeasureString(title, f);
                float x = (560 - sz.Width) / 2;
                using (var b = new LinearGradientBrush(
                    new PointF(x, 18), new PointF(x, 18 + sz.Height),
                    Color.FromArgb(220, 255, 215, 0), Color.FromArgb(180, 255, 140, 0)))
                    g.DrawString(title, f, b, x, 18);
            }
            using (var p = new Pen(Color.FromArgb(50, 255, 200, 50), 1f))
                g.DrawLine(p, 40, 78, 520, 78);
        }

        void DrawTable(Graphics g)
        {
            if (_entries.Count == 0)
            {
                using (var f = new Font("Consolas", 16, FontStyle.Italic))
                using (var b = new SolidBrush(Color.FromArgb(140, 200, 200, 200)))
                {
                    string msg = "No scores yet — play a game!";
                    SizeF sz = g.MeasureString(msg, f);
                    g.DrawString(msg, f, b, (560 - sz.Width) / 2, 220);
                }
                return;
            }

            // Header
            int y = 90;
            DrawRow(g, y, "#", "NAME", "SCORE", "LVL", isHeader: true);
            y += 32;

            using (var p = new Pen(Color.FromArgb(40, 80, 120, 255), 1f))
                g.DrawLine(p, 30, y - 6, 530, y - 6);

            // Rows
            for (int i = 0; i < _entries.Count; i++)
            {
                var entry = _entries[i];
                Color rowC = i == 0 ? Color.FromArgb(200, 255, 215, 0) :
                             i == 1 ? Color.FromArgb(200, 192, 192, 192) :
                             i == 2 ? Color.FromArgb(200, 205, 127, 50) :
                                      Color.FromArgb(160, 180, 200, 220);

                // Alternate row bg
                if (i % 2 == 0)
                    using (var b = new SolidBrush(Color.FromArgb(12, 60, 100, 180)))
                        g.FillRectangle(b, 28, y - 2, 504, 28);

                DrawRow(g, y, $"{i+1}", entry.Name, $"{entry.Score:D6}", $"L{entry.Level}",
                        isHeader: false, color: rowC);
                y += 30;
            }
        }

        void DrawRow(Graphics g, int y, string rank, string name, string score, string level,
                     bool isHeader, Color? color = null)
        {
            Color c = color ?? Color.FromArgb(200, 200, 210, 230);
            var   f = isHeader
                ? new Font("Consolas", 13, FontStyle.Bold)
                : new Font("Consolas", 12, FontStyle.Regular);

            using (f)
            using (var b = new SolidBrush(c))
            {
                g.DrawString(rank,  f, b, 36,  y);
                g.DrawString(name,  f, b, 80,  y);
                g.DrawString(score, f, b, 320, y);
                g.DrawString(level, f, b, 480, y);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SpaceShooter
{
    public class FormGame : Form
    {
        // ── State ────────────────────────────────────────────────────────────────
        private Player _player;
        private List<Enemy>     _enemies    = new List<Enemy>();
        private List<Bullet>    _bullets    = new List<Bullet>();
        private List<Meteor>    _meteors    = new List<Meteor>();
        private List<Explosion> _explosions = new List<Explosion>();
        private Star[]          _stars      = new Star[180];

        private Timer  _gameTimer = new Timer();
        private int    _tick      = 0;
        private int    _score     = 0;
        private int    _level;
        private bool   _paused    = false;
        private int    _nebulaOff = 0;
        private Random _rng       = new Random();

        private int _meteorCooldown = 0;
        private int _surviveTick    = 0;

        // keyboard state
        private HashSet<Keys> _keys = new HashSet<Keys>();

        // Properties derived from level
        private int EnemySpeed      => _level == 1 ? GameConfig.ENEMY_SPEED_L1      : GameConfig.ENEMY_SPEED_L2;
        private int EnemyBulletSpd  => _level == 1 ? GameConfig.ENEMY_BULLET_SPEED_L1 : GameConfig.ENEMY_BULLET_SPEED_L2;
        private int FireInterval    => _level == 1 ? GameConfig.ENEMY_FIRE_INTERVAL_L1 : GameConfig.ENEMY_FIRE_INTERVAL_L2;
        private int MeteorInterval  => _level == 1 ? GameConfig.METEOR_INTERVAL_L1   : GameConfig.METEOR_INTERVAL_L2;
        private int MeteorSpeed     => _level == 1 ? GameConfig.METEOR_SPEED_L1      : GameConfig.METEOR_SPEED_L2;
        private int EnemyCount      => _level == 1 ? GameConfig.LEVEL1_ENEMY_COUNT   : GameConfig.LEVEL2_ENEMY_COUNT;
        private int EnemyHP         => _level == 1 ? 1 : 2;

        // ── Constructor ──────────────────────────────────────────────────────────
        public FormGame(int level)
        {
            _level = level;
            InitUI();
            InitStars();
            InitEntities();

            _gameTimer.Interval = GameConfig.TICK_MS;
            _gameTimer.Tick += OnTick;
            _gameTimer.Start();
        }

        // ── UI Setup ─────────────────────────────────────────────────────────────
        void InitUI()
        {
            Text             = $"Space Shooter — Level {_level}";
            ClientSize       = new Size(GameConfig.FORM_WIDTH, GameConfig.FORM_HEIGHT);
            BackColor        = Color.Black;
            DoubleBuffered   = true;
            FormBorderStyle  = FormBorderStyle.FixedSingle;
            MaximizeBox      = false;
            StartPosition    = FormStartPosition.CenterScreen;
            KeyPreview       = true;
        }

        // ── Stars ────────────────────────────────────────────────────────────────
        void InitStars()
        {
            for (int i = 0; i < _stars.Length; i++)
            {
                _stars[i] = new Star
                {
                    X          = _rng.Next(0, GameConfig.FORM_WIDTH),
                    Y          = _rng.Next(0, GameConfig.FORM_HEIGHT),
                    Size       = (float)(_rng.NextDouble() * 2.5 + 0.5),
                    Brightness = (float)_rng.NextDouble(),
                    Speed      = (float)(_rng.NextDouble() * 1.5 + 0.3),
                    Color      = PickStarColor(),
                };
            }
        }

        Color PickStarColor()
        {
            int r = _rng.Next(4);
            switch (r)
            {
                case 0: return Color.White;
                case 1: return Color.FromArgb(200, 220, 255);
                case 2: return Color.FromArgb(255, 220, 180);
                default:return Color.FromArgb(180, 200, 255);
            }
        }

        void UpdateStars()
        {
            foreach (var s in _stars)
            {
                s.Y += s.Speed;
                if (s.Y > GameConfig.FORM_HEIGHT + 5)
                {
                    s.Y = -5;
                    s.X = _rng.Next(0, GameConfig.FORM_WIDTH);
                }
                // twinkle
                s.Brightness = Math.Max(0.1f, Math.Min(1f,
                    s.Brightness + (float)(_rng.NextDouble() * 0.06 - 0.03)));
            }
        }

        // ── Entities ─────────────────────────────────────────────────────────────
        void InitEntities()
        {
            _player = new Player(GameConfig.FORM_WIDTH, GameConfig.FORM_HEIGHT);
            SpawnEnemies();
        }

        void SpawnEnemies()
        {
            _enemies.Clear();
            int cols   = EnemyCount <= 6 ? 3 : 5;
            int rows   = (int)Math.Ceiling((double)EnemyCount / cols);
            int startX = 60;
            int startY = 60;
            int padX   = (GameConfig.FORM_WIDTH - startX * 2) / cols;
            int padY   = 75;

            int count = 0;
            for (int row = 0; row < rows && count < EnemyCount; row++)
                for (int col = 0; col < cols && count < EnemyCount; col++, count++)
                {
                    EnemyType t = (EnemyType)(_rng.Next(0, 3));
                    int x = startX + col * padX;
                    int y = startY + row * padY;
                    _enemies.Add(new Enemy(x, y, t, EnemySpeed, EnemyHP, FireInterval));
                }
        }

        // ── Game Loop ─────────────────────────────────────────────────────────────
        void OnTick(object sender, EventArgs e)
        {
            if (_paused) return;
            _tick++;
            _nebulaOff = (_nebulaOff + 1) % (GameConfig.FORM_HEIGHT + 200);

            // Survive score
            _surviveTick++;
            if (_surviveTick >= 60) { _score += GameConfig.SCORE_SURVIVE; _surviveTick = 0; }

            ProcessInput();
            UpdateStars();
            _player.Update(GameConfig.FORM_WIDTH);
            UpdateEnemies();
            UpdateBullets();
            UpdateMeteors();
            UpdateExplosions();
            CheckCollisions();
            CheckGameState();

            Invalidate();
        }

        // ── Input ─────────────────────────────────────────────────────────────────
        protected override void OnKeyDown(KeyEventArgs e)
        {
            _keys.Add(e.KeyCode);
            if (e.KeyCode == Keys.Escape)
            {
                _paused = !_paused;
                Invalidate();
            }
            if (e.KeyCode == Keys.Space && !_paused && _player.CanShoot && _player.IsAlive)
                FirePlayerBullet();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            _keys.Remove(e.KeyCode);
        }

        void ProcessInput()
        {
            _player.MoveLeft  = _keys.Contains(Keys.Left)  || _keys.Contains(Keys.A);
            _player.MoveRight = _keys.Contains(Keys.Right) || _keys.Contains(Keys.D);

            // Auto-fire when held
            if ((_keys.Contains(Keys.Space) || _keys.Contains(Keys.Z)) && _player.CanShoot && _player.IsAlive)
                FirePlayerBullet();
        }

        // ── Bullet Firing ────────────────────────────────────────────────────────
        void FirePlayerBullet()
        {
            int cx = _player.Bounds.Left + _player.Bounds.Width / 2;
            int cy = _player.Bounds.Top;
            _bullets.Add(new Bullet(cx, cy, -GameConfig.PLAYER_BULLET_SPEED, true));
            _player.ShootCooldown = 10;
        }

        void FireEnemyBullet(Enemy en)
        {
            int cx = en.Bounds.Left + en.Bounds.Width / 2;
            int cy = en.Bounds.Bottom;
            _bullets.Add(new Bullet(cx, cy, EnemyBulletSpd, false, en.Type));
            en.ResetFireCooldown(FireInterval + _rng.Next(-10, 10));
        }

        // ── Update Methods ────────────────────────────────────────────────────────
        void UpdateEnemies()
        {
            foreach (var en in _enemies)
                if (en.IsAlive)
                {
                    en.Update(GameConfig.FORM_WIDTH, GameConfig.FORM_HEIGHT, FireInterval);
                    if (en.CanFire()) FireEnemyBullet(en);
                }
        }

        void UpdateBullets()
        {
            for (int i = _bullets.Count - 1; i >= 0; i--)
            {
                _bullets[i].Update();
                if (_bullets[i].IsOffScreen(GameConfig.FORM_HEIGHT))
                    _bullets.RemoveAt(i);
            }
        }

        void UpdateMeteors()
        {
            _meteorCooldown--;
            if (_meteorCooldown <= 0)
            {
                _meteors.Add(new Meteor(GameConfig.FORM_WIDTH, MeteorSpeed));
                _meteorCooldown = MeteorInterval + _rng.Next(-20, 20);
            }
            for (int i = _meteors.Count - 1; i >= 0; i--)
            {
                _meteors[i].Update(GameConfig.FORM_WIDTH);
                if (_meteors[i].IsOffScreen(GameConfig.FORM_HEIGHT))
                    _meteors.RemoveAt(i);
            }
        }

        void UpdateExplosions()
        {
            for (int i = _explosions.Count - 1; i >= 0; i--)
            {
                _explosions[i].Update();
                if (_explosions[i].Done)
                    _explosions.RemoveAt(i);
            }
        }

        // ── Collision Detection ───────────────────────────────────────────────────
        void CheckCollisions()
        {
            // Player bullets vs enemies + meteors
            for (int bi = _bullets.Count - 1; bi >= 0; bi--)
            {
                var b = _bullets[bi];
                if (!b.IsPlayerBullet) continue;

                bool hit = false;

                // vs enemies
                foreach (var en in _enemies)
                {
                    if (!en.IsAlive) continue;
                    if (b.Bounds.IntersectsWith(en.Bounds))
                    {
                        en.TakeDamage(1);
                        if (!en.IsAlive)
                        {
                            _score += GameConfig.SCORE_ENEMY;
                            Explode(en.Bounds);
                        }
                        hit = true;
                        break;
                    }
                }
                if (hit) { _bullets.RemoveAt(bi); continue; }

                // vs meteors
                for (int mi = _meteors.Count - 1; mi >= 0; mi--)
                {
                    var m = _meteors[mi];
                    if (!m.Active) continue;
                    if (b.Bounds.IntersectsWith(m.Bounds))
                    {
                        _score += GameConfig.SCORE_METEOR;
                        Explode(m.Bounds);
                        _meteors.RemoveAt(mi);
                        hit = true;
                        break;
                    }
                }
                if (hit) _bullets.RemoveAt(bi);
            }

            // Enemy bullets vs player
            for (int bi = _bullets.Count - 1; bi >= 0; bi--)
            {
                var b = _bullets[bi];
                if (b.IsPlayerBullet) continue;
                if (b.Bounds.IntersectsWith(_player.Bounds) && _player.IsAlive)
                {
                    _player.TakeDamage(10);
                    _bullets.RemoveAt(bi);
                }
            }

            // Meteors vs player
            for (int mi = _meteors.Count - 1; mi >= 0; mi--)
            {
                if (_meteors[mi].Bounds.IntersectsWith(_player.Bounds) && _player.IsAlive)
                {
                    _player.TakeDamage(20);
                    Explode(_meteors[mi].Bounds);
                    _meteors.RemoveAt(mi);
                }
            }
        }

        void Explode(Rectangle r)
        {
            _explosions.Add(new Explosion(new Point(r.Left + r.Width / 2, r.Top + r.Height / 2)));
        }

        // ── Win/Lose Check ────────────────────────────────────────────────────────
        void CheckGameState()
        {
            // Player dead
            if (!_player.IsAlive)
            {
                _gameTimer.Stop();
                ShowGameOverScreen(false);
                return;
            }

            // All enemies dead
            bool allDead = true;
            foreach (var en in _enemies)
                if (en.IsAlive) { allDead = false; break; }

            if (allDead)
            {
                _gameTimer.Stop();
                ShowGameOverScreen(true);
            }
        }

        void ShowGameOverScreen(bool won)
        {
            // Allow remaining explosions to flash briefly
            var dlg = new FormGameOver(won, _score, _level);
            var result = dlg.ShowDialog(this);
            this.DialogResult = result;
            this.Close();
        }

        // ── Rendering ────────────────────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Background
            DrawHelper.DrawNebula(g, ClientSize.Width, ClientSize.Height, _nebulaOff);
            DrawHelper.DrawStars(g, _stars);

            // Game objects
            foreach (var en in _enemies)  if (en.IsAlive) en.Draw(g);
            foreach (var m  in _meteors)  m.Draw(g);
            foreach (var b  in _bullets)  b.Draw(g);
            foreach (var ex in _explosions) ex.Draw(g);
            _player.Draw(g);

            DrawHUD(g);

            if (_paused) DrawPauseOverlay(g);
        }

        void DrawHUD(Graphics g)
        {
            // Top bar
            DrawHelper.DrawHUDPanel(g, new Rectangle(0, 0, ClientSize.Width, 46));

            // Score
            using (var f = new Font("Consolas", 16, FontStyle.Bold))
            using (var b = new SolidBrush(Color.FromArgb(220, 180, 255, 255)))
                g.DrawString($"SCORE: {_score:D6}", f, b, 12, 10);

            // Level
            using (var f = new Font("Consolas", 14, FontStyle.Bold))
            using (var b = new SolidBrush(Color.FromArgb(200, 255, 200, 80)))
                g.DrawString($"LEVEL {_level}", f, b, ClientSize.Width / 2 - 40, 12);

            // Enemies left
            int alive = 0;
            foreach (var en in _enemies) if (en.IsAlive) alive++;
            using (var f = new Font("Consolas", 13, FontStyle.Regular))
            using (var b = new SolidBrush(Color.FromArgb(200, 255, 120, 80)))
                g.DrawString($"ENEMIES: {alive}", f, b, ClientSize.Width - 140, 12);

            // HP bar
            string hpLabel = "HP";
            var hpLabelR = new Rectangle(8, ClientSize.Height - 34, 26, 20);
            using (var f = new Font("Consolas", 10, FontStyle.Bold))
            using (var b = new SolidBrush(Color.FromArgb(180, 200, 255, 200)))
                g.DrawString(hpLabel, f, b, hpLabelR.X, hpLabelR.Y);

            var hpBar = new Rectangle(38, ClientSize.Height - 34, 200, 20);
            DrawHelper.DrawHealthBar(g, hpBar, _player.HPRatio);

            // Controls hint
            using (var f = new Font("Consolas", 9, FontStyle.Regular))
            using (var b = new SolidBrush(Color.FromArgb(100, 200, 200, 200)))
                g.DrawString("← → Move  |  SPACE Fire  |  ESC Pause", f, b, ClientSize.Width - 300, ClientSize.Height - 32);
        }

        void DrawPauseOverlay(Graphics g)
        {
            using (var b = new SolidBrush(Color.FromArgb(140, 0, 0, 20)))
                g.FillRectangle(b, 0, 0, ClientSize.Width, ClientSize.Height);

            using (var f = new Font("Impact", 52, FontStyle.Bold))
            {
                string txt = "PAUSED";
                SizeF sz = g.MeasureString(txt, f);
                // shadow
                using (var sb = new SolidBrush(Color.FromArgb(100, 0, 0, 80)))
                    g.DrawString(txt, f, sb, (ClientSize.Width - sz.Width) / 2 + 3, ClientSize.Height / 2 - 60 + 3);
                using (var sb = new SolidBrush(Color.FromArgb(230, 100, 220, 255)))
                    g.DrawString(txt, f, sb, (ClientSize.Width - sz.Width) / 2, ClientSize.Height / 2 - 60);
            }
            using (var f = new Font("Consolas", 16, FontStyle.Regular))
            using (var b = new SolidBrush(Color.FromArgb(200, 200, 200, 200)))
            {
                string hint = "Press ESC to resume";
                SizeF sz = g.MeasureString(hint, f);
                g.DrawString(hint, f, b, (ClientSize.Width - sz.Width) / 2, ClientSize.Height / 2 + 10);
            }
        }

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGame));
			this.SuspendLayout();
			// 
			// FormGame
			// 
			this.ClientSize = new System.Drawing.Size(282, 253);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormGame";
			this.ResumeLayout(false);

		}
	}
}

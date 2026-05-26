using System;
using System.Drawing;

namespace SpaceShooter
{
    // ─── Player ──────────────────────────────────────────────────────────────────
    public class Player
    {
        public Rectangle Bounds;
        public int HP = GameConfig.PLAYER_MAX_HP;
        public bool MoveLeft, MoveRight;
        public int ShootCooldown = 0;
        public int InvincibleFrames = 0;   // brief invincibility after being hit

        public Player(int formW, int formH)
        {
            Bounds = new Rectangle(
                formW / 2 - GameConfig.PLAYER_WIDTH / 2,
                formH - GameConfig.PLAYER_HEIGHT - 60,
                GameConfig.PLAYER_WIDTH,
                GameConfig.PLAYER_HEIGHT);
        }

        public void Update(int formW)
        {
            if (MoveLeft  && Bounds.Left  > 10)      Bounds.X -= GameConfig.PLAYER_SPEED;
            if (MoveRight && Bounds.Right < formW-10) Bounds.X += GameConfig.PLAYER_SPEED;
            if (ShootCooldown > 0) ShootCooldown--;
            if (InvincibleFrames > 0) InvincibleFrames--;
        }

        public float HPRatio => (float)HP / GameConfig.PLAYER_MAX_HP;
        public bool IsAlive => HP > 0;
        public bool CanShoot => ShootCooldown == 0;
        public bool IsInvincible => InvincibleFrames > 0;

        public void TakeDamage(int dmg)
        {
            if (IsInvincible) return;
            HP -= dmg;
            if (HP < 0) HP = 0;
            InvincibleFrames = 45; // ~0.75 sec at 60fps
        }

        public void Draw(Graphics g)
        {
            // Flicker when invincible
            if (InvincibleFrames > 0 && (InvincibleFrames / 4) % 2 == 0) return;
            DrawHelper.DrawPlayerShip(g, Bounds);
        }
    }

    // ─── Enemy ───────────────────────────────────────────────────────────────────
    public class Enemy
    {
        public Rectangle Bounds;
        public EnemyType Type;
        public bool IsAlive = true;
        public int HP;
        public int MaxHP;
        public string Direction = "Right";
        public int FireCooldown;
        public int RotAngle = 0; // for green saucer spin

        // Dive behaviour (level 2)
        public bool Diving = false;
        public int DiveTarget;
        public float DiveY;

        private int _speed;
        private static Random _rng = new Random();

        public Enemy(int x, int y, EnemyType type, int speed, int hp, int fireInterval)
        {
            Type = type;
            HP = hp;
            MaxHP = hp;
            _speed = speed;
            FireCooldown = _rng.Next(0, fireInterval);
            Bounds = new Rectangle(x, y, GameConfig.ENEMY_WIDTH, GameConfig.ENEMY_HEIGHT);
        }

        public void Update(int formW, int formH, int fireInterval)
        {
            // Horizontal patrol
            if (!Diving)
            {
                if (Direction == "Right") Bounds.X += _speed;
                else                      Bounds.X -= _speed;

                if (Bounds.Right >= formW - 5) Direction = "Left";
                if (Bounds.Left  <= 5)         Direction = "Right";

                // Level 2: occasionally dive toward player
                if (_speed >= GameConfig.ENEMY_SPEED_L2 && _rng.Next(0, 3000) < 1)
                {
                    Diving = true;
                    DiveY = Bounds.Y;
                    DiveTarget = formH / 2;
                }
            }
            else
            {
                Bounds.Y += 4;
                if (Bounds.Y >= DiveTarget)
                {
                    Diving = false;
                    Bounds.Y = (int)DiveY;
                }
            }

            if (Type == EnemyType.Green) RotAngle = (RotAngle + 1) % 360;

            if (FireCooldown > 0) FireCooldown--;
        }

        public bool CanFire() { return FireCooldown <= 0; }
        public void ResetFireCooldown(int interval) { FireCooldown = interval; }

        public void TakeDamage(int dmg)
        {
            HP -= dmg;
            if (HP <= 0) IsAlive = false;
        }

        public void Draw(Graphics g)
        {
            DrawHelper.DrawEnemyShip(g, Bounds, Type);

            // Mini health bar above enemy
            if (MaxHP > 1)
            {
                var hbr = new Rectangle(Bounds.Left, Bounds.Top - 8, Bounds.Width, 5);
                DrawHelper.DrawHealthBar(g, hbr, (float)HP / MaxHP);
            }
        }
    }

    // ─── Bullet ──────────────────────────────────────────────────────────────────
    public class Bullet
    {
        public Rectangle Bounds;
        public int SpeedY;
        public bool IsPlayerBullet;
        public EnemyType EType; // only relevant for enemy bullets
        public bool Active = true;

        public Bullet(int cx, int cy, int speedY, bool isPlayer, EnemyType etype = EnemyType.Red)
        {
            IsPlayerBullet = isPlayer;
            SpeedY = speedY;
            EType = etype;
            int w = isPlayer ? 8 : 7;
            int h = isPlayer ? 22 : 18;
            Bounds = new Rectangle(cx - w / 2, cy, w, h);
        }

        public void Update() { Bounds.Y += SpeedY; }

        public bool IsOffScreen(int formH) => Bounds.Bottom < -30 || Bounds.Top > formH + 30;

        public void Draw(Graphics g)
        {
            if (IsPlayerBullet) DrawHelper.DrawPlayerBullet(g, Bounds);
            else                DrawHelper.DrawEnemyBullet(g, Bounds, EType);
        }
    }

    // ─── Meteor ──────────────────────────────────────────────────────────────────
    public class Meteor
    {
        public Rectangle Bounds;
        public int SpeedY;
        public int SpeedX;
        public int RotAngle = 0;
        public bool Active = true;

        private static Random _rng = new Random();

        public Meteor(int formW, int speedY)
        {
            int size = _rng.Next(28, 52);
            SpeedY = speedY;
            SpeedX = _rng.Next(-2, 3);
            Bounds = new Rectangle(_rng.Next(10, formW - 10 - size), -size, size, size);
        }

        public void Update(int formW)
        {
            Bounds.Y += SpeedY;
            Bounds.X += SpeedX;
            RotAngle = (RotAngle + 2) % 360;
            if (Bounds.Left < 0) SpeedX = Math.Abs(SpeedX);
            if (Bounds.Right > formW) SpeedX = -Math.Abs(SpeedX);
        }

        public bool IsOffScreen(int formH) => Bounds.Top > formH + 60;

        public void Draw(Graphics g) => DrawHelper.DrawMeteor(g, Bounds, RotAngle);
    }

    // ─── Explosion ───────────────────────────────────────────────────────────────
    public class Explosion
    {
        public Point Center;
        public int Frame = 0;
        public const int MaxFrames = 24;
        public bool Done => Frame >= MaxFrames;

        public Explosion(Point c) { Center = c; }
        public void Update() { Frame++; }
        public void Draw(Graphics g) => DrawHelper.DrawExplosion(g, Center, Frame, MaxFrames);
    }

    // ─── Score Entry ─────────────────────────────────────────────────────────────
    public class ScoreEntry
    {
        public string Name;
        public int Score;
        public int Level;
        public DateTime Date;

        public ScoreEntry(string name, int score, int level)
        {
            Name = name; Score = score; Level = level; Date = DateTime.Now;
        }
    }
}

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SpaceShooter
{
    /// <summary>
    /// All game art is drawn procedurally with GDI+.
    /// No external image files are required.
    /// </summary>
    public static class DrawHelper
    {
        // ─── Player Ship ────────────────────────────────────────────────────────
        public static void DrawPlayerShip(Graphics g, Rectangle r)
        {
            // Engine glow
            using (var glow = new LinearGradientBrush(
                new Point(r.Left, r.Bottom - 20), new Point(r.Left, r.Bottom + 20),
                Color.FromArgb(180, 0, 120, 255), Color.Transparent))
            {
                g.FillEllipse(glow, r.Left + r.Width / 2 - 14, r.Bottom - 10, 28, 22);
            }

            // Main body
            var body = new PointF[]
            {
                new PointF(r.Left + r.Width / 2f, r.Top),
                new PointF(r.Right - 6, r.Top + r.Height * 0.55f),
                new PointF(r.Right - 2, r.Bottom),
                new PointF(r.Left + 2,  r.Bottom),
                new PointF(r.Left + 6,  r.Top + r.Height * 0.55f),
            };
            using (var b = new LinearGradientBrush(new Point(r.Left, r.Top), new Point(r.Right, r.Bottom),
                Color.FromArgb(60, 160, 255), Color.FromArgb(10, 60, 180)))
                g.FillPolygon(b, body);

            using (var p = new Pen(Color.FromArgb(120, 220, 255), 1.5f))
                g.DrawPolygon(p, body);

            // Cockpit
            var cockpit = new RectangleF(r.Left + r.Width / 2f - 11, r.Top + 8, 22, 26);
            using (var b = new LinearGradientBrush(cockpit,
                Color.FromArgb(180, 200, 255, 255), Color.FromArgb(80, 0, 180, 255),
                LinearGradientMode.Vertical))
                g.FillEllipse(b, cockpit);
            using (var p = new Pen(Color.FromArgb(200, 100, 220, 255), 1f))
                g.DrawEllipse(p, cockpit);

            // Left wing
            var lw = new PointF[]
            {
                new PointF(r.Left + 6,  r.Top + r.Height * 0.55f),
                new PointF(r.Left,      r.Bottom - 10),
                new PointF(r.Left + 2,  r.Bottom),
                new PointF(r.Left + 18, r.Top + r.Height * 0.7f),
            };
            using (var b = new SolidBrush(Color.FromArgb(30, 100, 220)))
                g.FillPolygon(b, lw);
            using (var p = new Pen(Color.FromArgb(80, 160, 255), 1f))
                g.DrawPolygon(p, lw);

            // Right wing
            var rw = new PointF[]
            {
                new PointF(r.Right - 6,  r.Top + r.Height * 0.55f),
                new PointF(r.Right,      r.Bottom - 10),
                new PointF(r.Right - 2,  r.Bottom),
                new PointF(r.Right - 18, r.Top + r.Height * 0.7f),
            };
            using (var b = new SolidBrush(Color.FromArgb(30, 100, 220)))
                g.FillPolygon(b, rw);
            using (var p = new Pen(Color.FromArgb(80, 160, 255), 1f))
                g.DrawPolygon(p, rw);

            // Engine trail
            DrawEngineTrail(g, r.Left + r.Width / 2 - 8, r.Bottom, 16, 22);
        }

        static void DrawEngineTrail(Graphics g, int x, int y, int w, int h)
        {
            using (var b = new LinearGradientBrush(
                new Point(x, y), new Point(x, y + h),
                Color.FromArgb(200, 0, 180, 255), Color.Transparent))
                g.FillEllipse(b, x, y, w, h);

            using (var b2 = new LinearGradientBrush(
                new Point(x + 3, y), new Point(x + 3, y + h / 2),
                Color.FromArgb(220, 255, 255, 255), Color.Transparent))
                g.FillEllipse(b2, x + 4, y, w - 8, h / 2);
        }

        // ─── Enemy Ships ─────────────────────────────────────────────────────────
        public static void DrawEnemyShip(Graphics g, Rectangle r, EnemyType type)
        {
            switch (type)
            {
                case EnemyType.Red:    DrawRedEnemy(g, r);    break;
                case EnemyType.Purple: DrawPurpleEnemy(g, r); break;
                case EnemyType.Green:  DrawGreenEnemy(g, r);  break;
            }
        }

        static void DrawRedEnemy(Graphics g, Rectangle r)
        {
            // Body
            var body = new PointF[]
            {
                new PointF(r.Left + r.Width/2f, r.Bottom),
                new PointF(r.Right - 4, r.Top + r.Height * 0.45f),
                new PointF(r.Right - 10, r.Top),
                new PointF(r.Left + 10,  r.Top),
                new PointF(r.Left + 4,  r.Top + r.Height * 0.45f),
            };
            using (var b = new LinearGradientBrush(new Point(r.Left, r.Top), new Point(r.Right, r.Bottom),
                Color.FromArgb(255, 60, 60), Color.FromArgb(140, 20, 20)))
                g.FillPolygon(b, body);
            using (var p = new Pen(Color.FromArgb(255, 120, 80), 1.5f))
                g.DrawPolygon(p, body);

            // Cockpit
            var ck = new RectangleF(r.Left + r.Width / 2f - 10, r.Top + 8, 20, 18);
            using (var b = new SolidBrush(Color.FromArgb(180, 255, 80, 80)))
                g.FillEllipse(b, ck);

            // Wings
            DrawSimpleWings(g, r, Color.FromArgb(200, 40, 40), Color.FromArgb(255, 100, 60));

            // Engine glow (downward)
            using (var b = new LinearGradientBrush(
                new Point(r.Left, r.Top), new Point(r.Left, r.Top + 18),
                Color.FromArgb(180, 255, 60, 0), Color.Transparent))
                g.FillEllipse(b, r.Left + r.Width / 2 - 12, r.Top - 14, 24, 18);
        }

        static void DrawPurpleEnemy(Graphics g, Rectangle r)
        {
            var body = new PointF[]
            {
                new PointF(r.Left + r.Width / 2f, r.Bottom),
                new PointF(r.Right,               r.Top + r.Height * 0.6f),
                new PointF(r.Right - 8,           r.Top),
                new PointF(r.Left + 8,            r.Top),
                new PointF(r.Left,                r.Top + r.Height * 0.6f),
            };
            using (var b = new LinearGradientBrush(new Point(r.Left, r.Top), new Point(r.Right, r.Bottom),
                Color.FromArgb(180, 60, 255), Color.FromArgb(90, 20, 160)))
                g.FillPolygon(b, body);
            using (var p = new Pen(Color.FromArgb(220, 140, 255), 1.5f))
                g.DrawPolygon(p, body);

            var ck = new RectangleF(r.Left + r.Width / 2f - 11, r.Top + 6, 22, 22);
            using (var b = new SolidBrush(Color.FromArgb(160, 200, 100, 255)))
                g.FillEllipse(b, ck);

            DrawSimpleWings(g, r, Color.FromArgb(130, 30, 200), Color.FromArgb(180, 80, 255));

            using (var b = new LinearGradientBrush(
                new Point(r.Left, r.Top), new Point(r.Left, r.Top + 18),
                Color.FromArgb(180, 160, 0, 255), Color.Transparent))
                g.FillEllipse(b, r.Left + r.Width / 2 - 12, r.Top - 14, 24, 18);
        }

        static void DrawGreenEnemy(Graphics g, Rectangle r)
        {
            // Saucer shape
            var ellRect = new RectangleF(r.Left + 4, r.Top + r.Height * 0.35f, r.Width - 8, r.Height * 0.5f);
            using (var b = new LinearGradientBrush(ellRect,
                Color.FromArgb(60, 220, 80), Color.FromArgb(20, 100, 30), LinearGradientMode.Vertical))
                g.FillEllipse(b, ellRect);
            using (var p = new Pen(Color.FromArgb(100, 255, 120), 1.5f))
                g.DrawEllipse(p, ellRect);

            // Dome
            var dome = new RectangleF(r.Left + r.Width / 2f - 16, r.Top, 32, r.Height * 0.55f);
            using (var b = new LinearGradientBrush(dome,
                Color.FromArgb(180, 80, 255, 120), Color.FromArgb(60, 30, 180, 60), LinearGradientMode.Vertical))
                g.FillEllipse(b, dome);
            using (var p = new Pen(Color.FromArgb(120, 200, 100), 1f))
                g.DrawEllipse(p, dome);

            // Lights
            for (int i = 0; i < 4; i++)
            {
                float lx = r.Left + 10 + i * (r.Width - 20) / 3f;
                float ly = r.Top + r.Height * 0.65f;
                using (var b = new SolidBrush(i % 2 == 0 ? Color.FromArgb(200, 255, 80, 0) : Color.FromArgb(200, 0, 200, 255)))
                    g.FillEllipse(b, lx - 4, ly - 4, 8, 8);
            }
        }

        static void DrawSimpleWings(Graphics g, Rectangle r, Color fill, Color edge)
        {
            var lw = new PointF[]
            {
                new PointF(r.Left + 6,            r.Top + r.Height * 0.45f),
                new PointF(r.Left,                r.Bottom - 8),
                new PointF(r.Left + r.Width/4f,   r.Top + r.Height * 0.65f),
            };
            var rw = new PointF[]
            {
                new PointF(r.Right - 6,            r.Top + r.Height * 0.45f),
                new PointF(r.Right,                r.Bottom - 8),
                new PointF(r.Right - r.Width/4f,   r.Top + r.Height * 0.65f),
            };
            using (var b = new SolidBrush(fill))
            { g.FillPolygon(b, lw); g.FillPolygon(b, rw); }
            using (var p = new Pen(edge, 1f))
            { g.DrawPolygon(p, lw); g.DrawPolygon(p, rw); }
        }

        // ─── Bullets ─────────────────────────────────────────────────────────────
        public static void DrawPlayerBullet(Graphics g, Rectangle r)
        {
            using (var b = new LinearGradientBrush(
                new Point(r.Left, r.Bottom), new Point(r.Left, r.Top),
                Color.FromArgb(60, 180, 255), Color.FromArgb(255, 255, 255)))
            {
                g.FillEllipse(b, r);
            }
            // glow ring
            using (var p = new Pen(Color.FromArgb(80, 0, 200, 255), 1.5f))
                g.DrawEllipse(p, r.Left - 2, r.Top - 2, r.Width + 4, r.Height + 4);
        }

        public static void DrawEnemyBullet(Graphics g, Rectangle r, EnemyType type)
        {
            Color c1, c2;
            switch (type)
            {
                case EnemyType.Purple: c1 = Color.FromArgb(220, 80, 255); c2 = Color.White; break;
                case EnemyType.Green:  c1 = Color.FromArgb(60, 255, 80);  c2 = Color.White; break;
                default:               c1 = Color.FromArgb(255, 80, 60);  c2 = Color.FromArgb(255, 200, 100); break;
            }
            using (var b = new LinearGradientBrush(
                new Point(r.Left, r.Top), new Point(r.Left, r.Bottom), c2, c1))
                g.FillEllipse(b, r);
        }

        // ─── Meteor ──────────────────────────────────────────────────────────────
        public static void DrawMeteor(Graphics g, Rectangle r, int rotAngle)
        {
            g.TranslateTransform(r.Left + r.Width / 2f, r.Top + r.Height / 2f);
            g.RotateTransform(rotAngle);

            var pts = new PointF[]
            {
                new PointF(0,           -r.Height / 2f),
                new PointF(r.Width/2f,  -r.Height / 4f),
                new PointF(r.Width*0.4f, r.Height / 4f),
                new PointF(0,            r.Height / 2f),
                new PointF(-r.Width*0.45f, r.Height/3f),
                new PointF(-r.Width/2f,  -r.Height/5f),
            };

            var bounds = new RectangleF(-r.Width/2f, -r.Height/2f, r.Width, r.Height);
            using (var b = new LinearGradientBrush(bounds,
                Color.FromArgb(160, 100, 60), Color.FromArgb(80, 50, 30), LinearGradientMode.ForwardDiagonal))
                g.FillPolygon(b, pts);
            using (var p = new Pen(Color.FromArgb(200, 130, 80), 1.5f))
                g.DrawPolygon(p, pts);

            // craters
            using (var b = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
            {
                g.FillEllipse(b, -r.Width / 4f, -r.Height / 6f, r.Width / 5f, r.Height / 6f);
                g.FillEllipse(b, r.Width / 8f, r.Height / 10f, r.Width / 6f, r.Height / 7f);
            }

            g.ResetTransform();
        }

        // ─── Explosion ───────────────────────────────────────────────────────────
        public static void DrawExplosion(Graphics g, Point center, int frame, int maxFrames)
        {
            float progress = (float)frame / maxFrames;
            int radius = Math.Max(2, (int)(50 * progress));  // never 0
            int alpha  = Math.Max(1, (int)(255 * (1f - progress)));

            // outer ring
            using (var p = new Pen(Color.FromArgb(alpha, 255, 150, 0), 3f))
                g.DrawEllipse(p, center.X - radius, center.Y - radius, radius * 2, radius * 2);

            // inner burst — points must differ or GDI+ throws OutOfMemoryException
            int inner = Math.Max(2, (int)(30 * progress));
            using (var b = new LinearGradientBrush(
                new Point(center.X - inner, center.Y - inner),
                new Point(center.X + inner + 1, center.Y + inner + 1),
                Color.FromArgb(alpha, 255, 220, 0),
                Color.FromArgb(Math.Max(1, alpha / 2), 255, 60, 0)))
                g.FillEllipse(b, center.X - inner, center.Y - inner, inner * 2, inner * 2);

            // sparks
            var rng = new Random(frame * 7);
            for (int i = 0; i < 8; i++)
            {
                double angle = i * Math.PI / 4 + rng.NextDouble() * 0.3;
                float dist = radius * 0.8f;
                float sx = center.X + (float)(Math.Cos(angle) * dist);
                float sy = center.Y + (float)(Math.Sin(angle) * dist);
                using (var p = new Pen(Color.FromArgb(alpha, 255, 200, 50), 2f))
                    g.DrawLine(p, center.X, center.Y, sx, sy);
            }
        }

        // ─── Stars ───────────────────────────────────────────────────────────────
        public static void DrawStars(Graphics g, Star[] stars)
        {
            foreach (var s in stars)
            {
                int a = (int)(80 + s.Brightness * 175);
                using (var b = new SolidBrush(Color.FromArgb(a, s.Color)))
                    g.FillEllipse(b, s.X - s.Size / 2, s.Y - s.Size / 2, s.Size, s.Size);
            }
        }

        // ─── Nebula background ───────────────────────────────────────────────────
        public static void DrawNebula(Graphics g, int w, int h, int offset)
        {
            // Deep space gradient
            using (var b = new LinearGradientBrush(
                new Point(0, 0), new Point(0, h),
                Color.FromArgb(8, 6, 30), Color.FromArgb(5, 3, 18)))
                g.FillRectangle(b, 0, 0, w, h);

            // Nebula clouds
            DrawNebulaCloud(g, 100, (120 + offset / 3) % (h + 200) - 100, 260, 180, Color.FromArgb(15, 60, 0, 120));
            DrawNebulaCloud(g, 500, (300 + offset / 4) % (h + 200) - 100, 220, 160, Color.FromArgb(15, 0, 40, 100));
            DrawNebulaCloud(g, 300, (500 + offset / 5) % (h + 200) - 100, 300, 200, Color.FromArgb(12, 80, 20, 60));
        }

        static void DrawNebulaCloud(Graphics g, int x, int y, int w, int h, Color c)
        {
            using (var path = new System.Drawing.Drawing2D.GraphicsPath())
            {
                path.AddEllipse(x, y, w, h);
                using (var b = new PathGradientBrush(path))
                {
                    b.CenterColor = c;
                    b.SurroundColors = new[] { Color.Transparent };
                    g.FillPath(b, path);
                }
            }
        }

        // ─── Health bar ──────────────────────────────────────────────────────────
        public static void DrawHealthBar(Graphics g, Rectangle r, float ratio)
        {
            // Background
            using (var b = new SolidBrush(Color.FromArgb(60, 0, 0, 0)))
                g.FillRectangle(b, r);

            // Fill
            int fw = (int)(r.Width * ratio);
            if (fw > 0)
            {
                Color c1 = ratio > 0.5f ? Color.FromArgb(0, 220, 80) :
                           ratio > 0.25f ? Color.Orange : Color.OrangeRed;
                Color c2 = ratio > 0.5f ? Color.FromArgb(0, 150, 40) :
                           ratio > 0.25f ? Color.DarkOrange : Color.Red;
                using (var b = new LinearGradientBrush(r, c1, c2, LinearGradientMode.Vertical))
                    g.FillRectangle(b, r.Left, r.Top, fw, r.Height);
            }

            // Border
            using (var p = new Pen(Color.FromArgb(120, 200, 200, 255), 1.5f))
                g.DrawRectangle(p, r);
        }

        // ─── HUD Panel ───────────────────────────────────────────────────────────
        public static void DrawHUDPanel(Graphics g, Rectangle r)
        {
            using (var b = new SolidBrush(Color.FromArgb(100, 5, 10, 30)))
                g.FillRectangle(b, r);
            using (var p = new Pen(Color.FromArgb(60, 80, 160, 255), 1f))
                g.DrawRectangle(p, r);
        }
    }

    // ─── Supporting data types ───────────────────────────────────────────────────
    public class Star
    {
        public float X, Y, Size, Brightness;
        public Color Color;
        public float Speed;
    }

    public enum EnemyType { Red, Purple, Green }
}

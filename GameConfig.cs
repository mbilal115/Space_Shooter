namespace SpaceShooter
{
    public static class GameConfig
    {
        // Form size
        public const int FORM_WIDTH  = 800;
        public const int FORM_HEIGHT = 700;

        // Player
        public const int PLAYER_SPEED    = 8;
        public const int PLAYER_WIDTH    = 60;
        public const int PLAYER_HEIGHT   = 70;
        public const int PLAYER_MAX_HP   = 100;
        public const int PLAYER_BULLET_SPEED = 18;

        // Enemies
        public const int ENEMY_WIDTH   = 56;
        public const int ENEMY_HEIGHT  = 60;
        public const int ENEMY_SPEED_L1 = 6;
        public const int ENEMY_SPEED_L2 = 10;
        public const int ENEMY_BULLET_SPEED_L1 = 8;
        public const int ENEMY_BULLET_SPEED_L2 = 13;

        // Timer
        public const int TICK_MS = 16; // ~60 fps

        // Levels
        public const int LEVEL1_ENEMY_COUNT = 6;
        public const int LEVEL2_ENEMY_COUNT = 10;

        // Fire intervals (in ticks)
        public const int ENEMY_FIRE_INTERVAL_L1 = 90;
        public const int ENEMY_FIRE_INTERVAL_L2 = 55;

        // Meteor
        public const int METEOR_INTERVAL_L1 = 180;
        public const int METEOR_INTERVAL_L2 = 100;
        public const int METEOR_SPEED_L1 = 4;
        public const int METEOR_SPEED_L2 = 7;

        // Score
        public const int SCORE_ENEMY     = 100;
        public const int SCORE_METEOR    = 25;
        public const int SCORE_SURVIVE   = 1; // per tick
    }
}

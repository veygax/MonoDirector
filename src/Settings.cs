namespace NEP.MonoDirector
{
    public static class Settings
    {
        public static class Camera
        {
            public static float SlowSpeed;
            public static float FastSpeed;
            public static float RotationSmoothness;
            public static bool UseHeadCamera;
            public static bool LockXAxis;
            public static bool LockYAxis;
            public static bool LockZAxis;
            public static bool KinematicOnRelease;
        }

        public static class World
        {
            public static int Delay = 2;
            public static bool UseMicrophone = false;
            public static bool MicPlayback = false;
            public static float PlaybackRate = 1f;
            public static float FPS = 60f;
            public static bool IgnoreSlowmo = false;
            public static bool TemporalScaling = false;
        }

        public static class Debug
        {
            public static bool DebugEnabled = true;
            public static bool UseKeys = true;
        }
    }
}

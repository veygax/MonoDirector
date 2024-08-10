namespace NEP.MonoDirector.State.Playback
{
    public sealed class PrePlaybackState : PlayheadState
    {
        private float _countdownTimer;
        private float _currentCountdown;

        public override void Start()
        {
            _currentCountdown = Settings.World.delay;
            _countdownTimer = 1f;
        }

        public override void Process()
        {

        }

        public override void Stop()
        {
            
        }
    }
}

using UnityEngine;

using NEP.MonoDirector.Audio;
using NEP.MonoDirector.Core;

namespace NEP.MonoDirector.State.Playback
{
    public sealed class PrePlaybackState : PlayheadState
    {
        private float _countdownTimer;
        private int _currentCountdown;

        private bool _finishedCountdown;
        private bool _countdownTickReached;

        public override void Start()
        {
            _currentCountdown = Settings.World.Delay;
            _countdownTimer = 1f;
            _finishedCountdown = false;
            _countdownTickReached = false;
        }

        public override void Process()
        {
            if (!_finishedCountdown)
            {
                AdvanceCountdown();
                return;
            }

            Stop();
        }

        public override void Stop()
        {
            Director.Instance.SetPlayState(new PlaybackState());
        }

        private void AdvanceCountdown()
        {
            if (_currentCountdown > 0)
            {
                if (_countdownTimer > 0f)
                {
                    _countdownTimer -= Time.deltaTime;
                    return;
                }

                if (!_countdownTickReached)
                {
                    FeedbackSFX.Instance.Beep();
                    _countdownTimer = 1f;
                    _currentCountdown--;
                    _countdownTickReached = true;
                }

                _countdownTickReached = false;
                return;
            }

            _finishedCountdown = true;
        }
    }
}

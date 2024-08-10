using UnityEngine;

using NEP.MonoDirector.Audio;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Actors;

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
            Director.Instance.Playhead.Reset();
            InitializeCountdown();
            PositionActors();
        }

        public override void Process()
        {
            if (!_finishedCountdown)
            {
                AdvanceCountdown();
                return;
            }

            Director.Instance.SetPlayState(new PlaybackState());
        }

        private void InitializeCountdown()
        {
            _currentCountdown = Settings.World.Delay;
            _countdownTimer = 1f;
            _finishedCountdown = false;
            _countdownTickReached = false;
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

            FeedbackSFX.Instance.BeepHigh();
            _finishedCountdown = true;
        }

        private void PositionActors()
        {
            foreach (Actor actor in Director.Instance.Cast)
            {
                actor.OnSceneBegin();
            }

            foreach (Prop prop in Director.Instance.WorldProps)
            {
                prop.OnSceneBegin();
                prop.gameObject.SetActive(true);
            }
        }
    }
}

using System;

using UnityEngine;

using NEP.MonoDirector.Audio;
using NEP.MonoDirector.Core;

namespace NEP.MonoDirector.State.Playback
{
    public sealed class PlaybackState : PlayheadState
    {
        public override void Start()
        {

        }

        public override void Process()
        {
            if (Director.Instance.Playhead.PlaybackTime > Director.Instance.Playhead.TakeTime)
            {
                FeedbackSFX.Instance.Beep();
                Director.Instance.SetPlayState(null);
                return;
            }

            Director.Instance.AnimateAll();
            Director.Instance.Playhead.Move(Settings.World.PlaybackRate * Time.deltaTime);
            Events.OnPlaybackTick?.Invoke();
        }

        public override void Stop()
        {

        }
    }
}

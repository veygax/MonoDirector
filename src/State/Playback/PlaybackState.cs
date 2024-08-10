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
            if (Director.Instance.Playhead == null )
            {
                throw new NullReferenceException("Playhead is missing from the Director!");
            }

            FeedbackSFX.Instance.BeepHigh();
        }

        public override void Process()
        {
            if (Director.Instance.Playhead == null)
            {
                Stop();
                return;
            }

            Director.Instance.AnimateAll();
            Director.Instance.Playhead.Move(Settings.World.playbackRate * Time.deltaTime);
            Events.OnPlaybackTick?.Invoke();
        }

        public override void Stop()
        {

        }
    }
}

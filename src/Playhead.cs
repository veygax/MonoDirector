using NEP.MonoDirector.Core;
using NEP.MonoDirector.State;

namespace NEP.MonoDirector
{
    public sealed class Playhead
    {
        public float PlaybackTime => _playbackTime;
        public float PlaybackRate => Settings.World.playbackRate;

        private float _playbackTime;

        public void Reset()
        {
            _playbackTime = 0f;
        }

        public void Move(float amount)
        {
            if (_playbackTime <= 0f)
            {
                _playbackTime = 0f;
            }

            if (_playbackTime >= Recorder.instance.TakeTime)
            {
                _playbackTime = Recorder.instance.TakeTime;
            }

            _playbackTime += amount;
        }
    }
}

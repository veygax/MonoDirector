using NEP.MonoDirector.State.Recording;

namespace NEP.MonoDirector.Core
{
    public sealed class Playhead
    {
        public float PlaybackTime => _playbackTime;
        public float PlaybackRate => Settings.World.PlaybackRate;

        public float RecordingTime => _recordingTime;

        public float TakeTime => _takeTime;

        private float _playbackTime;

        private float _recordingTime;

        private float _takeTime;

        public void Reset()
        {
            _playbackTime = 0f;
            _recordingTime = 0f;
        }

        public void Move(float amount)
        {
            if (_playbackTime < 0f)
            {
                _playbackTime = 0f;
            }

            if (_playbackTime > _takeTime)
            {
                _playbackTime = _takeTime;
            }

            _playbackTime += amount;
        }

        public void SetTakeTime(float time)
        {
            _takeTime = time;
        }

        public void SetRecordingTime(RecordingState recordingState)
        {
            _recordingTime = recordingState.RecordingTime;
        }
    }
}

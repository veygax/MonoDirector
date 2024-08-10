using UnityEngine;

using AudioImportLib;

using MarrowAudio = BoneLib.Audio;

namespace NEP.MonoDirector.Audio
{
    public sealed class FeedbackSFX
    {
        public FeedbackSFX()
        {
            Instance = this;
            OnEnable();
        }

        ~FeedbackSFX()
        {
            OnDisable();
        }

        public static FeedbackSFX Instance { get; private set; }

        private AudioClip sfx_preroll = API.LoadAudioClip(Constants.dirSFX + "preroll.wav", true);
        private AudioClip sfx_postroll = API.LoadAudioClip(Constants.dirSFX + "postroll.wav", true);
        private AudioClip sfx_beep = API.LoadAudioClip(Constants.dirSFX + "beep.wav", true);
        private AudioClip sfx_linkedaudio = API.LoadAudioClip(Constants.dirSFX + "linkaudio.wav", true);

        private void OnEnable()
        {
            Events.OnStopRecording += Beep;
            Events.OnStopPlayback += Beep;
            Events.OnTimerCountdown += Beep;
        }

        private void OnDisable()
        {
            Events.OnStopRecording -= Beep;
            Events.OnStopPlayback -= Beep;
            Events.OnTimerCountdown -= Beep;
        }

        private void Preroll()
        {
            MarrowAudio.Play2DOneShot(sfx_preroll, MarrowAudio.UI, 1f, 1f);
        }

        public void Beep()
        {
            MarrowAudio.Play2DOneShot(sfx_beep, MarrowAudio.UI, 1f, 1f);
        }

        public void BeepLow()
        {
            MarrowAudio.Play2DOneShot(sfx_beep, MarrowAudio.UI, 1f, 0.5f);
        }

        public void BeepHigh()
        {
            MarrowAudio.Play2DOneShot(sfx_beep, MarrowAudio.UI, 1f, 2f);
        }

        public void LinkAudio()
        {
            MarrowAudio.Play2DOneShot(sfx_linkedaudio, MarrowAudio.UI, 0.5f, 1f);
        }

        private void Postroll() 
        {
            MarrowAudio.Play2DOneShot(sfx_postroll, MarrowAudio.UI, 1f, 1f);
        }
    }
}

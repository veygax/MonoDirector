using UnityEngine;

using Il2CppTMPro;

using NEP.MonoDirector.Core;
using NEP.MonoDirector.State;

namespace NEP.MonoDirector.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class InformationInterface : MonoBehaviour
    {
        public InformationInterface(System.IntPtr ptr) : base(ptr) { }

        public static InformationInterface Instance { get; private set; }

        public bool ShowUI
        {
            get
            {
                return showUI;
            }
            set
            {
                showUI = value;

                ShowIcons = showUI;
                ShowTimecode = showUI;
                ShowPlaymode = showUI;
            }
        }

        public bool ShowIcons
        {
            get
            {
                return showIcons;
            }
            set
            {
                showIcons = value;
                microIconsObject?.SetActive(showIcons);
            }
        }

        public bool ShowTimecode
        {
            get
            {
                return showTimecode;
            }
            set
            {
                showTimecode = value;
                timecodeObject?.SetActive(showTimecode);
            }
        }

        public bool ShowPlaymode
        {
            get
            {
                return showPlaymode;
            }
            set
            {
                showPlaymode = value;
                playmodeObject?.SetActive(showPlaymode);
            }
        }

        private GameObject microIconsObject;
        private GameObject timecodeObject;
        private GameObject playmodeObject;
        private GameObject countdownObject;

        private GameObject micObject;
        private GameObject micOffObject;

        private TextMeshProUGUI timecodeText;
        private TextMeshProUGUI playmodeText;
        private TextMeshProUGUI countdownText;

        private Animator countdownAnimator;

        private PlayheadState playState;

        private bool showUI;
        private bool showIcons;
        private bool showTimecode;
        private bool showPlaymode;

        private void Awake()
        {
            Instance = this;

            microIconsObject = transform.Find("MicroIcons").gameObject;
            timecodeObject = transform.Find("Timecode").gameObject;
            playmodeObject = transform.Find("Playmode").gameObject;
            countdownObject = transform.Find("Countdown").gameObject;

            micObject = microIconsObject.transform.Find("Microphone/Mic").gameObject;
            micOffObject = microIconsObject.transform.Find("Microphone/Disabled").gameObject;

            timecodeText = timecodeObject.transform.Find("Time").GetComponent<TextMeshProUGUI>();
            playmodeText = playmodeObject.transform.Find("Mode").GetComponent<TextMeshProUGUI>();
            countdownText = countdownObject.transform.Find("Counter").GetComponent<TextMeshProUGUI>();

            countdownAnimator = countdownObject.GetComponent<Animator>();
        }

        private void Start()
        {
            Events.OnPlayStateSet += OnPlayStateSet;

            Events.OnPrePlayback += OnSceneStart;
            Events.OnPlaybackTick += OnSceneTick;
            Events.OnStopPlayback += OnSceneEnd;

            Events.OnPreRecord += OnSceneStart;
            Events.OnStartRecording += OnStartRecording;
            Events.OnRecordTick += OnSceneTick;
            Events.OnStopRecording += OnSceneEnd;

            Events.OnTimerCountdown += OnTimerCountdown;

            showIcons = false;
            showTimecode = false;
            showPlaymode = false;

            microIconsObject.SetActive(false);
            timecodeObject.SetActive(false);
            playmodeObject.SetActive(false);
            countdownObject.SetActive(false);
        }

        private void Update()
        {
            micOffObject.SetActive(!Settings.World.UseMicrophone);

            transform.position = Vector3.Lerp(transform.position, BoneLib.Player.Head.position + BoneLib.Player.Head.forward, 16f * Time.deltaTime);
            transform.LookAt(BoneLib.Player.Head);
        }

        public void OnSceneStart()
        {
            // TODO:
            // Adjust for new state machine implementation
            //if(Director.PlayState != PlayheadState.Prerecording)
            //{
            //    return;
            //}

            timecodeText.text = "0s";
            countdownObject.SetActive(true);
        }

        public void OnStartRecording()
        {
            countdownObject.SetActive(false);
        }

        public void OnSceneTick()
        {
            float time = 0f;

            // TODO:
            // Adjust for new state machine implementation
            //if(playState == PlayheadState.Playing)
            //{
            //time = Playback.Instance.PlaybackTime;
            //}

            //if(playState == PlayheadState.Recording)
            //{
            //time = Recorder.instance.RecordingTime;
            //}

            timecodeText.text = time.ToString("0.000") + "s";
        }

        public void OnSceneEnd()
        {
            
        }

        public void OnTimerCountdown()
        {
            // TODO:
            // Adjust for new state machine implementation
            //countdownObject.SetActive(false);
            //int counter = Director.PlayState == PlayheadState.Prerecording ? Recorder.instance.Countdown : Playback.Instance.Countdown;
            //int currentCountdown = Settings.World.delay - counter;
            //countdownText.text = currentCountdown.ToString();
            //countdownObject.SetActive(true);
            //countdownAnimator.Play("Countdown");
        }

        private void OnPlayStateSet(PlayheadState playState)
        {
            this.playState = playState;
            playmodeText.text = playState.ToString();
        }
    }
}

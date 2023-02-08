﻿using UnityEngine;

using NEP.MonoDirector.Cameras;
using NEP.MonoDirector.State;

using System.Collections.Generic;
using NEP.MonoDirector.Actors;

namespace NEP.MonoDirector.Core
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class Director : MonoBehaviour
    {
        public Director(System.IntPtr ptr) : base(ptr) { }

        public static Director instance { get; private set; }

        public Playback playback;
        public Recorder recorder;

        public static PlayState PlayState { get => playState; }
        public static CaptureState CaptureState { get => captureState; }

        public List<Actor> Cast;
        public List<ActorProp> WorldProps;
        public List<ActorProp> RecordingProps;

        public int WorldTick { get => worldTick; }

        private static PlayState playState;
        private static CaptureState captureState = CaptureState.CaptureActor;

        private CameraRig camera;

        private int worldTick;

        private void Awake()
        {
            instance = this;

            playback = new Playback();
            recorder = new Recorder();

            Cast = new List<Actor>();
            WorldProps = new List<ActorProp>();
            RecordingProps = new List<ActorProp>();
        }

        public void Play()
        {
            playState = PlayState.Playing;
            playback.BeginPlayback();
        }

        public void Pause()
        {
            playState = PlayState.Paused;
        }

        public void Record()
        {
            playState = PlayState.Recording;
            recorder.BeginRecording();
        }

        public void Stop()
        {
            playState = PlayState.Stopped;
        }

        public void SetCamera(CameraRig camera)
        {
            this.camera = camera;
        }

        public void RemoveActor(Actor actor)
        {
            for(int i = 0; i < Cast.Count; i++)
            {
                var castMember = Cast[i];

                if (castMember == actor)
                {
                    castMember.Delete();
                    Cast.Remove(actor);
                    return;
                }
            }
        }

        public void RemoveAllActors()
        {
            playState = PlayState.Stopped;

            for (int i = 0; i < Cast.Count; i++)
            {
                Cast[i].Delete();
            }

            Cast.Clear();
            worldTick = 0;
        }
        
        public void ClearScene()
        {
            RemoveAllActors();
            
            foreach(var prop in WorldProps)
            {
                prop.InteractableRigidbody.isKinematic = false;
                GameObject.Destroy(prop);
            }

            WorldProps.Clear();
        }
    }
}
   
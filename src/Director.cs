using System.Collections.Generic;

using UnityEngine;

using BoneLib;

using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Cameras;
using NEP.MonoDirector.State;
using NEP.MonoDirector.State.Playback;
// using NEP.MonoDirector.State.Recording;

namespace NEP.MonoDirector.Core
{
    public class Director
    {
        internal Director()
        {
            Instance = this;

            Playhead = new Playhead();

            Cast = new List<Actor>();
            NPCCast = new List<ActorNPC>();
            WorldProps = new List<Prop>();
            RecordingProps = new List<Prop>();
        }

        public static Director Instance { get; private set; }

        public Playhead Playhead { get; private set; }

        public FreeCamera Camera { get => _camera; }
        public CameraVolume Volume { get => _camera.GetComponent<CameraVolume>(); }

        public PlayheadState PlayState { get => _playState; }
        public PlayheadState LastPlayState { get => _lastPlayState; }
        public CaptureState CaptureState { get => _captureState; }

        public List<Actor> Cast;
        public List<ActorNPC> NPCCast;

        public List<Prop> WorldProps;
        public List<Prop> RecordingProps;
        public List<Prop> LastRecordedProps;

        public int WorldTick { get => _worldTick; }

        private PlayheadState _playState;
        private PlayheadState _lastPlayState;
        private CaptureState _captureState = CaptureState.CaptureActor;

        private FreeCamera _camera;

        private int _worldTick;

        public void Play()
        {
            SetPlayState(new PrePlaybackState());
        }

        public void Pause()
        {
            SetPlayState(PlayState.Paused);
        }

        public void Record()
        {
            Recorder.StartRecordRoutine();
        }

        public void Recast(Actor actor)
        {
            Vector3 actorPosition = actor.Frames[0].TransformFrames[0].position;
            Player.RigManager.Teleport(actorPosition, true);
            Player.RigManager.SwapAvatar(actor.ClonedAvatar);

            // Any props recorded by this actor must be removed if we're recasting
            // If we don't, the props will still play, but they will be floating in the air aimlessly.
            // Spooky!

            if (WorldProps.Count != 0)
            {
                foreach (var prop in WorldProps)
                {
                    if (prop.Actor == actor)
                    {
                        GameObject.Destroy(prop);
                    }
                }
            }

            Cast.Remove(actor);
            actor.Delete();

            Record();
        }

        public void Stop()
        {
            SetPlayState(PlayState.Stopped);
        }

        public void SetCamera(FreeCamera camera)
        {
            this._camera = camera;
        }

        public void AnimateAll()
        {
            foreach (var castMember in Cast)
            {
                AnimateActor(castMember);
            }

            foreach (var prop in WorldProps)
            {
                AnimateProp(prop);
            }
        }

        public void AnimateActor(Trackable actor)
        {
            if (actor != null)
            {
                actor.Act();
            }
        }

        public void AnimateProp(Prop prop)
        {
            if (prop != null)
            {
                prop.Act();
            }
        }

        public void RemoveActor(Actor actor)
        {
            Cast.Remove(actor);
            actor.Delete();
        }

        public void RemoveLastActor()
        {
            RemoveActor(Recorder.instance.LastActor);

            foreach(var prop in LastRecordedProps)
            {
                WorldProps.Remove(prop);
                prop.InteractableRigidbody.isKinematic = false;
                GameObject.Destroy(prop);
            }
        }

        public void RemoveAllActors()
        {
            for (int i = 0; i < Cast.Count; i++)
            {
                Cast[i].Delete();
            }

            Cast.Clear();
        }
        
        public void ClearLastProps()
        {
            foreach(var prop in LastRecordedProps)
            {
                prop.InteractableRigidbody.isKinematic = false;
                WorldProps.Remove(prop);
                GameObject.Destroy(prop);
            }

            LastRecordedProps.Clear();
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

        public void SetPlayState(PlayheadState state)
        {
            _lastPlayState = _playState;

            if (_lastPlayState != null)
            {
                _lastPlayState.Stop();
            }

            _playState = state;

            _playState.Start();

            Events.OnPlayStateSet?.Invoke(state);
        }

        internal void CleanUp()
        {
            Instance = null;
        }
    }
}
   
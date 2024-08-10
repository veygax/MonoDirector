using System.Collections.Generic;

using UnityEngine;

using BoneLib;

using Il2CppSLZ.Marrow.Utilities;

using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Cameras;
using NEP.MonoDirector.State;
using NEP.MonoDirector.State.Playback;
using NEP.MonoDirector.State.Recording;

using Avatar = Il2CppSLZ.VRMK.Avatar;

namespace NEP.MonoDirector.Core
{
    public sealed class Director
    {
        public Director()
        {
            Instance = this;

            Playhead = new Playhead();

            _cast = new List<Actor>();
            _worldProps = new List<Prop>();
            _recordingProps = new List<Prop>();

            MarrowGame.playerLoop._updateDelegate += new System.Action(() => ProcessActiveState());
        }

        public static Director Instance { get; private set; }

        public Playhead Playhead { get; private set; }

        public PlayheadState PlayState { get => _playState; }
        public PlayheadState LastPlayState { get => _lastPlayState; }
        public CaptureState CaptureState { get => _captureState; }

        public Actor ActiveActor => _activeActor;
        public Actor LastActor => _lastActor;

        public IReadOnlyList<Actor> Cast => _cast.AsReadOnly();

        public IReadOnlyList<Prop> WorldProps => _worldProps;
        public IReadOnlyList<Prop> RecordingProps => _recordingProps;
        public IReadOnlyList<Prop> LastRecordedProps => _lastRecordedProps;

        public int WorldTick { get => _worldTick; }

        private List<Actor> _cast;

        private List<Prop> _worldProps;
        private List<Prop> _recordingProps;
        private List<Prop> _lastRecordedProps;

        private Actor _activeActor;
        private Actor _lastActor;

        private PlayheadState _playState;
        private PlayheadState _lastPlayState;
        private CaptureState _captureState = CaptureState.CaptureActor;

        private FreeCamera _camera;

        private int _worldTick;

        public void ProcessActiveState()
        {
            try
            {
                if (_playState == null)
                {
                    return;
                }

                _playState.Process();
            }
            catch
            {
                throw;
            }
        }

        public void Play()
        {
            SetPlayState(new PrePlaybackState());
        }

        public void Pause()
        {
            SetPlayState(null);
        }

        public void Record()
        {
            SetPlayState(new RecordingState());
        }

        public void Stop()
        {
            // SetPlayState(PlayState.Stopped);
        }

        public void StageActor(Avatar avatar)
        {
            _lastActor = _activeActor;
            _activeActor = new Actor(avatar);
        }

        public void CastActor(Actor actor)
        {
            _cast.Add(actor);
        }

        public void CastActors(Actor[] actors)
        {
            _cast.AddRange(actors);
        }

        public void UncastActor(Actor actor)
        {
            _cast.Remove(actor);
        }

        public void RecastActor(Actor actor)
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
                        // For the record this only destroys the component.
                        // NOT the game object.
                        GameObject.Destroy(prop);
                    }
                }
            }

            RemoveActor(actor);

            Record();
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
            UncastActor(actor);
            actor.Delete();
        }

        public void RemoveLastActor()
        {
            // RemoveActor(Recorder.instance.LastActor);

            foreach(var prop in LastRecordedProps)
            {
                _worldProps.Remove(prop);
                prop.InteractableRigidbody.isKinematic = false;
                GameObject.Destroy(prop);
            }
        }

        public void RemoveAllActors()
        {
            for (int i = 0; i < _cast.Count; i++)
            {
                RemoveActor(_cast[i]);
            }

            _cast.Clear();
        }

        public void AddProp(Prop prop)
        {
            _worldProps.Add(prop);
        }

        public void AddRecordingProp(Prop prop)
        {
            _recordingProps.Add(prop);
        }

        public void AddProps(Prop[] props)
        {
            _worldProps.AddRange(props);
        }
        
        public void ClearLastProps()
        {
            foreach(var prop in LastRecordedProps)
            {
                prop.InteractableRigidbody.isKinematic = false;
                _worldProps.Remove(prop);
                GameObject.Destroy(prop);
            }

            _lastRecordedProps.Clear();
        }

        public void ClearScene()
        {
            RemoveAllActors();
            
            foreach(var prop in _worldProps)
            {
                prop.InteractableRigidbody.isKinematic = false;
                GameObject.Destroy(prop);
            }

            _worldProps.Clear();
        }

        public void SetPlayState(PlayheadState state)
        {
            _lastPlayState = _playState;

            if (_lastPlayState != null)
            {
                _lastPlayState.Stop();
            }

            _playState = state;

            if (_playState == null)
            {
                return;
            }

            _playState.Start();

            Events.OnPlayStateSet?.Invoke(state);
        }

        internal void CleanUp()
        {
            Instance = null;
        }
    }
}
   
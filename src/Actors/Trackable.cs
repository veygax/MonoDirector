using System;
using System.Collections.Generic;

using UnityEngine;

using NEP.MonoDirector.Data;
using NEP.MonoDirector.Core;

namespace NEP.MonoDirector.Actors
{
    public class Trackable
    {
        public Trackable()
        {
            objectFrames = new List<ObjectFrame>();
            actionFrames = new List<ActionFrame>();
        }

        public string ActorName { get => actorName; }
        public int ActorId { get => actorId; }

        protected string actorName;
        protected int actorId;

        protected Transform transform;
        protected List<ObjectFrame> objectFrames;
        protected List<ActionFrame> actionFrames;

        protected int stateTick;
        protected int recordedTicks;

        private ObjectFrame previousFrame;
        private ObjectFrame nextFrame;

        public virtual void OnSceneBegin()
        {
            foreach (ActionFrame actionFrame in actionFrames)
            {
                actionFrame.Reset();
            }
        }

        /// <summary>
        /// Updates the actor's pose on this recorded frame.
        /// </summary>
        public virtual void Act()
        {
            previousFrame = new ObjectFrame();
            nextFrame = new ObjectFrame();

            foreach (var frame in objectFrames)
            {
                previousFrame = nextFrame;
                nextFrame = frame;

                if (frame.frameTime > Director.Instance.Playhead.PlaybackTime)
                {
                    break;
                }
            }

            float gap = nextFrame.frameTime - previousFrame.frameTime;
            float head = Director.Instance.Playhead.PlaybackTime - previousFrame.frameTime;

            float delta = head / gap;

            transform.position = Vector3.Lerp(previousFrame.position, nextFrame.position, delta);
            transform.rotation = Quaternion.Slerp(previousFrame.rotation, nextFrame.rotation, delta);

            foreach (ActionFrame actionFrame in actionFrames)
            {
                if (Director.Instance.Playhead.PlaybackTime < actionFrame.timestamp)
                {
                    continue;
                }
                else
                {
                    actionFrame.Run();
                }
            }
        }

        public virtual void RecordFrame()
        {
            ObjectFrame objectFrame = new ObjectFrame()
            {
                transform = transform
            };

            objectFrames.Add(objectFrame);
        }

        public virtual void RecordAction(Action action)
        {
            // TODO:
            // Use new state machine system
            //if (Director.PlayState == State.PlayheadState.Recording)
            //{
            //    actionFrames.Add(new ActionFrame(action, Recorder.instance.RecordingTime));
            //}
        }

        public void SetTransform(Transform transform)
        {
            this.transform = transform;
        }

        public virtual void Delete()
        {
            
        }
    }
}
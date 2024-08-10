﻿using System;
using System.Collections.Generic;

using Il2CppInterop.Runtime;

using UnityEngine;

using Il2CppSLZ.Marrow;

using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;

namespace NEP.MonoDirector.Actors
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class Prop : MonoBehaviour
    {
        public Prop(System.IntPtr ptr) : base(ptr) { }

        public Trackable Actor { get => actor; }

        public List<ObjectFrame> PropFrames { get => propFrames; }
        public Rigidbody InteractableRigidbody { get => interactableRigidbody; }
        public bool isRecording;

        public static readonly Il2CppSystem.Type[] whitelistedTypes = new Il2CppSystem.Type[]
        {
            Il2CppType.Of<Gun>(),
            Il2CppType.Of<Magazine>(),
            Il2CppType.Of<ObjectDestructible>(),
            Il2CppType.Of<Atv>()
        };

        private Trackable actor;
        private Rigidbody interactableRigidbody;

        protected int stateTick;
        protected int recordedTicks;

        protected List<ObjectFrame> propFrames;
        protected List<ActionFrame> actionFrames;

        protected virtual void Awake()
        {
            propFrames = new List<ObjectFrame>();
            actionFrames = new List<ActionFrame>();
        }

        public static bool IsActorProp(Rigidbody rigidbody)
        {
            if (rigidbody == null)
            {
                return false;
            }

            if (rigidbody.isKinematic || rigidbody.gameObject.isStatic)
            {
                return false;
            }

            if (rigidbody.gameObject.layer == LayerMask.NameToLayer("EnemyColliders"))
            {
                return false;
            }

            if(rigidbody.GetComponent<InteractableHost>() == null)
            {
                return false;
            }

            if (rigidbody.GetComponent<Prop>() != null || rigidbody.GetComponent<WorldGrip>() != null)
            {
                return false;
            }

            return true;
        }

        public static bool EligibleWithType<T>(Rigidbody rigidbody)
        {
            return rigidbody.GetComponent<T>() != null;
        }

        public void SetRigidbody(Rigidbody rigidbody)
        {
            interactableRigidbody = rigidbody;
        }

        public void SetActor(Trackable actor)
        {
            this.actor = actor;
        }

        public void SetPhysicsActive(bool enable)
        {
            interactableRigidbody.isKinematic = enable;
        }

        public virtual void OnSceneBegin()
        {
            if(PropFrames == null)
            {
                return;
            }

            if(PropFrames.Count == 0)
            {
                return;
            }

            transform.position = PropFrames[0].position;
            transform.rotation = PropFrames[0].rotation;
            transform.localScale = PropFrames[0].scale;
            gameObject.SetActive(PropFrames[0].activeThisFrame);

            if(interactableRigidbody != null)
            {
                interactableRigidbody.isKinematic = true;
            }
        }

        public virtual void Act()
        {
            if(interactableRigidbody == null)
            {
                interactableRigidbody = GetComponent<Rigidbody>();
            }
            else
            {
                interactableRigidbody.isKinematic = true;
            }

            transform.position = Interpolator.InterpolatePosition(PropFrames);
            transform.rotation = Interpolator.InterpolateRotation(PropFrames);

            foreach(var actionFrame in actionFrames)
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

        public virtual void Record(int frame)
        {
            isRecording = true;

            ObjectFrame objectFrame = new ObjectFrame()
            {
                transform = transform,
                position = transform.position,
                rotation = transform.rotation,
                scale = transform.localScale,
                frameTime = Director.Instance.Playhead.RecordingTime,
                activeThisFrame = gameObject.activeSelf
            };

            if (frame == 0 || interactableRigidbody != null && interactableRigidbody.IsSleeping())
            {
                propFrames.Add(objectFrame);
            }
            else
            {
                propFrames.Add(objectFrame);
                recordedTicks++;
            }
        }

        public virtual void RecordAction(Action action)
        {
            // TODO:
            // Adjust for new state machine system
            //if (Director.PlayState == State.PlayheadState.Recording)
            //{
            //    if (!Director.instance.RecordingProps.Contains(this))
            //    {
            //        return;
            //    }

            //    actionFrames.Add(new ActionFrame(action, Recorder.instance.RecordingTime));
            //}
        }

        public void ResetTicks()
        {
            stateTick = 0;
        }
    }
}

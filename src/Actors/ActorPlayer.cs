﻿using System;
using System.Collections.Generic;

using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;

using SLZ.Props;

using UnityEngine;

namespace NEP.MonoDirector.Actors
{
    public class ActorPlayer : Actor
    {
        public ActorPlayer(SLZ.VRMK.Avatar avatar) : base()
        {
            playerAvatar = avatar;
            avatarBones = GetAvatarBones(playerAvatar);
            avatarFrames = new List<FrameGroup>();
        }

        public SLZ.VRMK.Avatar PlayerAvatar { get => playerAvatar; }
        public Transform[] AvatarBones { get => avatarBones; }

        protected List<FrameGroup> avatarFrames;

        private SLZ.VRMK.Avatar playerAvatar;
        private SLZ.VRMK.Avatar clonedAvatar;

        private Transform[] avatarBones;
        private Transform[] clonedRigBones;

        private FrameGroup previousFrame;
        private FrameGroup nextFrame;

        public override void Act()
        {
            previousFrame = new FrameGroup();
            nextFrame = new FrameGroup();

            foreach (var frame in avatarFrames)
            {
                previousFrame = nextFrame;
                nextFrame = frame;

                if (frame.frameTime > Playback.instance.PlaybackTime)
                {
                    break;
                }
            }

            float gap = nextFrame.frameTime - previousFrame.frameTime;
            float head = Playback.instance.PlaybackTime - previousFrame.frameTime;

            float delta = head / gap;

            List<ObjectFrame> previousTransformFrames = previousFrame.transformFrames;
            List<ObjectFrame> nextTransformFrames = nextFrame.transformFrames;

            for (int i = 0; i < 55; i++)
            {
                var bone = clonedRigBones[i];

                if (bone == null)
                {
                    continue;
                }

                if (previousTransformFrames == null)
                {
                    continue;
                }

                Vector3 previousBonePosition = previousTransformFrames[i].position;
                Vector3 nextBonePosition = nextTransformFrames[i].position;

                Quaternion previousBoneRotation = previousTransformFrames[i].rotation;
                Quaternion nextBoneRotation = nextTransformFrames[i].rotation;

                bone.position = Vector3.Lerp(previousBonePosition, nextBonePosition, delta);
                bone.rotation = Quaternion.Slerp(previousBoneRotation, nextBoneRotation, delta);
            }
        }

        /// <summary>
        /// Records the actor's bones, positons, and rotations for this frame.
        /// </summary>
        /// <param name="index">The frame to record the bones.</param>
        public override void RecordFrame()
        {
            FrameGroup frameGroup = new FrameGroup();
            frameGroup.SetFrames(CaptureBoneFrames(avatarBones), Recorder.instance.RecordingTime);
            avatarFrames.Add(frameGroup);
        }

        public void CaptureAvatarAction(int frame, Action action)
        {
            if (Director.PlayState == State.PlayState.Recording && !actionFrames.ContainsKey(frame))
            {
                actionFrames.Add(frame, action);
            }
        }

        public void CloneAvatar()
        {
            GameObject clonedAvatarObject = GameObject.Instantiate(playerAvatar.gameObject);
            clonedAvatar = clonedAvatarObject.GetComponent<SLZ.VRMK.Avatar>();

            clonedRigBones = GetAvatarBones(clonedAvatar);
            GameObject.Destroy(clonedAvatar.GetComponent<LODGroup>());

            actorName = $"Actor - {Constants.rigManager.AvatarCrate.Crate.Title}";
            clonedAvatar.name = actorName;
            ShowHairMeshes(clonedAvatar);

            GameObject.FindObjectOfType<PullCordDevice>().PlayAvatarParticleEffects();

            Events.OnActorCasted?.Invoke(this);
        }

        public override void Delete()
        {
            avatarFrames.Clear();
            GameObject.Destroy(clonedAvatar.gameObject);
            transform = null;
        }

        private void ShowHairMeshes(SLZ.VRMK.Avatar avatar)
        {
            foreach (var mesh in avatar.hairMeshes)
            {
                mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
            }
        }

        private List<ObjectFrame> CaptureBoneFrames(Transform[] boneList)
        {
            List<ObjectFrame> frames = new List<ObjectFrame>();

            for (int i = 0; i < boneList.Length; i++)
            {
                ObjectFrame frame = new ObjectFrame(boneList[i]);
                //frame.currentTime = Recorder.instance.RecordingTime;
                frames.Add(frame);
            }

            return frames;
        }

        private Transform[] GetAvatarBones(SLZ.VRMK.Avatar avatar)
        {
            Transform[] bones = new Transform[(int)HumanBodyBones.LastBone];

            for (int i = 0; i < bones.Length; i++)
            {
                var currentBone = (HumanBodyBones)i;

                if (avatar.animator.GetBoneTransform(currentBone) == null)
                {
                    continue;
                }
                else
                {
                    var boneTransform = avatar.animator.GetBoneTransform(currentBone);
                    bones[i] = boneTransform;
                }
            }

            return bones;
        }
    }
}
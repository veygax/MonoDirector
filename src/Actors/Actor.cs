using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using UnityEngine;

using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Warehouse;

using NEP.MonoDirector.Audio;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;

using Avatar = Il2CppSLZ.VRMK.Avatar;

namespace NEP.MonoDirector.Actors
{
    public class Actor : Trackable, IBinaryData
    {
        public Actor() : base()
        {
#if DEBUG
            _previousFrameDebugger = new Transform[55];
            _nextFrameDebugger = new Transform[55];

            GameObject baseCube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            baseCube.GetComponent<BoxCollider>().enabled = false;
            baseCube.transform.localScale = Vector3.one * 0.03F;

            GameObject empty = new GameObject("MONODIRECTOR DEBUG VIZ");
            baseCube.transform.parent = empty.transform;
            
            for (int i = 0; i < 55; i++)
            {
                _previousFrameDebugger[i] = GameObject.Instantiate(empty).transform;
                _nextFrameDebugger[i] = GameObject.Instantiate(empty).transform;
            }

            GameObject.Destroy(baseCube);
#endif
        }
        
        public Actor(Il2CppSLZ.VRMK.Avatar avatar) : this()
        {
            RigManager rigManager = BoneLib.Player.RigManager;
            _avatarBarcode = rigManager.AvatarCrate.Barcode;
            
            _playerAvatar = avatar;

            _avatarBones = GetAvatarBones(_playerAvatar);
            _avatarFrames = new List<FrameGroup>();

            GameObject micObject = new GameObject("Actor Microphone");
            _microphone = micObject.AddComponent<ActorSpeech>();

            _tempFrames = new ObjectFrame[_avatarBones.Length];
            OwnedProps = new List<Prop>();
        }

        // For a traditional rig, this should be all the "head" bones
        public readonly List<int> HeadBones = new List<int>()
        {
            (int)HumanBodyBones.Head,
            (int)HumanBodyBones.Jaw,
            (int)HumanBodyBones.LeftEye,
            (int)HumanBodyBones.RightEye,
        };


        private Barcode _avatarBarcode;
        public Barcode AvatarBarcode => _avatarBarcode;
        
        public Avatar PlayerAvatar => _playerAvatar;
        public Avatar ClonedAvatar => _clonedAvatar;
        public Transform[] AvatarBones => _avatarBones;

        public List<Prop> OwnedProps { get; private set; }
        public IReadOnlyList<FrameGroup> Frames => _avatarFrames.AsReadOnly();

        public ActorBody ActorBody => _body;
        public ActorSpeech Microphone => _microphone;
        public Texture2D AvatarPortrait => _avatarPortrait;

        public bool Seated => _activeSeat != null;

        protected List<FrameGroup> _avatarFrames;

        private ActorBody _body;
        private ActorSpeech _microphone;
        private Texture2D _avatarPortrait;

        private Il2CppSLZ.Marrow.Seat _activeSeat;

        private Avatar _playerAvatar;
        private Avatar _clonedAvatar;

        private ObjectFrame[] _tempFrames;

        private Transform[] _avatarBones;
        private Transform[] _clonedRigBones;

        private FrameGroup _previousFrame;
        private FrameGroup _nextFrame;

        private Transform _lastPelvisParent;
        private int _headIndex;
        
        // Debug build stuff
        #if DEBUG
        private Transform[] _previousFrameDebugger;
        private Transform[] _nextFrameDebugger;
        #endif

        public override void OnSceneBegin()
        {
            base.OnSceneBegin();

            for (int i = 0; i < 55; i++)
            {
                var bone = _clonedRigBones[i];

                if (bone == null)
                {
                    continue;
                }
                
                bone.position = _avatarFrames[0].TransformFrames[i].position;
                bone.rotation = _avatarFrames[0].TransformFrames[i].rotation;
            }
        }

        public override void Act()
        {
            _previousFrame = new FrameGroup();
            _nextFrame = new FrameGroup();

            for(int i = 0; i < _avatarFrames.Count; i++)
            {
                var frame = _avatarFrames[i];

                _previousFrame = _nextFrame;
                _nextFrame = frame;

                if (frame.FrameTime > Director.Instance.Playhead.PlaybackTime)
                {
                    break;
                }
            }

            float gap = _nextFrame.FrameTime - _previousFrame.FrameTime;
            float head = Director.Instance.Playhead.PlaybackTime - _previousFrame.FrameTime;

            float delta = head / gap;

            ObjectFrame[] previousTransformFrames = _previousFrame.TransformFrames;
            ObjectFrame[] nextTransformFrames = _nextFrame.TransformFrames;

            for (int i = 0; i < 55; i++)
            {
                if (i == (int)HumanBodyBones.Jaw)
                {
                    continue;
                }

                if (previousTransformFrames == null)
                {
                    continue;
                }

                Vector3 previousPosition = previousTransformFrames[i].position;
                Vector3 nextPosition = nextTransformFrames[i].position;

                Quaternion previousRotation = previousTransformFrames[i].rotation;
                Quaternion nextRotation = nextTransformFrames[i].rotation;

                var bone = _clonedRigBones[i];

                if(bone == null)
                {
                    continue;
                }

                bone.position = Vector3.Lerp(previousPosition, nextPosition, delta);
                bone.rotation = Quaternion.Slerp(previousRotation, nextRotation, delta);
#if DEBUG
                _previousFrameDebugger[i].position = previousPosition;
                _previousFrameDebugger[i].rotation = previousRotation;
                
                _nextFrameDebugger[i].position = nextPosition;
                _nextFrameDebugger[i].rotation = nextRotation;
#endif
            }
            
            for(int i = 0; i < actionFrames.Count; i++)
            {
                var actionFrame = actionFrames[i];

                if(Director.Instance.Playhead.PlaybackTime < actionFrame.timestamp)
                {
                    continue;
                }
                else
                {
                    actionFrame.Run();
                }
            }

            _microphone?.Playback();
            _microphone?.UpdateJaw();
        }

        /// <summary>
        /// Records the actor's bones, positons, and rotations for this frame.
        /// </summary>
        /// <param name="index">The frame to record the bones.</param>
        public override void RecordFrame()
        {
            FrameGroup frameGroup = new FrameGroup();
            CaptureBoneFrames(_avatarBones);
            frameGroup.SetFrames(_tempFrames, Director.Instance.Playhead.RecordingTime);
            _avatarFrames.Add(frameGroup);
        }

        public void CloneAvatar()
        {
            GameObject clonedAvatarObject = GameObject.Instantiate(_playerAvatar.gameObject);
            _clonedAvatar = clonedAvatarObject.GetComponent<Avatar>();

            _clonedAvatar.gameObject.SetActive(true);

            _body = new ActorBody(this, BoneLib.Player.PhysicsRig);

            // stops position overrides, if there are any
            _clonedAvatar.GetComponent<Animator>().enabled = false;

            _clonedRigBones = GetAvatarBones(_clonedAvatar);

            GameObject.Destroy(_clonedAvatar.GetComponent<LODGroup>());

            actorName = BoneLib.Player.RigManager.AvatarCrate.Crate.Title;
            _clonedAvatar.name = actorName;
            ShowHairMeshes(_clonedAvatar);

            _microphone.SetAvatar(_clonedAvatar);

            _clonedAvatar.gameObject.SetActive(true);

            // avatarPortrait = AvatarPhotoBuilder.avatarPortraits[actorName];

            Events.OnActorCasted?.Invoke(this);
        }

        public void SwitchToActor(Actor actor)
        {
            _clonedAvatar.gameObject.SetActive(false);
            actor._clonedAvatar.gameObject.SetActive(true);
        }

        public override void Delete()
        {
            Events.OnActorUncasted?.Invoke(this);
            _body.Delete();
            GameObject.Destroy(_clonedAvatar.gameObject);
            GameObject.Destroy(_microphone.gameObject);
            _microphone = null;
            _avatarFrames.Clear();
            DeleteOwnedProps();
        }

        public void DeleteProp(Prop prop)
        {
            Events.OnPropRemoved?.Invoke(prop);
            OwnedProps.Remove(prop);
            GameObject.Destroy(prop);
        }

        public void DeleteOwnedProps()
        {
            foreach (var prop in OwnedProps)
            {
                DeleteProp(prop);
            }
            
            OwnedProps.Clear();
        }

        public void ParentToSeat(Il2CppSLZ.Marrow.Seat seat)
        {
            _activeSeat = seat;

            Transform pelvis = _clonedAvatar.animator.GetBoneTransform(HumanBodyBones.Hips);

            _lastPelvisParent = pelvis.GetParent();

            Vector3 seatOffset = new Vector3(seat._buttOffset.x, Mathf.Abs(seat._buttOffset.y) * _clonedAvatar.heightPercent, seat._buttOffset.z);

            pelvis.SetParent(seat.transform);

            pelvis.position = seat.buttTargetInWorld;
            pelvis.localPosition = seatOffset;
        }

        public void UnparentSeat()
        {
            _activeSeat = null;
            Transform pelvis = _clonedAvatar.animator.GetBoneTransform(HumanBodyBones.Hips);
            pelvis.SetParent(_lastPelvisParent);
        }

        private void ShowHairMeshes(Avatar avatar)
        {
            if(avatar == null)
            {
                Main.Logger.Error("ShowHairMeshes: Avatar doesn't exist!");
            }

            if(avatar.hairMeshes.Count == 0 || avatar.hairMeshes == null)
            {
                Main.Logger.Error("ShowHairMeshes: No hair meshes to clone.");
            }

            foreach (var mesh in avatar.hairMeshes)
            {
                if(mesh == null)
                {
                    continue;
                }

                mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
            }
        }

        private void CaptureBoneFrames(Transform[] boneList)
        {
            for (int i = 0; i < boneList.Length; i++)
            {
                if (boneList[i] == null)
                {
                    _tempFrames[i] = new ObjectFrame(default, default);
                    continue;
                }

                Vector3 bonePosition = boneList[i].position;
                Quaternion boneRotation = boneList[i].rotation;

                ObjectFrame frame = new ObjectFrame(bonePosition, boneRotation);
                _tempFrames[i] = frame;
            }

            // Undo the head offset... afterward because no branching :P
            // This undoes it for every bone we count under it too!
            foreach (int headBone in HeadBones)
                _tempFrames[headBone].position += Patches.PlayerAvatarArtPatches.UpdateAvatarHead.calculatedHeadOffset;
        }

        private Transform[] GetAvatarBones(Avatar avatar)
        {
            Transform[] bones = new Transform[(int)HumanBodyBones.LastBone];

            for (int i = 0; i < (int)HumanBodyBones.LastBone - 1; i++)
            {
                var currentBone = (HumanBodyBones)i;

                var boneTransform = avatar.animator.GetBoneTransform(currentBone);
                bones[i] = boneTransform;
            }

            return bones;
        }
        
        //
        // Enums
        //
        public enum VersionNumber : short
        {
            V1
        }
        
        //
        // IBinaryData
        //
        public byte[] ToBinary()
        {
            // TODO: Keep this up to date

            List<byte> bytes = new List<byte>();
            
            // The header contains the following data
            //
            // version: u16
            // barcode_size: i32
            // barcode : utf-8 string
            // take_time: float
            // num_frames : u32
            //
            // Below the header is the following
            //
            // num_frames FrameGroup blocks
            //
            bytes.AddRange(BitConverter.GetBytes((short)VersionNumber.V1));
            
            byte[] encodedBarcode = Encoding.UTF8.GetBytes(_avatarBarcode.ID);
            bytes.AddRange(BitConverter.GetBytes(encodedBarcode.Length));
            bytes.AddRange(encodedBarcode);
            bytes.AddRange(BitConverter.GetBytes(Director.Instance.Playhead.TakeTime));
            bytes.AddRange(BitConverter.GetBytes(_avatarFrames.Count));

            foreach (FrameGroup group in _avatarFrames)
                bytes.AddRange(group.ToBinary());

            return bytes.ToArray();
        }

        public void FromBinary(Stream stream)
        {
            // Check the version number
            byte[] versionBytes = new byte[sizeof(short)];
            stream.Read(versionBytes, 0, versionBytes.Length);

            short version = BitConverter.ToInt16(versionBytes, 0);

            if (version != (short)VersionNumber.V1)
                throw new Exception($"Unsupported version type! Value was {version}");

            // Deserialize
            if (version == (short)VersionNumber.V1)
            {
                // How long is the string?
                byte[] strLenBytes = new byte[sizeof(int)];
                stream.Read(strLenBytes, 0, strLenBytes.Length);

                int strLen = BitConverter.ToInt32(strLenBytes, 0);

                byte[] strBytes = new byte[strLen];
                stream.Read(strBytes, 0, strBytes.Length);

                // avatarBarcode = Encoding.UTF8.GetString(strBytes);
                
#if DEBUG
                Main.Logger.Msg($"[ACTOR]: Barcode: {_avatarBarcode}");
#endif
                
                // Then the take
                byte[] takeBytes = new byte[sizeof(float)];
                stream.Read(takeBytes, 0, takeBytes.Length);

                float takeTime = BitConverter.ToSingle(takeBytes, 0);

                // Force the take time to be correct
                // This means if an actor takes a long time on disk
                // We then match their take time and not ours
                //if (Director.Instance.Playhead.TakeTime < takeTime)
                    //Director.Instance.Playhead.TakeTime = takeTime;
                
                // Then deserialize the frames
                byte[] frameNumBytes = new byte[sizeof(int)];
                stream.Read(frameNumBytes, 0, frameNumBytes.Length);

                int numFrames = BitConverter.ToInt32(frameNumBytes, 0);

#if DEBUG
                Main.Logger.Msg($"[ACTOR]: NumFrames: {numFrames}");
#endif
                
                FrameGroup[] frameGroups = new FrameGroup[numFrames];

                for (int f = 0; f < numFrames; f++)
                {
                    frameGroups[f] = new FrameGroup();
                    frameGroups[f].FromBinary(stream);
                }

                _avatarFrames = new List<FrameGroup>(frameGroups);
            }
        }

        // TADB - Tracked Actor Data Block
        public uint GetBinaryID() => 0x42444154;
    }
}

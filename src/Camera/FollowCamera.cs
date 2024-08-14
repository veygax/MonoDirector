using System.Collections.Generic;
using UnityEngine;
using BoneLib;
using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Data;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class FollowCamera : MonoBehaviour
    {
        public readonly Dictionary<BodyPart, BodyPartData> FollowPoints;

        public FollowCamera(System.IntPtr ptr) : base(ptr)
        {
#if DEBUG
            // Log the values of all critical components
            Main.Logger.Msg($"CameraRigManager.Instance: {(CameraRigManager.Instance != null ? CameraRigManager.Instance.ToString() : "null")}");
            Main.Logger.Msg($"CameraRigManager.Instance.RigScreenOptions: {(CameraRigManager.Instance != null && CameraRigManager.Instance.RigScreenOptions != null ? CameraRigManager.Instance.RigScreenOptions.ToString() : "null")}");
            Main.Logger.Msg($"Player.PhysicsRig: {(Player.PhysicsRig != null ? Player.PhysicsRig.ToString() : "null")}");
            Main.Logger.Msg($"Player.PhysicsRig.m_chest: {(Player.PhysicsRig != null && Player.PhysicsRig.m_chest != null ? Player.PhysicsRig.m_chest.ToString() : "null")}");
            Main.Logger.Msg($"Player.PhysicsRig.m_pelvis: {(Player.PhysicsRig != null && Player.PhysicsRig.m_pelvis != null ? Player.PhysicsRig.m_pelvis.ToString() : "null")}");
#endif

            if (CameraRigManager.Instance == null ||
                CameraRigManager.Instance.RigScreenOptions == null ||
                Player.PhysicsRig == null ||
                Player.PhysicsRig.m_chest == null ||
                Player.PhysicsRig.m_pelvis == null)
            {
#if DEBUG
                Main.Logger.Error("Critical components are not initialized!");
#endif
                return;
            }

            FollowPoints = new Dictionary<BodyPart, BodyPartData>()
            {
                { BodyPart.Head, new BodyPartData(CameraRigManager.Instance.RigScreenOptions.TargetTransform) },
                { BodyPart.Chest, new BodyPartData(Player.PhysicsRig.m_chest) },
                { BodyPart.Pelvis, new BodyPartData(Player.PhysicsRig.m_pelvis) }
            };

#if DEBUG
            Main.Logger.Msg("FollowCamera constructor completed successfully.");
#endif
        }

        public Transform FollowTarget { get => _followTarget; }

        public float delta = 4f;

        private Vector3 _positionOffset;
        private Vector3 _rotationEulerOffset;

        private Transform _followTarget;

        private void Update()
        {
#if DEBUG
            Main.Logger.Msg("Update method called.");
#endif

            if (_followTarget == null)
            {
#if DEBUG
                Main.Logger.Msg("Follow target is null");
#endif
                return;
            }

#if DEBUG
            Main.Logger.Msg($"Follow target position: {_followTarget.position}, rotation: {_followTarget.rotation}");
#endif

            transform.position = _followTarget.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, _followTarget.rotation, delta * Time.deltaTime);

#if DEBUG
            Main.Logger.Msg("Update method completed.");
#endif
        }

        public void SetDefaultTarget()
        {
#if DEBUG
            Main.Logger.Msg("SetDefaultTarget method called.");
#endif

            SetFollowTarget(FollowPoints[BodyPart.Head].transform);

#if DEBUG
            Main.Logger.Msg("SetDefaultTarget method completed.");
#endif
        }

        public void SetFollowTarget(Transform target)
        {
#if DEBUG
            Main.Logger.Msg($"SetFollowTarget method called with target: {target}");
#endif

            _followTarget = target;

#if DEBUG
            Main.Logger.Msg($"Follow target set to: {_followTarget}");
#endif
        }

        public void SetPositionOffset(Vector3 offset)
        {
#if DEBUG
            Main.Logger.Msg($"SetPositionOffset method called with offset: {offset}");
#endif

            _positionOffset = offset;

#if DEBUG
            Main.Logger.Msg($"Position offset set to: {_positionOffset}");
#endif
        }

        public void SetRotationOffset(Vector3 offset)
        {
#if DEBUG
            Main.Logger.Msg($"SetRotationOffset(Vector3) method called with offset: {offset}");
#endif

            _rotationEulerOffset = offset;

#if DEBUG
            Main.Logger.Msg($"Rotation offset set to: {_rotationEulerOffset}");
#endif
        }

        public void SetRotationOffset(Quaternion offset)
        {
#if DEBUG
            Main.Logger.Msg($"SetRotationOffset(Quaternion) method called with offset: {offset}");
#endif

            _rotationEulerOffset = offset.eulerAngles;

#if DEBUG
            Main.Logger.Msg($"Rotation offset set to: {_rotationEulerOffset}");
#endif
        }

        public void SetFollowBone(BodyPart part)
        {
#if DEBUG
            Main.Logger.Msg($"SetFollowBone method called with BodyPart: {part}");
#endif

            _positionOffset = Vector3.zero;
            _rotationEulerOffset = Vector3.zero;

            Vector3 point = FollowPoints[part].position;

#if DEBUG
            Main.Logger.Msg($"Setting follow target position to: {point}");
#endif

            if (_followTarget != null)
            {
                _followTarget.position = point;
                _followTarget.localPosition = _positionOffset;
                _followTarget.localRotation = Quaternion.Euler(_rotationEulerOffset);

#if DEBUG
                Main.Logger.Msg("Follow target position and rotation set.");
#endif
            }
            else
            {
#if DEBUG
                Main.Logger.Msg("Error: _followTarget is null in SetFollowBone.");
#endif
            }
        }
    }
}

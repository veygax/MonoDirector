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
        public FollowCamera(System.IntPtr ptr) : base(ptr) { }

        public readonly Dictionary<BodyPart, BodyPartData> FollowPoints = new Dictionary<BodyPart, BodyPartData>()
        {
            { BodyPart.Head, new BodyPartData(CameraRigManager.Instance.RigScreenOptions.TargetTransform) },
            { BodyPart.Chest, new BodyPartData(Player.PhysicsRig.m_chest) },
            { BodyPart.Pelvis, new BodyPartData(Player.PhysicsRig.m_pelvis) }
        };

        public Transform FollowTarget { get => _followTarget; }

        public float delta = 4f;

        private Vector3 _positionOffset;
        private Vector3 _rotationEulerOffset;

        private Transform _followTarget;

        private void Update()
        {
            if (_followTarget == null)
            {
                Main.Logger.Msg("Follow target is null");
                return;
            }

            transform.position = _followTarget.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, _followTarget.rotation, delta * Time.deltaTime);
        }

        public void SetDefaultTarget()
        {
            SetFollowTarget(FollowPoints[BodyPart.Head].transform);
        }

        public void SetFollowTarget(Transform target)
        {
            _followTarget = target;
        }

        public void SetPositionOffset(Vector3 offset)
        {
            _positionOffset = offset;
        }

        public void SetRotationOffset(Vector3 offset)
        {
            _rotationEulerOffset = offset;
        }

        public void SetRotationOffset(Quaternion offset)
        {
            _rotationEulerOffset = offset.eulerAngles;
        }

        public void SetFollowBone(BodyPart part)
        {
            _positionOffset = Vector3.zero;
            _rotationEulerOffset = Vector3.zero;

            Vector3 point = FollowPoints[part].position;

            _followTarget.position = point;

            _followTarget.localPosition = _positionOffset;
            _followTarget.localRotation = Quaternion.Euler(_rotationEulerOffset);
        }
    }
}

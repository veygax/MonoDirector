using Il2CppInterop.Runtime.InteropTypes.Fields;

using UnityEngine;

using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class HandheldCamera : MonoBehaviour
    {
        public HandheldCamera(System.IntPtr ptr) : base(ptr) { }

        public Il2CppReferenceField<CylinderGrip> leftHandle;
        public Il2CppReferenceField<CylinderGrip> rightHandle;

        public Il2CppReferenceField<Transform> leftHandleTransform;
        public Il2CppReferenceField<Transform> rightHandleTransform;

        public Il2CppReferenceField<Camera> sensorCamera;

        public Il2CppReferenceField<GameObject> backViewfinderScreen;
        public Il2CppReferenceField<GameObject> frontViewfinderScreen;
        public Il2CppReferenceField<GameObject> displayScreen;

        public Il2CppReferenceField<Rigidbody> cameraRigidbody;

        private RenderTexture displayTexture  => sensorCamera.Get().targetTexture;

        private void OnEnable()
        {
            Events.OnCameraModeSet += OnCameraModeChanged;

            leftHandle.Get().attachedUpdateDelegate += new System.Action<Hand>(LeftHandUpdate);
            rightHandle.Get().attachedUpdateDelegate += new System.Action<Hand>(RightHandUpdate);
            leftHandle.Get().detachedHandDelegate += new System.Action<Hand>(LeftHandDetached);
            leftHandle.Get().detachedHandDelegate += new System.Action<Hand>(RightHandDetached);
        }

        private void OnDisable()
        {
            Events.OnCameraModeSet -= OnCameraModeChanged;

            leftHandle.Get().attachedUpdateDelegate -= new System.Action<Hand>(LeftHandUpdate);
            rightHandle.Get().attachedUpdateDelegate -= new System.Action<Hand>(RightHandUpdate);
            leftHandle.Get().detachedHandDelegate -= new System.Action<Hand>(LeftHandDetached);
            leftHandle.Get().detachedHandDelegate -= new System.Action<Hand>(RightHandDetached);
        }

        private void OnCameraModeChanged(CameraMode mode)
        {
            if(mode == CameraMode.Handheld)
            {
                displayScreen.Get().active = true;
                backViewfinderScreen.Get().active = true;
                frontViewfinderScreen.Get().active = true;

                CameraRigManager.Instance.ClonedCamera.targetTexture = displayTexture;
                CameraRigManager.Instance.ClonedCamera.gameObject.SetActive(true);
                CameraRigManager.Instance.FollowCamera.SetFollowTarget(sensorCamera.Get().transform);
                CameraRigManager.Instance.CameraDisplay.FollowCamera.SetFollowTarget(sensorCamera.Get().transform);
            }
            else
            {
                displayScreen.Get().active = false;
                backViewfinderScreen.Get().active = false;
                frontViewfinderScreen.Get().active = false;

                CameraRigManager.Instance.ClonedCamera.gameObject.SetActive(false);
                CameraRigManager.Instance.FollowCamera.SetDefaultTarget();
            }
        }

        private void LeftHandUpdate(Hand hand)
        {
            cameraRigidbody.Get().isKinematic = false;

            if (hand.GetIndexTriggerAxis() > 0.25f)
            {
                float rate = CameraRigManager.Instance.FOVController.fovChangeRate;

                CameraRigManager.Instance.CameraDisplay.FOVController.SetFOV(-(hand.GetIndexTriggerAxis() * rate / 10f));
                CameraRigManager.Instance.FOVController.SetFOV(-(hand.GetIndexTriggerAxis() * rate / 10f));
            }
        }
         
        private void RightHandUpdate(Hand hand)
        {
            cameraRigidbody.Get().isKinematic = false;

            if (hand.GetIndexTriggerAxis() > 0.25f)
            {
                float rate = CameraRigManager.Instance.FOVController.fovChangeRate;

                CameraRigManager.Instance.CameraDisplay.FOVController.SetFOV(hand.GetIndexTriggerAxis() * rate / 10f);
                CameraRigManager.Instance.FOVController.SetFOV(hand.GetIndexTriggerAxis() * rate / 10f);
            }
        }

        private void LeftHandDetached(Hand hand)
        {
            if (Settings.Camera.KinematicOnRelease)
            {
                cameraRigidbody.Get().isKinematic = true;
            }
        }

        private void RightHandDetached(Hand hand)
        {
            if (Settings.Camera.KinematicOnRelease)
            {
                cameraRigidbody.Get().isKinematic = true;
            }
        }
    }
}

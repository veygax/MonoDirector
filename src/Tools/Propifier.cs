using Il2CppInterop.Runtime.InteropTypes.Fields;

using UnityEngine;

using Il2CppSLZ.Marrow;

using NEP.MonoDirector.Actors;

namespace NEP.MonoDirector.Tools
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class Propifier : MonoBehaviour
    {
        public Propifier(System.IntPtr ptr) : base(ptr) { }

        public enum Mode
        {
            Prop,
            Remove
        }

        public Mode mode;

        public Il2CppReferenceField<TargetGrip> triggerGrip;
        public Il2CppReferenceField<Transform> firePoint;
        public Il2CppValueField<float> maxRange;
        public Il2CppReferenceField<GameObject> laserPointer;
        public Il2CppReferenceField<Rigidbody> rigidbody;

        public Il2CppReferenceField<GameObject> propModeIcon;
        public Il2CppReferenceField<GameObject> removeModeIcon;

        public float fireForce = 5f;

        private GunSFX gunSFX;

        private void Awake()
        {
            maxRange.Set(30f);
        }

        private void OnEnable()
        {
            triggerGrip.Get().attachedHandDelegate += new System.Action<Hand>((hand) => OnAttachHand());
            triggerGrip.Get().detachedHandDelegate += new System.Action<Hand>((hand) => OnDetachHand());
            triggerGrip.Get().attachedUpdateDelegate += new System.Action<Hand>((hand) => OnTriggerGripUpdate());
        }

        private void OnDisable()
        {
            triggerGrip.Get().attachedHandDelegate -= new System.Action<Hand>((hand) => OnAttachHand());
            triggerGrip.Get().detachedHandDelegate -= new System.Action<Hand>((hand) => OnDetachHand());
            triggerGrip.Get().attachedUpdateDelegate -= new System.Action<Hand>((hand) => OnTriggerGripUpdate());
        }

        private void PrimaryButtonDown()
        {
            rigidbody.Get().AddForce(rigidbody.Get().transform.up - firePoint.Get().forward * fireForce, ForceMode.Impulse);

            if(Physics.Raycast(firePoint.Get().position, firePoint.Get().forward * maxRange, out RaycastHit hit))
            {
                if(hit.rigidbody == null)
                {
                    return;
                }

                InteractableHost entity = hit.rigidbody.GetComponent<InteractableHost>();

                if(entity == null)
                {
                    return;
                }

                if(mode == Mode.Prop)
                {
                    PropBuilder.BuildProp(entity);
                }
                else
                {
                    PropBuilder.RemoveProp(entity);
                }
            }
        }

        public void OnAttachHand()
        {
            // laserPointer.Get().SetActive(true);
        }

        public void OnDetachHand()
        {
            // laserPointer.Get().SetActive(false);
        }

        public void OnTriggerGripUpdate()
        {
            Hand hand = triggerGrip.Get().GetHand();
            bool bTapped = hand.Controller.GetMenuTap();

            if (bTapped)
            {
                if(mode == Mode.Prop)
                {
                    SetMode(Mode.Remove);
                }
                else
                {
                    SetMode(Mode.Prop);
                }
            }

            if (hand._indexButtonDown)
            {
                PrimaryButtonDown();
            }
        }

        public void SetMode(Mode mode)
        {
            this.mode = mode;
            // gunSFX.DryFire();

            if (mode == Mode.Prop)
            {
                propModeIcon.Get().SetActive(true);
                removeModeIcon.Get().SetActive(false);
            }
            else if (mode == Mode.Remove)
            {
                propModeIcon.Get().SetActive(false);
                removeModeIcon.Get().SetActive(true);
            }
        }
    }
}
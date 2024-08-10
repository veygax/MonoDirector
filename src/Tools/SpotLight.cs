using System.Collections.Generic;

using UnityEngine;

using Il2CppSLZ.Marrow;

using NEP.MonoDirector.State;

namespace NEP.MonoDirector.Tools
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class SpotLight : MonoBehaviour
    {
        public SpotLight(System.IntPtr ptr) : base(ptr) { }

        public static List<SpotLight> ComponentCache { get; private set; }

        public float Range { get; private set; }
        public float Intensity { get; private set; }

        private Rigidbody rb;

        private GameObject sprite;

        private Grip lightGrip;

        private GameObject arrow;

        private void Awake()
        {
            ComponentCache = new List<SpotLight>();
            rb = GetComponent<Rigidbody>();
            sprite = transform.Find("Sprite").gameObject;
            lightGrip = transform.Find("Grip").GetComponent<Grip>();

            arrow = transform.Find("arrow").gameObject;
        
            lightGrip.attachedHandDelegate += new System.Action<Hand>((hand) => AttachedHand(hand));
            lightGrip.detachedHandDelegate += new System.Action<Hand>((hand) => DetachedHand(hand));
        }

        private void OnEnable()
        {
            Events.OnPlayStateSet += OnPlayStateSet;
            ComponentCache.Add(this);
        }

        private void OnDisable()
        {
            Events.OnPlayStateSet -= OnPlayStateSet;
            ComponentCache.Remove(this);
        }

        private void AttachedHand(Hand hand)
        {
            rb.isKinematic = false;
        }

        private void DetachedHand(Hand hand)
        {
            rb.isKinematic = true;
        }

        private void ShowVisuals()
        {
            sprite.SetActive(true);
            arrow.SetActive(true);
        }

        private void HideVisuals()
        {
            sprite.SetActive(false);
            arrow.SetActive(false);
        }

        private void OnPlayStateSet(PlayheadState playState)
        {
            if (playState == PlayheadState.Preplaying 
            || playState == PlayheadState.Playing 
            || playState == PlayheadState.Stopped)
            {
                HideVisuals();
            }
            else
            {
                ShowVisuals();
            }
        }
    }
}
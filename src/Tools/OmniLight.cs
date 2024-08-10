using System.Collections.Generic;
using NEP.MonoDirector.State;
using Il2CppSLZ.Marrow;
using UnityEngine;

namespace NEP.MonoDirector.Tools
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class OmniLight : MonoBehaviour
    {
        public OmniLight(System.IntPtr ptr) : base(ptr) { }

        public static List<OmniLight> ComponentCache { get; private set; }

        public float Range { get; private set; }
        public float Intensity { get; private set; }

        private Rigidbody rb;

        private GameObject sprite;

        private Grip lightGrip;

        private void Awake()
        {
            ComponentCache = new List<OmniLight>();
            rb = GetComponent<Rigidbody>();
            sprite = transform.Find("Sprite").gameObject;
            lightGrip = transform.Find("Grip").GetComponent<Grip>();
        
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
        }

        private void HideVisuals()
        {
            sprite.SetActive(false);
        }

        private void OnPlayStateSet(PlayheadState playState)
        {
            // TODO:
            // Adjust for new state machine system
            //if (playState == PlayheadState.Preplaying 
            //|| playState == PlayheadState.Playing 
            //|| playState == PlayheadState.Stopped)
            //{
            //    HideVisuals();
            //}
            //else
            //{
            //    ShowVisuals();
            //}
        }
    }
}
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Data;

using NEP.MonoDirector.Core;
using NEP.MonoDirector.Actors;

namespace NEP.MonoDirector.Patches.Guns
{
    public static class Magazine
    {
        [HarmonyLib.HarmonyPatch(typeof(Il2CppSLZ.Marrow.Magazine), nameof(Il2CppSLZ.Marrow.Magazine.OnGrab))]
        public static class OnGrab
        {
            public static void Postfix(Hand hand)
            {
                if (Director.PlayState == State.PlayState.Recording)
                {
                    HandReciever reciever = hand.AttachedReceiver;
                    var poolee = reciever.Host.Rb.GetComponent<InteractableHost>();
                    PropBuilder.BuildProp(poolee);
                }
            }
        }
    }
}
using UnityEngine;

using NEP.MonoDirector.Audio;

namespace NEP.MonoDirector.Patches
{
    internal class HandSFX
    {
        [HarmonyLib.HarmonyPatch(typeof(Il2CppSLZ.Marrow.HandSFX), nameof(Il2CppSLZ.Marrow.HandSFX.Grab))]
        internal static class Grab
        {
            internal static void Postfix(Il2CppSLZ.Marrow.HandSFX __instance)
            {
                /* var activeActor = Recorder.instance.ActiveActor;

                if (activeActor == null)
                {
                    return;
                }

                if(Director.PlayState == State.PlayState.Recording)
                {
                    activeActor.RecordAction(() => PlaySFX(__instance, __instance.transform.position));
                } */
            }

            internal static void PlaySFX(Il2CppSLZ.Marrow.HandSFX hand, Vector3 position)
            {
                int rand = Random.Range(0, hand.grab.Count);

                AudioManager.Instance.PlayAtPosition(hand.grab[rand], position);
            }
        }
    }
}

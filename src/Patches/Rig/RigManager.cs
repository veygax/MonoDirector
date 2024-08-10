using UnityEngine;
using Il2CppSLZ.Marrow;
using NEP.MonoDirector.Core;

using Avatar = Il2CppSLZ.VRMK.Avatar;

namespace NEP.MonoDirector.Patches
{
    internal static class RigManager
    {
        [HarmonyLib.HarmonyPatch(typeof(Il2CppSLZ.Marrow.RigManager), nameof(Il2CppSLZ.Marrow.RigManager.SwitchAvatar))]
        internal static class SwitchAvatar
        {
            internal static void Postfix(Avatar newAvatar)
            {
                if(Director.PlayState != State.PlayheadState.Recording)
                {
                    return;
                }

                var activeActor = Recorder.instance.ActiveActor;
                activeActor.RecordAction(new System.Action(() => activeActor.SwitchToActor(activeActor)));
                activeActor.CloneAvatar();
                Recorder.instance.ActiveActors.Add(activeActor);
                Recorder.instance.SetActor(newAvatar);
            }
        }
    }
}

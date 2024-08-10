using Il2CppSLZ.Bonelab;
using NEP.MonoDirector.Cameras;

namespace NEP.MonoDirector.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(RigScreenOptions), nameof(RigScreenOptions.Start))]
    public static class RigScreenOptionPatch
    {
        public static void Postfix(RigScreenOptions __instance)
        {
            new CameraRigManager(__instance);
        }
    }
}

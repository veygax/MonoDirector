using NEP.MonoDirector.Actors;

namespace NEP.MonoDirector.Patches
{
    internal class ObjectDestructable
    {
        [HarmonyLib.HarmonyPatch(typeof(Il2CppSLZ.Marrow.ObjectDestructible), nameof(Il2CppSLZ.Marrow.ObjectDestructible.Awake))]
        internal static class TakeDamage
        {
            static void Postfix(Il2CppSLZ.Marrow.ObjectDestructible __instance)
            {
                __instance.OnDestruction += new System.Action<Il2CppSLZ.Marrow.ObjectDestructible>(OnObjectDestroyed);
            }

            static void OnObjectDestroyed(Il2CppSLZ.Marrow.ObjectDestructible destructable)
            {
                var prop = destructable.GetComponent<BreakableProp>();
                prop?.RecordAction(prop.DestructionEvent);
            }
        }
    }
}

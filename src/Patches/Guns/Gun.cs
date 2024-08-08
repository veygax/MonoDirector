using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Core;

using Il2CppSLZ.Marrow.Data;

namespace NEP.MonoDirector.Patches
{
    internal static class Gun
    {
        [HarmonyLib.HarmonyPatch(typeof(Il2CppSLZ.Marrow.Gun), nameof(OnFire))]
        internal static class OnFire
        {
            internal static void Postfix(Il2CppSLZ.Marrow.Gun __instance)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp?.RecordAction(new System.Action(() => gunProp.GunFakeFire()));
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Il2CppSLZ.Marrow.Gun), nameof(Il2CppSLZ.Marrow.Gun.SetAnimationState))]
        internal static class PlayAnimationState
        {
            internal static void Postfix(Il2CppSLZ.Marrow.Gun __instance, Il2CppSLZ.Marrow.Gun.AnimationStates state, float perc)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp?.RecordAction(() => __instance.SetAnimationState(state, perc));
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Il2CppSLZ.Marrow.Gun), nameof(Il2CppSLZ.Marrow.Gun.OnMagazineInserted))]
        internal static class OnMagazineInserted
        {
            internal static void Postfix(Il2CppSLZ.Marrow.Gun __instance)
            {
                if (__instance._magState != null)
                {
                    var gunProp = __instance.gameObject.GetComponent<GunProp>();
                    int count = __instance._magState.AmmoCount;
                    CartridgeData cartridgeData = __instance._magState.cartridgeData;
                    MagazineData magazineData = __instance._magState.magazineData;
                    gunProp?.RecordAction(new System.Action(() => gunProp.InsertMagState(cartridgeData, magazineData, count)));
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Il2CppSLZ.Marrow.Gun), nameof(Il2CppSLZ.Marrow.Gun.OnMagazineRemoved))]
        internal static class OnMagazineRemoved
        {
            internal static void Postfix(Il2CppSLZ.Marrow.Gun __instance)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp?.RecordAction(new System.Action(() => gunProp.RemoveMagState()));
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Il2CppSLZ.Marrow.Gun), nameof(Il2CppSLZ.Marrow.Gun.UpdateArt))]
        internal static class UpdateArt
        {
            internal static void Postfix(Il2CppSLZ.Marrow.Gun __instance)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp?.RecordAction(new System.Action(() => __instance.UpdateArt()));
            }
        }
    }
}

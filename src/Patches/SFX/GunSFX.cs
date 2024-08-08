using NEP.MonoDirector.Actors;

namespace NEP.MonoDirector.Patches
{
    internal static class GunSFX
    {
        [HarmonyLib.HarmonyPatch(typeof(Il2CppSLZ.Marrow.GunSFX), nameof(Il2CppSLZ.Marrow.GunSFX.MagazineInsert))]
        internal static class MagazineInsert
        {
            internal static void Postfix(Il2CppSLZ.Marrow.GunSFX __instance)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp?.RecordAction(gunProp.Gun.gunSFX.MagazineInsert);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Il2CppSLZ.Marrow.GunSFX), nameof(Il2CppSLZ.Marrow.GunSFX.MagazineDrop))]
        internal static class MagazineDrop
        {
            internal static void Postfix(Il2CppSLZ.Marrow.GunSFX __instance)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp?.RecordAction(gunProp.Gun.gunSFX.MagazineDrop);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Il2CppSLZ.Marrow.GunSFX), nameof(Il2CppSLZ.Marrow.GunSFX.SlidePull))]
        internal static class SlidePull
        {
            internal static void Postfix(Il2CppSLZ.Marrow.GunSFX __instance)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp?.RecordAction(gunProp.Gun.gunSFX.SlidePull);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Il2CppSLZ.Marrow.GunSFX), nameof(Il2CppSLZ.Marrow.GunSFX.SlideRelease))]
        internal static class SlideRelease
        {
            internal static void Postfix(Il2CppSLZ.Marrow.GunSFX __instance)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp?.RecordAction(gunProp.Gun.gunSFX.SlideRelease);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Il2CppSLZ.Marrow.GunSFX), nameof(Il2CppSLZ.Marrow.GunSFX.SlideLock))]
        internal static class SlideLock
        {
            internal static void Postfix(Il2CppSLZ.Marrow.GunSFX __instance)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp?.RecordAction(gunProp.Gun.gunSFX.SlideLock);
            }
        }
    }
}

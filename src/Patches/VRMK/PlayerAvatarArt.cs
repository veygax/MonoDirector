using UnityEngine;

namespace NEP.MonoDirector.Patches
{
    internal static class PlayerAvatarArtPatches
    {
        [HarmonyLib.HarmonyPatch(typeof(Il2CppSLZ.Marrow.PlayerAvatarArt), nameof(Il2CppSLZ.Marrow.PlayerAvatarArt.UpdateAvatarHead))]
        internal static class UpdateAvatarHead
        {
            internal static Vector3 preTransformHead;
            internal static Vector3 postTransformHead;
            
            internal static Vector3 calculatedHeadOffset;
            
            internal static void Prefix(Il2CppSLZ.Marrow.PlayerAvatarArt __instance)
            {
                Il2CppSLZ.Marrow.RigManager rigManager = BoneLib.Player.RigManager;
                Transform head = rigManager.avatar.animator.GetBoneTransform(HumanBodyBones.Head);
                preTransformHead = head.position;
            }

            internal static void Postfix(Il2CppSLZ.Marrow.PlayerAvatarArt __instance)
            {
                Il2CppSLZ.Marrow.RigManager rigManager = BoneLib.Player.RigManager;
                Transform head = rigManager.avatar.animator.GetBoneTransform(HumanBodyBones.Head);
                postTransformHead = head.position;

                calculatedHeadOffset = preTransformHead - postTransformHead;
            }
        }
    }
}
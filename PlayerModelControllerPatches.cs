using HarmonyLib;
using UnityEngine;

namespace ColorCustomizer
{
    [HarmonyPatch(typeof(PlayerModelController))]
    public static class PlayerModelControllerPatches
    {
        public static Material playerSuitMaterial = null;
        public static Material playerDepthSuitMaterial = null;

        [HarmonyPatch(nameof(PlayerModelController.Awake))]
        [HarmonyPostfix]
        public static void Awake_Postfix(PlayerModelController __instance)
        {
            playerSuitMaterial = __instance.playerMeshRenderer.materials[__instance.suitBaseMatIndex];
            playerDepthSuitMaterial = __instance.depthModuleSuitMat;
        }

        [HarmonyPatch(nameof(PlayerModelController.PointHeadAndBodyAtPoint))]
        [HarmonyPrefix]
        public static bool PointHeadAndBodyAtPoint_Prefix()
        {
            // Don't look at loddles if the menu is open
            return !(CustomizerMod.customizerUI != null && CustomizerMod.customizerUI.playerMenuIsOpen);
        }

        [HarmonyPatch(nameof(PlayerModelController.OnDestroy))]
        [HarmonyPostfix]
        public static void OnDestroy_Postfix(PlayerModelController __instance)
        {
            playerSuitMaterial = null;
            playerDepthSuitMaterial = null;
        }
    }
}

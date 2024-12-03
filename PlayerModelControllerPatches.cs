using HarmonyLib;
using UnityEngine;

namespace ColorCustomizer
{
    [HarmonyPatch(typeof(PlayerModelController))]
    public static class PlayerModelControllerPatches
    {
        const int suitBaseIndex = 0;
        const int antennaInnerIndex = 1;
        const int helmetLightIndex = 2;
        const int helmetWindowIndex = 3;
        const int antennaOuterIndex = 4;

        [HarmonyPatch(nameof(PlayerModelController.Awake))]
        [HarmonyPostfix]
        public static void Awake_Postfix(PlayerModelController __instance)
        {
            CustomizerMod.playerSuitMaterial = __instance.playerMeshRenderer.materials[suitBaseIndex];
            CustomizerMod.playerDepthSuitMaterial = __instance.depthModuleSuitMat;
            CustomizerMod.playerAntennaInnerMaterial = __instance.playerMeshRenderer.materials[antennaInnerIndex];
            CustomizerMod.playerHelmetLightsMaterial = __instance.playerMeshRenderer.materials[helmetLightIndex];
            CustomizerMod.playerHelmetWindowMaterial = __instance.playerMeshRenderer.materials[helmetWindowIndex];
            CustomizerMod.playerAntennaOuterMaterial = __instance.playerMeshRenderer.materials[antennaOuterIndex];
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
            CustomizerMod.playerSuitMaterial = null;
            CustomizerMod.playerDepthSuitMaterial = null;
            CustomizerMod.playerAntennaInnerMaterial = null;
            CustomizerMod.playerHelmetLightsMaterial = null;
            CustomizerMod.playerHelmetWindowMaterial = null;
            CustomizerMod.playerAntennaOuterMaterial = null;
        }
    }
}

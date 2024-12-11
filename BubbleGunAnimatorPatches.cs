using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace ColorCustomizer
{
    [HarmonyPatch(typeof(BubbleGunAnimator))]
    public static class BubbleGunAnimatorPatches
    {
        const int gunMainMaterialIndex = 0;
        const int gunTipMaterialIndex = 0;
        const int gunGlassMaterialIndex = 1;
        const int gunCoreMaterialIndex = 2;
        const int gunBlasterBallMaterialIndex = 0;
        const int gunBlasterRodMaterialIndex = 1;
        const int gunBlasterDiskMaterialIndex1 = 0;
        const int gunBlasterDiskMaterialIndex2 = 0;

        [HarmonyPatch(nameof(BubbleGunAnimator.Awake))]
        [HarmonyPostfix]
        public static void Awake_Postfix(BubbleGunAnimator __instance)
        {
            CustomizerMod.gunMainMaterial = __instance.bubbleGunMeshRenderer.materials[gunMainMaterialIndex];
            CustomizerMod.gunTipMaterial = __instance.tipRenderer.materials[gunTipMaterialIndex];
            CustomizerMod.gunGlassMaterial = __instance.bubbleGunMeshRenderer.materials[gunGlassMaterialIndex];
            CustomizerMod.gunCoreMaterial = __instance.bubbleGunMeshRenderer.materials[gunCoreMaterialIndex];
            CustomizerMod.gunBlasterBallMaterial = __instance.blasterRodMeshRenderer.materials[gunBlasterBallMaterialIndex];
            CustomizerMod.gunBlasterRodMaterial = __instance.blasterRodMeshRenderer.materials[gunBlasterRodMaterialIndex];
            CustomizerMod.gunBlasterDiskMaterial1 = __instance.bigDiscRenderer.materials[gunBlasterDiskMaterialIndex1];
            CustomizerMod.gunBlasterDiskMaterial2 = __instance.smallDiscRenderer.materials[gunBlasterDiskMaterialIndex2];
            CustomizerMod.gunBarMaterial = __instance.blasterBarsMeshRenderer.materials[__instance.blasterBarsMaterialIndex];
        }
    }
}

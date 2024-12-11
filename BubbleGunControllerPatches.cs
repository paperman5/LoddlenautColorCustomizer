using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorCustomizer
{
    [HarmonyPatch(typeof(BubbleGunController))]
    public static class BubbleGunControllerPatches
    {
        [HarmonyPatch(nameof(BubbleGunController.Awake))]
        [HarmonyPostfix]
        public static void Awake_Postfix(BubbleGunController __instance)
        {
            CustomizerMod.gunLaserMaterial = __instance.laserLine.material;
        }
    }
}

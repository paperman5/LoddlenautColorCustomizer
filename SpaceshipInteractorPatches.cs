using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ColorCustomizer
{
    [HarmonyPatch(typeof(SpaceshipInteractor))]
    public static class SpaceshipInteractorPatches
    {
        [HarmonyPatch(nameof(SpaceshipInteractor.Awake))]
        [HarmonyPostfix]
        public static void Awake_Postfix(SpaceshipInteractor __instance)
        {
            CustomizerMod.ApplyAllShipColors();
        }
    }
}

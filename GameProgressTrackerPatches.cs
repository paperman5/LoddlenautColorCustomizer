using HarmonyLib;
using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ColorCustomizer
{
    [HarmonyPatch(typeof(GameProgressTracker))]
    public static class GameProgressTrackerPatches
    {
        [HarmonyPatch(nameof(GameProgressTracker.Update))]
        [HarmonyPostfix]
        public static void Update_Postfix(GameProgressTracker __instance)
        {
            // For debugging, reload the color customizer UI (for use after reloading with ScriptEngine)
            if (Input.GetKeyDown(CustomizerMod.startupKeyCode))
            {
                UIManagerPatches.Awake_Prefix(EngineHub.UIManager);
            }
            if (Input.GetKeyDown(CustomizerMod.testingKeyCode))
            {
                CustomizerMod.SaveTexture2DArray(CustomizerMod.jetpackOxygenMeterMaterial, "_TextureArray");
            }
        }
    }
}

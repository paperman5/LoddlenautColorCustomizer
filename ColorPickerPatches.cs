using HarmonyLib;
using Rewired.Utils.Classes.Utility;
using System;
using System.Management.Instrumentation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions.ColorPicker;

namespace ColorCustomizer
{
    [HarmonyPatch(typeof(ColorLabel))]
    public static class ColorLabelPatches
    {
        // Patch ColorLabel to use a TextMeshPro text object instead of a regular one
        [HarmonyPatch(nameof(ColorLabel.Awake))]
        [HarmonyPrefix]
        public static bool Awake_Prefix(ColorLabel __instance)
        {
            GameObject.DestroyImmediate(__instance.GetComponent<Text>());
            return false;
        }

        [HarmonyPatch(nameof(ColorLabel.UpdateValue))]
        [HarmonyPrefix]
        public static bool UpdateValue_Prefix(ColorLabel __instance)
        {
            if (__instance.picker == null)
            {
                __instance.label.text = __instance.prefix + "-";
                return false;
            }
            float value = __instance.minValue + __instance.picker.GetValue(__instance.type) * (__instance.maxValue - __instance.minValue);
            TextMeshProUGUI label = __instance.GetComponent<TextMeshProUGUI>();
            if (label != null)
                label.text = __instance.prefix + __instance.ConvertToDisplayString(value);
            
            return false;
        }
    }

    [HarmonyPatch(typeof(ColorPickerPresets))]
    public static class ColorPickerPresetsPatches
    {
        [HarmonyPatch(nameof(ColorPickerPresets.CreatePreset), new Type[] {typeof(Color), typeof(bool)})]
        [HarmonyPostfix]
        public static void CreatePreset_Postfix(ColorPickerPresets __instance)
        {
            CustomizerMod.customizerUI.StartCoroutine(CustomizerMod.customizerUI.RecalculatePickerPosition());
        }
    }
}

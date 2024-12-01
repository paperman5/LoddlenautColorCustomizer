using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ColorCustomizer
{
    [HarmonyPatch(typeof(UIManager))]
    public static class UIManagerPatches
    {
        [HarmonyPatch(nameof(UIManager.Awake))]
        [HarmonyPrefix]
        public static void Awake_Prefix(UIManager __instance)
        {
            AddColorCustomizerCanvas(__instance);
        }

        [HarmonyPatch(nameof(UIManager.Awake))]
        [HarmonyPostfix]
        public static void Awake_Postfix(UIManager __instance)
        {
            // The UIManager hudCanvasIndices needs to be updated but the indices can be different than
            // the transform's child index. We need the index from the allCanavases array.
            List<int> newHuds = __instance.hudCanvasIndices.ToList();
            for (int i = 0; i < __instance.allCanvases.Length; i++)
            {
                if (__instance.allCanvases[i].GetComponent<ColorCustomizerUI>() != null)
                {
                    newHuds.Add(i);
                    break;
                }
            }
            __instance.hudCanvasIndices = newHuds.ToArray();
        }

        public static void AddColorCustomizerCanvas(UIManager uiManager)
        {
            GameObject referenceCanvas = GameObject.Find("/PlayerRoot/UI Manager/HUD Canvas");
            GameObject newCanvas = GameObject.Instantiate(referenceCanvas);
            newCanvas.name = "ColorCustomizerCanvas";
            while (newCanvas.transform.childCount > 0)
            {
                GameObject.DestroyImmediate(newCanvas.transform.GetChild(0).gameObject);
            }

            newCanvas.transform.SetParent(uiManager.transform);
            GameObject colorCustomizerRoot = new GameObject("ColorCustomizerUI");
            colorCustomizerRoot.AddComponent<RectTransform>();
            colorCustomizerRoot.transform.SetParent(newCanvas.transform, false);

            colorCustomizerRoot.AddComponent<ColorCustomizerUI>();

            CustomizerPlugin.Logger.LogInfo("Color Customizer canvas added");
        }
    }
}

using HarmonyLib;
using I2.Loc;
using NullSave;
using System.Collections;
using TMPro;
using UnityEngine;

namespace ColorCustomizer
{
    [HarmonyPatch(typeof(PlayerInventoryUI))]
    public static class PlayerInventoryUIPatches
    {
        private static float verticalShift = 57f;
        private static ReIconedTMPActionPlus normalInventoryIcon = null;
        private static ReIconedTMPActionPlus storageInventoryIcon = null;

        [HarmonyPatch(nameof(PlayerInventoryUI.Start))]
        [HarmonyPostfix]
        public static void Start_Postfix(PlayerInventoryUI __instance)
        {
            __instance.StartCoroutine(DelayedUpdateNormalInventoryUI(__instance));
            __instance.StartCoroutine(DelayedUpdateStorageInventoryUI(__instance));
        }

        public static IEnumerator DelayedUpdateNormalInventoryUI(PlayerInventoryUI __instance)
        {
            yield return new WaitForEndOfFrame();
            GameObject newIcon = GameObject.Instantiate(CustomizerMod.uiKeybindIconPrototype);
            GameObject newText = GameObject.Instantiate(CustomizerMod.uiKeybindTextPrototype);
            newIcon.name = "Inventory Controls - Customize Colors Icon";
            newText.name = "Inventory Controls - Customize Colors Text";
            InputModifier.UpdateKeybindHint(newIcon, newText, KeybindingNames.openColorMenu);

            Transform textParent = __instance.transform.parent.Find("Inventory Controls Text Parent/NormalInventoryControlsParent");
            // Before adding the new stuff to the parent, shift everything else up a bit
            for (int i = 0; i < textParent.childCount; i++)
            {
                RectTransform rect = textParent.GetChild(i).GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y + verticalShift);
            }

            newIcon.transform.SetParent(textParent, false);
            newText.transform.SetParent(textParent, false);
            newIcon.SetActive(true);
            newText.SetActive(true);
            normalInventoryIcon = newIcon.GetComponent<ReIconedTMPActionPlus>();
        }

        public static IEnumerator DelayedUpdateStorageInventoryUI(PlayerInventoryUI __instance)
        {
            yield return new WaitForEndOfFrame();
            GameObject newIcon = GameObject.Instantiate(CustomizerMod.uiKeybindIconPrototype);
            GameObject newText = GameObject.Instantiate(CustomizerMod.uiKeybindTextPrototype);
            newIcon.name = "Storage Controls - Customize Colors Icon";
            newText.name = "Storage Controls - Customize Colors Text";
            InputModifier.UpdateKeybindHint(newIcon, newText, KeybindingNames.openColorMenu);

            Transform textParent = __instance.transform.Find("StorageInventoryControlsParent/StorageInventoryControlsDefaultParent");
            // Before adding the new stuff to the parent, shift everything else up a bit
            for (int i = 0; i < textParent.childCount; i++)
            {
                RectTransform rect = textParent.GetChild(i).GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y + verticalShift);
            }

            newIcon.transform.SetParent(textParent, false);
            newText.transform.SetParent(textParent, false);
            newIcon.SetActive(true);
            newText.SetActive(true);
            storageInventoryIcon = newIcon.GetComponent<ReIconedTMPActionPlus>();
        }

        public static void RefreshControlIcons()
        {
            if (normalInventoryIcon != null)
            {
                normalInventoryIcon.UpdateUI(null);
            }
            if (storageInventoryIcon != null)
            {
                storageInventoryIcon.UpdateUI(null);
            }
        }
    }
}

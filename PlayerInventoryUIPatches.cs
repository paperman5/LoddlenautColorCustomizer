using HarmonyLib;
using I2.Loc;
using NullSave;
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
            UpdateNormalInventoryUI(__instance);
            UpdateStorageInventoryUI(__instance);
        }

        public static void UpdateNormalInventoryUI(PlayerInventoryUI __instance)
        {
            // Add a keybinding indication for opening the customizing UI
            Transform textParent = __instance.transform.parent.Find("Inventory Controls Text Parent/NormalInventoryControlsParent");
            GameObject iconTemplate = textParent.Find("Inventory Controls - Exit Inventory Icon").gameObject;
            GameObject textTemplate = textParent.Find("Inventory Controls - Exit Inventory Text").gameObject;

            GameObject newIcon = GameObject.Instantiate(iconTemplate);
            newIcon.name = "Inventory Controls - Customize Colors Icon";
            GameObject newText = GameObject.Instantiate(textTemplate);
            newText.name = "Inventory Controls - Customize Colors Text";

            // Remove I2 localization stuff
            GameObject.Destroy(newIcon.GetComponent<ReIconedTMPI2ActionPlus>());
            GameObject.Destroy(newText.GetComponent<Localize>());

            // Add new non-I2 stuff back in
            var iconComponent = newIcon.AddComponent<ReIconedTMPActionPlus>();
            var textComponent = newText.GetComponent<TextMeshProUGUI>();

            iconComponent.formatText = $"{{action:{KeybindingNames.openColorMenu}}}";
            textComponent.text = InputModifier.actionData[KeybindingNames.openColorMenu].actionDescriptiveName;

            // Before adding the new stuff to the parent, shift everything else up a bit
            for (int i = 0; i < textParent.childCount; i++)
            {
                RectTransform rect = textParent.GetChild(i).GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y + verticalShift);
            }

            newIcon.transform.SetParent(textParent, false);
            newText.transform.SetParent(textParent, false);
            normalInventoryIcon = iconComponent;
        }

        public static void UpdateStorageInventoryUI(PlayerInventoryUI __instance)
        {
            // Add a keybinding indication for opening the customizing UI
            Transform textParent = __instance.transform.Find("StorageInventoryControlsParent/StorageInventoryControlsDefaultParent");
            GameObject iconTemplate = textParent.Find("Storage Controls  - Exit Storage Icon").gameObject;
            GameObject textTemplate = textParent.Find("Storage Controls  - Exit Storage Text").gameObject;

            GameObject newIcon = GameObject.Instantiate(iconTemplate);
            newIcon.name = "Storage Controls - Customize Colors Icon";
            GameObject newText = GameObject.Instantiate(textTemplate);
            newText.name = "Storage Controls - Customize Colors Text";

            // Remove I2 localization stuff
            GameObject.Destroy(newIcon.GetComponent<ReIconedTMPI2ActionPlus>());
            GameObject.Destroy(newText.GetComponent<Localize>());

            // Add new non-I2 stuff back in
            var iconComponent = newIcon.AddComponent<ReIconedTMPActionPlus>();
            var textComponent = newText.GetComponent<TextMeshProUGUI>();

            iconComponent.formatText = $"{{action:{KeybindingNames.openColorMenu}}}";
            textComponent.text = InputModifier.actionData[KeybindingNames.openColorMenu].actionDescriptiveName;

            // Before adding the new stuff to the parent, shift everything else up a bit
            for (int i = 0; i < textParent.childCount; i++)
            {
                RectTransform rect = textParent.GetChild(i).GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y + verticalShift);
            }

            newIcon.transform.SetParent(textParent, false);
            newText.transform.SetParent(textParent, false);
            storageInventoryIcon = iconComponent;
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

using HarmonyLib;
using TMPro;
using UnityEngine;

namespace ColorCustomizer
{
    [HarmonyPatch(typeof(MainMenu))]
    public static class MainMenu_Patch
    {
        [HarmonyPatch(nameof(MainMenu.Start))]
        [HarmonyPostfix]
        public static void MainMenuStart_Postfix(MainMenu __instance)
        {
            // Add a mod version label on the title screen
            GameObject versionLabelObject = UnityEngine.GameObject.Find("Version Text");
            TextMeshProUGUI versionLabel = versionLabelObject.GetComponent<TextMeshProUGUI>();
            string version = versionLabel.text;
            versionLabel.text = $"{version}\n{MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION}";
            versionLabel.alignment = TextAlignmentOptions.BottomRight;
        }
    }
}

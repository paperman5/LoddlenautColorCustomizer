using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Reflection;
using BepInEx.Configuration;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Configuration;
using Rewired;
using System.IO;

namespace ColorCustomizer;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class CustomizerPlugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    private static readonly Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

    internal static AssetBundle customizerAssets;
    internal static Dictionary<string, ConfigEntry<KeyCode>> keyboardSettings;
    internal static Dictionary<string, ConfigEntry<int>> controllerSettings;
    internal static Dictionary<string, ConfigEntry<Color>> playerSettings;
    internal static Dictionary<string, ConfigEntry<Color>> shipSettings;
    internal static Dictionary<string, ConfigEntry<Color>> allSettings;

    private void Awake()
    {
        Logger = base.Logger;

        // Attempt to load Asset bundle
        Logger.LogInfo("Loading Color Customizer Asset Bundle");
        customizerAssets = AssetBundle.LoadFromFile(Path.Combine(BepInEx.Paths.PluginPath, "ColorCustomizer", "color_customizer"));
        if (customizerAssets == null)
        {
            Logger.LogError("Could not load asset bundle, aborting load");
            return;
        }

        allSettings = new Dictionary<string, ConfigEntry<Color>>();

        keyboardSettings = new Dictionary<string, ConfigEntry<KeyCode>>();
        string sectionName = "Keyboard Controls";
        foreach (var kvp in InputModifier.actionData)
        {
            ConfigEntry<KeyCode> keyConfigEntry = Config.Bind(sectionName, kvp.Key, kvp.Value.keyCode);
            keyConfigEntry.SettingChanged += (sender, args) => InputModifier.AddOrUpdateKeycodeMapping(kvp.Key, keyConfigEntry.Value);
            keyboardSettings.Add(kvp.Key, keyConfigEntry);
        }

        // TODO: Controller Settings

        playerSettings = new Dictionary<string, ConfigEntry<Color>>();
        foreach (var info in typeof(PlayerKeyNames).GetFields(BindingFlags.Static | BindingFlags.Public))
        {
            if (info.IsLiteral && !info.IsInitOnly && info.FieldType == typeof(string))
            {
                string keyName = (string)info.GetRawConstantValue();
                Color defaultColor = CustomizerMod.playerColorDefaults[keyName];
                ConfigEntry<Color> configEntry = this.Config.Bind("Player Colors", keyName, defaultColor);
                configEntry.SettingChanged += (sender, args) => CustomizerMod.playerColorFunctions[keyName](configEntry.Value);
                playerSettings.Add(keyName, configEntry);
                allSettings.Add(keyName, configEntry);
            }
        }

        shipSettings = new Dictionary<string, ConfigEntry<Color>>();
        foreach (var info in typeof(ShipKeyNames).GetFields(BindingFlags.Static | BindingFlags.Public))
        {
            if (info.IsLiteral && !info.IsInitOnly && info.FieldType == typeof(string))
            {
                string keyName = (string)info.GetRawConstantValue();
                Color defaultColor = CustomizerMod.shipColorDefaults[keyName];
                ConfigEntry<Color> configEntry = this.Config.Bind("Ship Colors", keyName, defaultColor);
                configEntry.SettingChanged += (sender, args) => CustomizerMod.shipColorFunctions[keyName](configEntry.Value);
                shipSettings.Add(keyName, configEntry);
                allSettings.Add(keyName, configEntry);
            }
        }

        // Patching doesn't work the normal way for this game for whatever reason, manual override!
        AccessTools.GetTypesFromAssembly(Assembly.GetExecutingAssembly()).Do(delegate (Type type)
        {
            harmony.PatchAll(type);
        });

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void OnDestroy()
    {
        harmony?.UnpatchSelf();
        if (CustomizerMod.customizerUI != null)
            DestroyImmediate(CustomizerMod.customizerUI.transform.parent.gameObject);
        if (CustomizerMod.uiBackgroundPrototype != null)
            DestroyImmediate(CustomizerMod.uiBackgroundPrototype);
        if (CustomizerMod.uiButtonPrototype != null)
            DestroyImmediate(CustomizerMod.uiButtonPrototype);
        if (CustomizerMod.uiTitlePrototype != null)
            DestroyImmediate(CustomizerMod.uiTitlePrototype);
        if (CustomizerMod.uiPageSelectorPrototype != null)
            DestroyImmediate(CustomizerMod.uiPageSelectorPrototype);
        if (CustomizerMod.uiLeftArrowButtonPrototype != null)
            DestroyImmediate(CustomizerMod.uiLeftArrowButtonPrototype);
        if (CustomizerMod.uiRightArrowButtonPrototype != null)
            DestroyImmediate(CustomizerMod.uiRightArrowButtonPrototype);
        if (CustomizerMod.colorMenuEntryPrototype != null)
            DestroyImmediate(CustomizerMod.colorMenuEntryPrototype);
        customizerAssets.Unload(true);
    }
}

using HarmonyLib;
using Rewired;
using Rewired.Data.Mapping;

namespace ColorCustomizer
{
    [HarmonyPatch(typeof(InputManager_Base))]
    public static class InputManagerPatches
    {
        internal static int testActionId = 200;
        internal static int testCategoryId = 10;
        internal static int testBehaviorId = 2;

        [HarmonyPatch(nameof(InputManager_Base.Awake))]
        [HarmonyPrefix]
        public static void Awake_Prefix(InputManager_Base __instance)
        {
            // Add new input actions before InputManager gets initialized
            InputModifier.InjectNewControlData(__instance);
        }

        [HarmonyPatch(nameof(InputManager_Base.OnDestroy))]
        [HarmonyPostfix]
        public static void OnDestroy_Postfix(InputManager_Base __instance)
        {
            InputModifier.Reset();
        }
    }
}

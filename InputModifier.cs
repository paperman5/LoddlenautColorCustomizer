using BepInEx.Configuration;
using Rewired;
using Rewired.Data.Mapping;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ColorCustomizer
{
    internal static class InputModifier
    {
        private static readonly int baseActionId = 200;
        private static readonly int customizerCategoryId = 10;
        private static readonly int menuBehaviorId = 2;
        private static readonly int structureCategoryId = 5;
        private static readonly int inventoryCategoryId = 4;
        private static readonly string customizerCategoryName = "ColorCustomizer";
        private static readonly string structureCategoryName = "Structure";

        private static bool controlsInitialized = false;
        private static bool controlsApplied = false;
        private static KeyboardMap keyboardMap;
        private static JoystickMap joystickMap;

        internal static Dictionary<string, ActionData> actionData = new Dictionary<string, ActionData>
        {
            { 
                KeybindingNames.openColorMenu, new ActionData
                {
                    actionId = 200,
                    categoryId = inventoryCategoryId,
                    keyCode = KeyCode.C,
                    controllerButton = 0,
                    actionName = KeybindingNames.openColorMenu,
                    actionDescriptiveName = "Customize Colors",
                }
            },
            {
                KeybindingNames.rotateModelLeft, new ActionData
                {
                    actionId = 201,
                    categoryId = customizerCategoryId,
                    keyCode = KeyCode.Z,
                    controllerButton = 0,
                    actionName = KeybindingNames.rotateModelLeft,
                    actionDescriptiveName = "Rotate Left",
                }
            },
            {
                KeybindingNames.rotateModelRight, new ActionData
                {
                    actionId = 202,
                    categoryId = customizerCategoryId,
                    keyCode = KeyCode.X,
                    controllerButton = 0,
                    actionName = KeybindingNames.rotateModelRight,
                    actionDescriptiveName = "Rotate Right",
                }
            }
        };

        internal static void InjectNewControlData(InputManager_Base inputManager)
        {
            // Must be called before the InputManager_Base.Awake() method as it alters the serialized data
            // loaded into ReWired on initialization. A good place to do this is in a prefix wrapper.
            if (!controlsInitialized)
            {
                // Action category, maybe not strictly necessary
                InputCategory customizerCategory = new InputCategory  // Not part of the Input Map Categories
                {
                    name = customizerCategoryName,
                    descriptiveName = customizerCategoryName,
                    id = customizerCategoryId,
                    userAssignable = false,
                };

                // Add individual actions from config
                List<InputAction> actions = new List<InputAction>();
                foreach (var action in actionData)
                {
                    actions.Add(new InputAction
                    {
                        id = action.Value.actionId,
                        name = action.Key,
                        descriptiveName = action.Value.actionDescriptiveName,
                        behaviorId = menuBehaviorId,
                        categoryId = action.Value.categoryId,
                        type = InputActionType.Button,
                    });
                }

                // New map category so we can "overlay" our controls on the structure controls
                InputMapCategory customizerMapCategory = new InputMapCategory
                {
                    name = customizerCategoryName,
                    descriptiveName = customizerCategoryName,
                    id = customizerCategoryId,
                    userAssignable = false,
                };

                ActionCategoryMap.Entry customizerCategoryMapEntry = new ActionCategoryMap.Entry
                {
                    categoryId = customizerCategoryId,
                };

                inputManager.userData.actionCategories.Add(customizerCategory);
                inputManager.userData.mapCategories.Add(customizerMapCategory);
                foreach (var action in actions)
                {
                    inputManager.userData.actions.Add(action);
                }
                inputManager.userData.actionCategoryMap.list.Add(customizerCategoryMapEntry);

                foreach (var acm in inputManager.userData.actionCategoryMap.list)
                {
                    if (acm.categoryId == customizerCategoryId)
                    {
                        foreach (var action in actions)
                        {
                            acm.AddAction(action.id);
                        }
                        break;
                    }
                }

                controlsInitialized = true;
            }
        }

        internal static void CreateAndApplyCustomMaps()
        {
            if (controlsApplied)
                return;
            foreach (var data in actionData.Values)
            {
                data.keyCode = CustomizerPlugin.keyboardSettings[data.actionName].Value;
                // TODO: Controller settings
            }
            SetupKeyboardMap();
            SetupJoystickMap();
            UpdateMapManagerRulesets();
            controlsApplied = true;
        }

        internal static void AddOrUpdateKeycodeMapping(string actionName, KeyCode keyCode)
        {
            if (!actionData.ContainsKey(actionName))
                return;

            var playerMaps = ReInput.players.GetPlayer(0).controllers.maps;
            var data = actionData[actionName];
            var map = (KeyboardMap)playerMaps.GetFirstMapInCategory(ControllerType.Keyboard, 0, data.categoryId);
            if (map == null)
            {
                CustomizerPlugin.Logger.LogInfo($"Keyboard map does not contain category {data.categoryId}, creating");
                map = new KeyboardMap
                {
                    categoryId = data.categoryId,
                    layoutId = 0,
                };
                ReInput.players.GetPlayer(0).controllers.maps.AddMap(ControllerType.Keyboard, 0, map);
            }
            CustomizerPlugin.Logger.LogInfo($"Got keyboard map for category {data.categoryId}");
            var action = map.GetFirstElementMapWithAction(data.actionId);
            if (action == null)
            {
                CustomizerPlugin.Logger.LogInfo($"Keyboard map does not contain entry for action {data.actionId}, creating");
                map.CreateElementMap(new ElementAssignment(keyCode, ModifierKeyFlags.None, data.actionId, Pole.Positive));
            }
            else
            {
                CustomizerPlugin.Logger.LogInfo($"Keyboard map already contains entry for action {data.actionId}, updating");
                action.keyCode = keyCode;
            }
            PlayerInventoryUIPatches.RefreshControlIcons();
        }

        private static void SetupKeyboardMap()
        {
            // Create a new ReWired keyboard map with our custom controls and apply it to the current player
            //var playerMaps = ReInput.players.GetPlayer(0).controllers.maps;
            //keyboardMap = new KeyboardMap
            //{
            //    categoryId = customizerCategoryId,
            //    layoutId = 0,
            //};
            //foreach (var ar in actionData)
            //{
            //    KeyboardMap map = ar.Value.categoryId == customizerCategoryId ? 
            //        keyboardMap : 
            //        (KeyboardMap)playerMaps.GetFirstMapInCategory(ControllerType.Keyboard, 0, ar.Value.categoryId);
            //    KeyCode keyCode = ar.Value.keyCode;
            //    int actionId = ar.Value.actionId;
            //    map.CreateElementMap(new ElementAssignment(keyCode, ModifierKeyFlags.None, actionId, Pole.Positive));
            //}
            //ReInput.players.GetPlayer(0).controllers.maps.AddMap(ControllerType.Keyboard, 0, keyboardMap);
            foreach (var data in actionData.Values)
            {
                AddOrUpdateKeycodeMapping(data.actionName, data.keyCode);
            }
        }

        private static void SetupJoystickMap()
        {

        }

        private static void UpdateMapManagerRulesets()
        {
            // Loddlenaut uses a few different rulesets to enable/disable categories of controller maps for
            // different game contexts. These rulesets are enabled/disabled based on an enum so it is difficult
            // to add a completely new one. So we just alter what is already present since we want to use the
            // Structure UI context anyway.
            ControllerMapEnabler mapEnabler = ReInput.players.GetPlayer(0).controllers.maps.mapEnabler;
            foreach (var ruleSet in mapEnabler.ruleSets)
            {
                foreach (var rule in ruleSet.rules)
                {
                    // Whatever happens to the Structure control map (enable/disable) should happen to the custom map as well
                    if (rule.categoryIds.Contains(structureCategoryId))
                    {
                        var temp = rule.categoryIds.ToList();
                        temp.Add(customizerCategoryId);
                        rule.categoryIds = temp.ToArray();
                    }
                }
            }
            mapEnabler.Apply();
        }

        internal static void Reset()
        {
            controlsInitialized = false;
            controlsApplied = false;
            keyboardMap = null;
            joystickMap = null;
        }
    }

    public class ActionData
    {
        public int actionId;
        public int categoryId;
        public KeyCode keyCode;
        public int controllerButton;
        public string actionName;
        public string actionDescriptiveName;
    }
}

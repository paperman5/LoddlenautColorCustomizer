﻿using Rewired;
using Rewired.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ColorCustomizer
{
    public static class CustomizerMod
    {
        public static readonly KeyCode startupKeyCode = KeyCode.F3;
        public static readonly KeyCode testingKeyCode = KeyCode.F4;

        public static ColorCustomizerUI customizerUI = null;
        public static GameObject uiBackgroundPrototype = null;
        public static GameObject uiButtonPrototype = null;
        public static GameObject uiTitlePrototype = null;
        public static GameObject uiPageSelectorPrototype = null;
        public static GameObject uiLeftArrowButtonPrototype = null;
        public static GameObject uiRightArrowButtonPrototype = null;
        public static GameObject colorMenuEntryPrototype = null;

        internal static Material playerSuitMaterial = null;             // Set by PlayerModelControllerPatches
        internal static Material playerDepthSuitMaterial = null;        
        internal static Material playerAntennaInnerMaterial = null;     
        internal static Material playerAntennaOuterMaterial = null;     
        internal static Material playerHelmetWindowMaterial = null;     
        internal static Material playerHelmetLightsMaterial = null;     
        internal static Material playerJetpackMaterial = null;          // Set by PlayerMovementControllerPatches
        internal static Material jetpackUpArrowMaterial = null;         
        internal static Material jetpackDownArrowMaterial = null;       
        internal static Material jetpackTubeMaterial = null;            
        internal static Material jetpackBoostMeterMaterial = null;      
        internal static Material jetpackOxygenMeterMaterial = null;     
        internal static Material jetpackPropellerMaterial = null;       
        internal static Material jetpackPropellerBoostRingMaterial = null;
        internal static Material jetpackOxygenUpgradeMaterial = null;
        internal static Material jetpackOxygenUpgradeSlotMaterial = null;

        #region Player Data

        // Used to look up the proper texture to modify
        public enum PlayerTextureCategory
        {
            SUIT,
            DEPTH_SUIT,
            JETPACK,
            PROPELLER,
            OXYGEN_METER,
            OXYGEN_UPGRADE,
            OXYGEN_UPGRADE_SLOT,
            BUBBLE_GUN,
            SCRAP_VAC,
            PUDDLE_SCRUBBER,
        }

        // Used to look up the correct material to modify
        public enum PlayerShaderCategory
        {
            ANTENNA_INNER,
            ANTENNA_OUTER,
            HELMET_WINDOW,
            HELMET_LIGHTS,
            JETPACK_ARROWS,
            JETPACK_BOOST,
            JETPACK_OXYGEN_UPGRADE,
            JETPACK_TUBE,
        }

        // Texture regions for modifying each texture-based color
        public static Dictionary<string, RectInt> playerTextureCoords = new Dictionary<string, RectInt>
        {
            // Standard suit texture
            { PlayerKeyNames.helmetMain,        new RectInt(0,  0,  32, 32) },
            { PlayerKeyNames.helmetWindowRing,  new RectInt(32, 0,  32, 32) },
            { PlayerKeyNames.suitCollar,        new RectInt(64, 0,  32, 32) },
            { PlayerKeyNames.helmetAntennaStem, new RectInt(96, 0,  32, 32) },
            { PlayerKeyNames.suitMain,          new RectInt(0,  32, 32, 32) },
            { PlayerKeyNames.suitArmsLegs,      new RectInt(32, 32, 32, 32) },
            { PlayerKeyNames.suitCuffs,         new RectInt(64, 32, 32, 32) },
            { PlayerKeyNames.suitLapel1,        new RectInt(0,  64, 32, 32) },
            { PlayerKeyNames.suitLapel2,        new RectInt(32, 64, 32, 32) },
            { PlayerKeyNames.suitPocketBottom,  new RectInt(64, 64, 32, 32) },
            { PlayerKeyNames.suitPocketTop,     new RectInt(96, 64, 32, 32) },

            // Depth suit texture
            { PlayerKeyNames.helmetDepthMain,       new RectInt(0,  0,  32, 32) },
            { PlayerKeyNames.helmetDepthWindowRing, new RectInt(32, 0,  32, 32) },
            { PlayerKeyNames.suitDepthCollar,       new RectInt(64, 0,  32, 32) },
            { PlayerKeyNames.helmetDepthAntennaStem,new RectInt(96, 0,  32, 32) },
            { PlayerKeyNames.suitDepthMain,         new RectInt(0,  32, 32, 32) },
            { PlayerKeyNames.suitDepthArmsLegs,     new RectInt(32, 32, 32, 32) },
            { PlayerKeyNames.suitDepthCuffs,        new RectInt(64, 32, 32, 32) },
            { PlayerKeyNames.suitDepthLapel1,       new RectInt(0,  64, 32, 32) },
            { PlayerKeyNames.suitDepthLapel2,       new RectInt(32, 64, 32, 32) },
            { PlayerKeyNames.suitDepthPocketBottom, new RectInt(64, 64, 32, 32) },
            { PlayerKeyNames.suitDepthPocketTop,    new RectInt(96, 64, 32, 32) },

            // Jetpack texture
            { PlayerKeyNames.jetpackTop,                        new RectInt(0,   0,   64,  64 ) },
            { PlayerKeyNames.jetpackTopBooster1,                new RectInt(64,  0,   64,  64 ) },
            { PlayerKeyNames.jetpackTopBooster2,                new RectInt(128, 0,   64,  64 ) },
            { PlayerKeyNames.jetpackBottom,                     new RectInt(0,   64,  64,  64 ) },
            { PlayerKeyNames.jetpackBottomBooster1,             new RectInt(64,  64,  64,  64 ) },
            { PlayerKeyNames.jetpackBottomBooster2,             new RectInt(128, 64,  64,  64 ) },
            { PlayerKeyNames.jetpackPropellerConnector,         new RectInt(0,   128, 64,  32 ) },
            { PlayerKeyNames.jetpackPipeConnector,              new RectInt(0,   160, 64,  32 ) },
            { PlayerKeyNames.jetpackPropellerConnectorInside,   new RectInt(64,  128, 64,  64 ) },
            { PlayerKeyNames.jetpackPropellerBlades,            new RectInt(0,   0,   128, 128) },
            { PlayerKeyNames.jetpackPropellerShaft,             new RectInt(128, 0,   128, 128) },
            { PlayerKeyNames.jetpackPropellerHub,               new RectInt(0,   128, 128, 128) },

            // Oxygen upgrade texture
            { PlayerKeyNames.jetpackOxygenUpgradeLight,         new RectInt(0,   0,   64,  64) },
            { PlayerKeyNames.jetpackOxygenUpgradeCap,           new RectInt(64,  0,   64,  64) },

            // Oxygen capsule slot texture
            { PlayerKeyNames.jetpackOxygenUpgradeRing1,         new RectInt(0,   0,  128, 128) },
            { PlayerKeyNames.jetpackOxygenUpgradeRing2,         new RectInt(128, 0,  128, 128) },
        };

        // For easy color changing, just call `playerColorFunctions[key](colorToSet)`
        public static Dictionary<string, Action<Color>> playerColorFunctions = new Dictionary<string, Action<Color>>
        {
            { PlayerKeyNames.helmetMain,        color => SetPlayerTextureColor(PlayerKeyNames.helmetMain,          color) },
            { PlayerKeyNames.helmetWindowRing,  color => SetPlayerTextureColor(PlayerKeyNames.helmetWindowRing,    color) },
            { PlayerKeyNames.suitCollar,        color => SetPlayerTextureColor(PlayerKeyNames.suitCollar,          color) },
            { PlayerKeyNames.helmetAntennaStem, color => SetPlayerTextureColor(PlayerKeyNames.helmetAntennaStem,   color) },
            { PlayerKeyNames.suitMain,          color => SetPlayerTextureColor(PlayerKeyNames.suitMain,            color) },
            { PlayerKeyNames.suitArmsLegs,      color => SetPlayerTextureColor(PlayerKeyNames.suitArmsLegs,        color) },
            { PlayerKeyNames.suitCuffs,         color => SetPlayerTextureColor(PlayerKeyNames.suitCuffs,           color) },
            { PlayerKeyNames.suitLapel1,        color => SetPlayerTextureColor(PlayerKeyNames.suitLapel1,          color) },
            { PlayerKeyNames.suitLapel2,        color => SetPlayerTextureColor(PlayerKeyNames.suitLapel2,          color) },
            { PlayerKeyNames.suitPocketBottom,  color => SetPlayerTextureColor(PlayerKeyNames.suitPocketBottom,    color) },
            { PlayerKeyNames.suitPocketTop,     color => SetPlayerTextureColor(PlayerKeyNames.suitPocketTop,       color) },

            { PlayerKeyNames.helmetDepthMain,       color => SetPlayerTextureColor(PlayerKeyNames.helmetDepthMain,          color) },
            { PlayerKeyNames.helmetDepthWindowRing, color => SetPlayerTextureColor(PlayerKeyNames.helmetDepthWindowRing,    color) },
            { PlayerKeyNames.suitDepthCollar,       color => SetPlayerTextureColor(PlayerKeyNames.suitDepthCollar,          color) },
            { PlayerKeyNames.helmetDepthAntennaStem,color => SetPlayerTextureColor(PlayerKeyNames.helmetDepthAntennaStem,   color) },
            { PlayerKeyNames.suitDepthMain,         color => SetPlayerTextureColor(PlayerKeyNames.suitDepthMain,            color) },
            { PlayerKeyNames.suitDepthArmsLegs,     color => SetPlayerTextureColor(PlayerKeyNames.suitDepthArmsLegs,        color) },
            { PlayerKeyNames.suitDepthCuffs,        color => SetPlayerTextureColor(PlayerKeyNames.suitDepthCuffs,           color) },
            { PlayerKeyNames.suitDepthLapel1,       color => SetPlayerTextureColor(PlayerKeyNames.suitDepthLapel1,          color) },
            { PlayerKeyNames.suitDepthLapel2,       color => SetPlayerTextureColor(PlayerKeyNames.suitDepthLapel2,          color) },
            { PlayerKeyNames.suitDepthPocketBottom, color => SetPlayerTextureColor(PlayerKeyNames.suitDepthPocketBottom,    color) },
            { PlayerKeyNames.suitDepthPocketTop,    color => SetPlayerTextureColor(PlayerKeyNames.suitDepthPocketTop,       color) },

            { PlayerKeyNames.jetpackTop,                        color => SetPlayerTextureColor(PlayerKeyNames.jetpackTop,                       color) },
            { PlayerKeyNames.jetpackBottom,                     color => SetPlayerTextureColor(PlayerKeyNames.jetpackBottom,                    color) },
            { PlayerKeyNames.jetpackTopBooster1,                color => SetPlayerTextureColor(PlayerKeyNames.jetpackTopBooster1,               color) },
            { PlayerKeyNames.jetpackTopBooster2,                color => SetPlayerTextureColor(PlayerKeyNames.jetpackTopBooster2,               color) },
            { PlayerKeyNames.jetpackBottomBooster1,             color => SetPlayerTextureColor(PlayerKeyNames.jetpackBottomBooster1,            color) },
            { PlayerKeyNames.jetpackBottomBooster2,             color => SetPlayerTextureColor(PlayerKeyNames.jetpackBottomBooster2,            color) },
            { PlayerKeyNames.jetpackPropellerConnector,         color => SetPlayerTextureColor(PlayerKeyNames.jetpackPropellerConnector,        color) },
            { PlayerKeyNames.jetpackPropellerConnectorInside,   color => SetPlayerTextureColor(PlayerKeyNames.jetpackPropellerConnectorInside,  color) },
            { PlayerKeyNames.jetpackPipeConnector,              color => SetPlayerTextureColor(PlayerKeyNames.jetpackPipeConnector,             color) },
            { PlayerKeyNames.jetpackPropellerBlades,            color => SetPlayerTextureColor(PlayerKeyNames.jetpackPropellerBlades,           color) },
            { PlayerKeyNames.jetpackPropellerShaft,             color => SetPlayerTextureColor(PlayerKeyNames.jetpackPropellerShaft,            color) },
            { PlayerKeyNames.jetpackPropellerHub,               color => SetPlayerTextureColor(PlayerKeyNames.jetpackPropellerHub,              color) },
            { PlayerKeyNames.jetpackOxygenMeter,                color => SetPlayerTextureColor(PlayerKeyNames.jetpackOxygenMeter,               color) },
            { PlayerKeyNames.jetpackOxygenMeterLow,             color => SetPlayerTextureColor(PlayerKeyNames.jetpackOxygenMeterLow,            color) },
            { PlayerKeyNames.jetpackOxygenMeterOff,             color => SetPlayerTextureColor(PlayerKeyNames.jetpackOxygenMeterOff,            color) },
            { PlayerKeyNames.jetpackOxygenUpgradeLight,         color => SetPlayerTextureColor(PlayerKeyNames.jetpackOxygenUpgradeLight,        color) },
            { PlayerKeyNames.jetpackOxygenUpgradeCap,           color => SetPlayerTextureColor(PlayerKeyNames.jetpackOxygenUpgradeCap,          color) },
            { PlayerKeyNames.jetpackOxygenUpgradeRing1,         color => SetPlayerTextureColor(PlayerKeyNames.jetpackOxygenUpgradeRing1,        color) },
            { PlayerKeyNames.jetpackOxygenUpgradeRing2,         color => SetPlayerTextureColor(PlayerKeyNames.jetpackOxygenUpgradeRing2,        color) },

            { PlayerKeyNames.suitAntennaOrbInner,       color => SetPlayerShaderParamColor(PlayerKeyNames.suitAntennaOrbInner,          color) },
            { PlayerKeyNames.suitAntennaOrbOuter,       color => SetPlayerShaderParamColor(PlayerKeyNames.suitAntennaOrbOuter,          color) },
            { PlayerKeyNames.helmetWindow,              color => SetPlayerShaderParamColor(PlayerKeyNames.helmetWindow,                 color) },
            { PlayerKeyNames.helmetCheeks,              color => SetPlayerShaderParamColor(PlayerKeyNames.helmetCheeks,                 color) },
            { PlayerKeyNames.jetpackPipe,               color => SetPlayerShaderParamColor(PlayerKeyNames.jetpackPipe,                  color) },
            { PlayerKeyNames.jetpackArrows,             color => SetPlayerShaderParamColor(PlayerKeyNames.jetpackArrows,                color) },
            { PlayerKeyNames.jetpackArrowsOff,          color => SetPlayerShaderParamColor(PlayerKeyNames.jetpackArrowsOff,             color) },
            { PlayerKeyNames.jetpackBoostMeter,         color => SetPlayerShaderParamColor(PlayerKeyNames.jetpackBoostMeter,            color) },
            { PlayerKeyNames.jetpackBoostMeterUpgrade,  color => SetPlayerShaderParamColor(PlayerKeyNames.jetpackBoostMeterUpgrade,     color) },
            { PlayerKeyNames.jetpackBoostMeterOverdrive,color => SetPlayerShaderParamColor(PlayerKeyNames.jetpackBoostMeterOverdrive,   color) },
            { PlayerKeyNames.jetpackBoostMeterOff,      color => SetPlayerShaderParamColor(PlayerKeyNames.jetpackBoostMeterOff,         color) },
            { PlayerKeyNames.jetpackOxygenUpgradeGlow,  color => SetPlayerShaderParamColor(PlayerKeyNames.jetpackOxygenUpgradeGlow,     color) },
        };

        public static Dictionary<string, Color> playerColorDefaults = new Dictionary<string, Color>
        {
            { PlayerKeyNames.helmetMain,         GetColorFromString("dbdbd6") },
            { PlayerKeyNames.helmetWindowRing,   GetColorFromString("b8b7b2") },
            { PlayerKeyNames.suitCollar,         GetColorFromString("cb703f") },
            { PlayerKeyNames.helmetAntennaStem,  GetColorFromString("b8b7b2") },
            { PlayerKeyNames.suitMain,           GetColorFromString("da7945") },
            { PlayerKeyNames.suitArmsLegs,       GetColorFromString("d3a23c") },
            { PlayerKeyNames.suitCuffs,          GetColorFromString("cb703f") },
            { PlayerKeyNames.suitLapel1,         GetColorFromString("ba673c") },
            { PlayerKeyNames.suitLapel2,         GetColorFromString("cb703f") },
            { PlayerKeyNames.suitPocketBottom,   GetColorFromString("ba673c") },
            { PlayerKeyNames.suitPocketTop,      GetColorFromString("cb703f") },

            { PlayerKeyNames.helmetDepthMain,        GetColorFromString("e6e1e1") },
            { PlayerKeyNames.helmetDepthWindowRing,  GetColorFromString("b8b4b5") },
            { PlayerKeyNames.suitDepthCollar,        GetColorFromString("993657") },
            { PlayerKeyNames.helmetDepthAntennaStem, GetColorFromString("b8b7b2") },
            { PlayerKeyNames.suitDepthMain,          GetColorFromString("d35f63") },
            { PlayerKeyNames.suitDepthArmsLegs,      GetColorFromString("e6a667") },
            { PlayerKeyNames.suitDepthCuffs,         GetColorFromString("993657") },
            { PlayerKeyNames.suitDepthLapel1,        GetColorFromString("942c4a") },
            { PlayerKeyNames.suitDepthLapel2,        GetColorFromString("aa3357") },
            { PlayerKeyNames.suitDepthPocketBottom,  GetColorFromString("942c4a") },
            { PlayerKeyNames.suitDepthPocketTop,     GetColorFromString("aa3357") },

            { PlayerKeyNames.suitAntennaOrbInner,   GetColorFromString("21BFA3") },
            { PlayerKeyNames.suitAntennaOrbOuter,   GetColorFromString("CCF5FF54") },
            { PlayerKeyNames.helmetWindow,          GetColorFromString("4F4E4D") },
            { PlayerKeyNames.helmetCheeks,          GetColorFromString("B18557") },
            
            { PlayerKeyNames.jetpackArrows,                     GetColorFromString("00BF3A") },
            { PlayerKeyNames.jetpackArrowsOff,                  GetColorFromString("164561") },
            { PlayerKeyNames.jetpackBoostMeter,                 GetColorFromString("0ABF41") },
            { PlayerKeyNames.jetpackBoostMeterUpgrade,          GetColorFromString("30BF30") },
            { PlayerKeyNames.jetpackBoostMeterOverdrive,        GetColorFromString("BFBF0A") },
            { PlayerKeyNames.jetpackBoostMeterOff,              GetColorFromString("164561") },
            { PlayerKeyNames.jetpackOxygenMeter,                GetColorFromString("70C8FF") },
            { PlayerKeyNames.jetpackOxygenMeterLow,             GetColorFromString("FF6160") },
            { PlayerKeyNames.jetpackOxygenMeterOff,             GetColorFromString("9A9A9A") },
            { PlayerKeyNames.jetpackTop,                        GetColorFromString("d1e7e7") },
            { PlayerKeyNames.jetpackTopBooster1,                GetColorFromString("99c7cb") },
            { PlayerKeyNames.jetpackTopBooster2,                GetColorFromString("8fbac0") },
            { PlayerKeyNames.jetpackBottom,                     GetColorFromString("526fe7") },
            { PlayerKeyNames.jetpackBottomBooster1,             GetColorFromString("4e67cd") },
            { PlayerKeyNames.jetpackBottomBooster2,             GetColorFromString("475fc0") },
            { PlayerKeyNames.jetpackPropellerConnector,         GetColorFromString("8c8c8c") },
            { PlayerKeyNames.jetpackPipeConnector,              GetColorFromString("8c8c8c") },
            { PlayerKeyNames.jetpackPropellerConnectorInside,   GetColorFromString("787878") },
            { PlayerKeyNames.jetpackPropellerBlades,            GetColorFromString("8ee4e7") },
            { PlayerKeyNames.jetpackPropellerShaft,             GetColorFromString("999a99") },
            { PlayerKeyNames.jetpackPropellerHub,               GetColorFromString("8381e7") },
            { PlayerKeyNames.jetpackPipe,                       GetColorFromString("CCFFFF80") },
            { PlayerKeyNames.jetpackOxygenUpgradeLight,         GetColorFromString("ffffff") },
            { PlayerKeyNames.jetpackOxygenUpgradeGlow,          GetColorFromString("2800bf") },
            { PlayerKeyNames.jetpackOxygenUpgradeCap,           GetColorFromString("b592ff") },
            { PlayerKeyNames.jetpackOxygenUpgradeRing1,         GetColorFromString("475999") },
            { PlayerKeyNames.jetpackOxygenUpgradeRing2,         GetColorFromString("39467b") },
        };

        public static Dictionary<string, PlayerTextureCategory> playerTextureCategories = new Dictionary<string, PlayerTextureCategory>
        {
            { PlayerKeyNames.helmetMain,        PlayerTextureCategory.SUIT },
            { PlayerKeyNames.helmetWindowRing,  PlayerTextureCategory.SUIT },
            { PlayerKeyNames.suitCollar,        PlayerTextureCategory.SUIT },
            { PlayerKeyNames.helmetAntennaStem, PlayerTextureCategory.SUIT },
            { PlayerKeyNames.suitMain,          PlayerTextureCategory.SUIT },
            { PlayerKeyNames.suitArmsLegs,      PlayerTextureCategory.SUIT },
            { PlayerKeyNames.suitCuffs,         PlayerTextureCategory.SUIT },
            { PlayerKeyNames.suitLapel1,        PlayerTextureCategory.SUIT },
            { PlayerKeyNames.suitLapel2,        PlayerTextureCategory.SUIT },
            { PlayerKeyNames.suitPocketBottom,  PlayerTextureCategory.SUIT },
            { PlayerKeyNames.suitPocketTop,     PlayerTextureCategory.SUIT },

            { PlayerKeyNames.helmetDepthMain,       PlayerTextureCategory.DEPTH_SUIT },
            { PlayerKeyNames.helmetDepthWindowRing, PlayerTextureCategory.DEPTH_SUIT },
            { PlayerKeyNames.suitDepthCollar,       PlayerTextureCategory.DEPTH_SUIT },
            { PlayerKeyNames.helmetDepthAntennaStem,PlayerTextureCategory.DEPTH_SUIT },
            { PlayerKeyNames.suitDepthMain,         PlayerTextureCategory.DEPTH_SUIT },
            { PlayerKeyNames.suitDepthArmsLegs,     PlayerTextureCategory.DEPTH_SUIT },
            { PlayerKeyNames.suitDepthCuffs,        PlayerTextureCategory.DEPTH_SUIT },
            { PlayerKeyNames.suitDepthLapel1,       PlayerTextureCategory.DEPTH_SUIT },
            { PlayerKeyNames.suitDepthLapel2,       PlayerTextureCategory.DEPTH_SUIT },
            { PlayerKeyNames.suitDepthPocketBottom, PlayerTextureCategory.DEPTH_SUIT },
            { PlayerKeyNames.suitDepthPocketTop,    PlayerTextureCategory.DEPTH_SUIT },

            { PlayerKeyNames.jetpackTop,                PlayerTextureCategory.JETPACK },
            { PlayerKeyNames.jetpackBottom,             PlayerTextureCategory.JETPACK },
            { PlayerKeyNames.jetpackTopBooster1,        PlayerTextureCategory.JETPACK },
            { PlayerKeyNames.jetpackTopBooster2,        PlayerTextureCategory.JETPACK },
            { PlayerKeyNames.jetpackBottomBooster1,     PlayerTextureCategory.JETPACK },
            { PlayerKeyNames.jetpackBottomBooster2,     PlayerTextureCategory.JETPACK },
            { PlayerKeyNames.jetpackPropellerConnector, PlayerTextureCategory.JETPACK },
            { PlayerKeyNames.jetpackPropellerConnectorInside, PlayerTextureCategory.JETPACK },
            { PlayerKeyNames.jetpackPipeConnector,      PlayerTextureCategory.JETPACK },
            
            { PlayerKeyNames.jetpackPropellerBlades,    PlayerTextureCategory.PROPELLER },
            { PlayerKeyNames.jetpackPropellerShaft,     PlayerTextureCategory.PROPELLER },
            { PlayerKeyNames.jetpackPropellerHub,       PlayerTextureCategory.PROPELLER },

            { PlayerKeyNames.jetpackOxygenMeter,    PlayerTextureCategory.OXYGEN_METER },
            { PlayerKeyNames.jetpackOxygenMeterLow, PlayerTextureCategory.OXYGEN_METER },
            { PlayerKeyNames.jetpackOxygenMeterOff, PlayerTextureCategory.OXYGEN_METER },

            { PlayerKeyNames.jetpackOxygenUpgradeLight, PlayerTextureCategory.OXYGEN_UPGRADE },
            { PlayerKeyNames.jetpackOxygenUpgradeCap,   PlayerTextureCategory.OXYGEN_UPGRADE },
            
            { PlayerKeyNames.jetpackOxygenUpgradeRing1, PlayerTextureCategory.OXYGEN_UPGRADE_SLOT },
            { PlayerKeyNames.jetpackOxygenUpgradeRing2, PlayerTextureCategory.OXYGEN_UPGRADE_SLOT },
        };

        public static Dictionary<string, PlayerShaderCategory> playerShaderCategories = new Dictionary<string, PlayerShaderCategory>
        {
            { PlayerKeyNames.suitAntennaOrbInner,       PlayerShaderCategory.ANTENNA_INNER  },
            { PlayerKeyNames.suitAntennaOrbOuter,       PlayerShaderCategory.ANTENNA_OUTER  },
            { PlayerKeyNames.helmetWindow,              PlayerShaderCategory.HELMET_WINDOW  },
            { PlayerKeyNames.helmetCheeks,              PlayerShaderCategory.HELMET_LIGHTS  },
            { PlayerKeyNames.jetpackPipe,               PlayerShaderCategory.JETPACK_TUBE   },
            { PlayerKeyNames.jetpackArrows,             PlayerShaderCategory.JETPACK_ARROWS },
            { PlayerKeyNames.jetpackArrowsOff,          PlayerShaderCategory.JETPACK_ARROWS },
            { PlayerKeyNames.jetpackBoostMeter,         PlayerShaderCategory.JETPACK_BOOST  },
            { PlayerKeyNames.jetpackBoostMeterUpgrade,  PlayerShaderCategory.JETPACK_BOOST  },
            { PlayerKeyNames.jetpackBoostMeterOverdrive,PlayerShaderCategory.JETPACK_BOOST  },
            { PlayerKeyNames.jetpackBoostMeterOff,      PlayerShaderCategory.JETPACK_BOOST  },
            { PlayerKeyNames.jetpackOxygenUpgradeGlow,  PlayerShaderCategory.JETPACK_OXYGEN_UPGRADE  },
        };

        // Certain player colors use HDR color to create a bloom effect. To set these colors properly we can use a base SDR color
        // and multiply it by an intensity. Final color is `BaseColor * (2^intensity)`.
        public static Dictionary<string, float> playerLightIntensities = new Dictionary<string, float>
        {
            { PlayerKeyNames.jetpackArrows, 1.4f },
            { PlayerKeyNames.jetpackBoostMeter, 1.25f },
            { PlayerKeyNames.jetpackBoostMeterUpgrade, 1.8f },
            { PlayerKeyNames.jetpackBoostMeterOverdrive, 2.407534f },
            { PlayerKeyNames.jetpackOxygenUpgradeGlow, 1.634228f },
            { PlayerKeyNames.suitAntennaOrbInner, 1.8f },
        };

        #endregion
        
        #region Ship Data

        public static Dictionary<string, RectInt> shipTextureCoords = new Dictionary<string, RectInt>
        {
            { ShipKeyNames.shipBodyMain,        new RectInt(0,   0,   64, 64) },
            { ShipKeyNames.shipBumperRing,      new RectInt(64,  0,   64, 64) },
            { ShipKeyNames.shipBumper1,         new RectInt(128, 0,   64, 64) },
            { ShipKeyNames.shipBumper2,         new RectInt(192, 0,   64, 64) },
            { ShipKeyNames.shipPortholeRings,   new RectInt(0,   64,  64, 64) },
            { ShipKeyNames.shipPortholeWindows, new RectInt(64,  64,  64, 64) },
            { ShipKeyNames.shipGrill1,          new RectInt(128, 64,  64, 64) },
            { ShipKeyNames.shipGrill2,          new RectInt(192, 64,  64, 64) },
            { ShipKeyNames.shipThrusterOutside, new RectInt(0,   128, 64, 64) },
            { ShipKeyNames.shipThrusterInside,  new RectInt(64,  128, 64, 64) },
            { ShipKeyNames.shipTopperUpper,     new RectInt(128, 128, 64, 64) },
            { ShipKeyNames.shipTopperLower,     new RectInt(192, 128, 64, 64) },
            { ShipKeyNames.shipBottom1,         new RectInt(0,   192, 64, 64) },
            { ShipKeyNames.shipBottom2,         new RectInt(64,  192, 64, 64) },
            { ShipKeyNames.shipBottom3,         new RectInt(128, 192, 64, 64) },
            { ShipKeyNames.shipAntennaStem,     new RectInt(192, 192, 64, 64) },
        };

        public static Dictionary<string, Action<Color>> shipColorFunctions = new Dictionary<string, Action<Color>>
        {
            { ShipKeyNames.shipBodyMain,        color => SetShipTextureColor(ShipKeyNames.shipBodyMain,         color) },
            { ShipKeyNames.shipBumperRing,      color => SetShipTextureColor(ShipKeyNames.shipBumperRing,       color) },
            { ShipKeyNames.shipBumper1,         color => SetShipTextureColor(ShipKeyNames.shipBumper1,          color) },
            { ShipKeyNames.shipBumper2,         color => SetShipTextureColor(ShipKeyNames.shipBumper2,          color) },
            { ShipKeyNames.shipPortholeRings,   color => SetShipTextureColor(ShipKeyNames.shipPortholeRings,    color) },
            { ShipKeyNames.shipPortholeWindows, color => SetShipTextureColor(ShipKeyNames.shipPortholeWindows,  color) },
            { ShipKeyNames.shipGrill1,          color => SetShipTextureColor(ShipKeyNames.shipGrill1,           color) },
            { ShipKeyNames.shipGrill2,          color => SetShipTextureColor(ShipKeyNames.shipGrill2,           color) },
            { ShipKeyNames.shipThrusterOutside, color => SetShipTextureColor(ShipKeyNames.shipThrusterOutside,  color) },
            { ShipKeyNames.shipThrusterInside,  color => SetShipTextureColor(ShipKeyNames.shipThrusterInside,   color) },
            { ShipKeyNames.shipTopperUpper,     color => SetShipTextureColor(ShipKeyNames.shipTopperUpper,      color) },
            { ShipKeyNames.shipTopperLower,     color => SetShipTextureColor(ShipKeyNames.shipTopperLower,      color) },
            { ShipKeyNames.shipBottom1,         color => SetShipTextureColor(ShipKeyNames.shipBottom1,          color) },
            { ShipKeyNames.shipBottom2,         color => SetShipTextureColor(ShipKeyNames.shipBottom2,          color) },
            { ShipKeyNames.shipBottom3,         color => SetShipTextureColor(ShipKeyNames.shipBottom3,          color) },
            { ShipKeyNames.shipAntennaStem,     color => SetShipTextureColor(ShipKeyNames.shipAntennaStem,      color) },
            { ShipKeyNames.shipAntennaBall,     color => SetShipShaderParamColor(ShipKeyNames.shipAntennaBall,  color) },
        };

        public static Dictionary<string, Color> shipColorDefaults = new Dictionary<string, Color>
        {
            { ShipKeyNames.shipBodyMain,        GetColorFromString("c0bfc0") },
            { ShipKeyNames.shipBumperRing,      GetColorFromString("8c8c8c") },
            { ShipKeyNames.shipBumper1,         GetColorFromString("c3693f") },
            { ShipKeyNames.shipBumper2,         GetColorFromString("a55734") },
            { ShipKeyNames.shipPortholeRings,   GetColorFromString("787878") },
            { ShipKeyNames.shipPortholeWindows, GetColorFromString("39363d") },
            { ShipKeyNames.shipGrill1,          GetColorFromString("8c8c8c") },
            { ShipKeyNames.shipGrill2,          GetColorFromString("787878") },
            { ShipKeyNames.shipThrusterOutside, GetColorFromString("8c8c8c") },
            { ShipKeyNames.shipThrusterInside,  GetColorFromString("787878") },
            { ShipKeyNames.shipTopperUpper,     GetColorFromString("526fe7") },
            { ShipKeyNames.shipTopperLower,     GetColorFromString("475fc0") },
            { ShipKeyNames.shipBottom1,         GetColorFromString("526fe7") },
            { ShipKeyNames.shipBottom2,         GetColorFromString("4e67cd") },
            { ShipKeyNames.shipBottom3,         GetColorFromString("475fc0") },
            { ShipKeyNames.shipAntennaStem,     GetColorFromString("8c8c8c") },
            { ShipKeyNames.shipAntennaBall,     GetColorFromString("48FFFF") },
        };

        #endregion

        #region Player Functions

        public static void SetPlayerTextureColor(string param, Color color)
        {
            if (!playerTextureCoords.ContainsKey(param))
                return;
            
            RectInt rect = playerTextureCoords[param];
            Texture2D tex = GetPlayerTexture(param);
            if (tex == null)
                return;
            SetTextureBlockColor(tex, rect, color);
        }

        public static void SetPlayerShaderParamColor(string playerKeyName, Color color)
        {
            Material targetMaterial = GetPlayerShaderMaterial(playerKeyName);
            if (targetMaterial == null)
            {
                return;
            }
            float colorIntensity = playerLightIntensities.ContainsKey(playerKeyName) ? playerLightIntensities[playerKeyName] : 0f;
            color *= Mathf.Pow(2, colorIntensity);
            switch (playerKeyName)
            {
                case PlayerKeyNames.suitAntennaOrbInner:
                case PlayerKeyNames.suitAntennaOrbOuter:
                case PlayerKeyNames.helmetWindow:
                case PlayerKeyNames.jetpackOxygenUpgradeGlow:
                case PlayerKeyNames.jetpackPipe:
                    targetMaterial.color = color;
                    break;
                case PlayerKeyNames.helmetCheeks:
                    targetMaterial.color = color;
                    // TODO: color resets when blink is used, fix this
                    break;
                case PlayerKeyNames.jetpackArrows:
                    EngineHub.PlayerMovement.arrowLitColor = color;
                    // TODO: preview this in some way?
                    break;
                case PlayerKeyNames.jetpackArrowsOff:
                    EngineHub.PlayerMovement.arrowUnlitColor = color;
                    Material targetMaterial2 = jetpackDownArrowMaterial;
                    targetMaterial.color = color;
                    targetMaterial2.color = color;
                    break;
                case PlayerKeyNames.jetpackBoostMeter:
                    if (!EngineHub.Upgrades.boostDurationUpgradeIsUnlocked)
                        targetMaterial.SetColor("_LowerColor", color);
                    break;
                case PlayerKeyNames.jetpackBoostMeterUpgrade:
                    if (EngineHub.Upgrades.boostDurationUpgradeIsUnlocked)
                        targetMaterial.SetColor("_LowerColor", color);
                    break;
                case PlayerKeyNames.jetpackBoostMeterOverdrive:
                    if (EngineHub.PlayerMeterManager.overdriveIsActive)
                        targetMaterial.SetColor("_LowerColor", color);
                    break;
                case PlayerKeyNames.jetpackBoostMeterOff:
                    targetMaterial.SetColor("_UpperColor", color);
                    break;

                default:
                    CustomizerPlugin.Logger.LogInfo($"Unrecognized shader parameter key: {playerKeyName}");
                    break;
            }
        }

        public static void ApplyAllPlayerColors()
        {
            foreach (var kvp in CustomizerPlugin.playerSettings)
            {
                if (kvp.Value.Value != (Color)kvp.Value.DefaultValue)
                {
                    playerColorFunctions[kvp.Key](kvp.Value.Value);
                }
            }
        }

        private static Material GetPlayerShaderMaterial(string playerKeyName)
        {
            if (customizerUI == null || !playerShaderCategories.ContainsKey(playerKeyName))
                return null;

            Material targetMaterial = null;
            switch (playerShaderCategories[playerKeyName])
            {
                case PlayerShaderCategory.ANTENNA_INNER:
                    targetMaterial = playerAntennaInnerMaterial;
                    break;
                case PlayerShaderCategory.ANTENNA_OUTER:
                    targetMaterial = playerAntennaOuterMaterial;
                    break;
                case PlayerShaderCategory.HELMET_WINDOW:
                    targetMaterial = playerHelmetWindowMaterial;
                    break;
                case PlayerShaderCategory.HELMET_LIGHTS:
                    targetMaterial = playerHelmetLightsMaterial;
                    break;
                case PlayerShaderCategory.JETPACK_ARROWS:
                    targetMaterial = jetpackUpArrowMaterial;
                    break;
                case PlayerShaderCategory.JETPACK_BOOST:
                    targetMaterial = jetpackBoostMeterMaterial;
                    break;
                case PlayerShaderCategory.JETPACK_OXYGEN_UPGRADE:
                    targetMaterial = jetpackOxygenUpgradeMaterial;
                    break;
                case PlayerShaderCategory.JETPACK_TUBE:
                    targetMaterial = jetpackTubeMaterial;
                    break;
                default:
                    CustomizerPlugin.Logger.LogError($"Shader material finding not implemented for key {playerKeyName}");
                    return null;
            }

            if (targetMaterial == null)
            {
                CustomizerPlugin.Logger.LogError($"Key {playerKeyName} recognized but could not get associated shader material");
                return null;
            }

            return targetMaterial;
        }

        private static Texture2D GetPlayerTexture(string playerKeyName)
        {
            if (customizerUI == null || !playerTextureCategories.ContainsKey(playerKeyName))
                return null;

            Material targetMaterial = null;
            Texture2D targetTexture = null;
            string targetTextureKey = "_MainTexture";
            switch (playerTextureCategories[playerKeyName])
            {
                case PlayerTextureCategory.SUIT:
                    targetMaterial = playerSuitMaterial;
                    targetTexture = (Texture2D)targetMaterial.GetTexture(targetTextureKey);
                    break;
                case PlayerTextureCategory.DEPTH_SUIT:
                    targetMaterial = playerDepthSuitMaterial;
                    targetTexture = (Texture2D)targetMaterial.GetTexture(targetTextureKey);
                    break;
                case PlayerTextureCategory.JETPACK:
                    targetMaterial = playerJetpackMaterial;
                    targetTexture = (Texture2D)targetMaterial.GetTexture(targetTextureKey);
                    break;
                case PlayerTextureCategory.PROPELLER:
                    targetMaterial = jetpackPropellerMaterial;
                    targetTexture = (Texture2D)targetMaterial.GetTexture(targetTextureKey);
                    break;
                case PlayerTextureCategory.OXYGEN_METER:
                    targetMaterial = jetpackOxygenMeterMaterial;
                    targetTexture = null;
                    // TODO: figure out texture array stuff
                    break;
                case PlayerTextureCategory.OXYGEN_UPGRADE:
                    targetMaterial = jetpackOxygenUpgradeMaterial;
                    targetTextureKey = "_MainTex";
                    targetTexture = (Texture2D)targetMaterial.GetTexture(targetTextureKey);
                    break;
                case PlayerTextureCategory.OXYGEN_UPGRADE_SLOT:
                    targetMaterial = jetpackOxygenUpgradeSlotMaterial;
                    targetTexture = (Texture2D)targetMaterial.GetTexture(targetTextureKey);
                    break;
                default:
                    CustomizerPlugin.Logger.LogError($"Material/texture finding not implemented for key {playerKeyName}");
                    return null;
            }

            if (targetMaterial == null || targetTexture == null)
            {
                CustomizerPlugin.Logger.LogError($"Key {playerKeyName} recognized but could not get associated material/texture");
                return null;
            }

            if (!targetTexture.isReadable)
            {
                targetTexture = MakeReadableTexture(targetTexture);
                targetMaterial.SetTexture(targetTextureKey, targetTexture);
            }
            return targetTexture;
        }

        #endregion

        #region Ship Functions

        public static void SetShipTextureColor(string param, Color color)
        {
            if (!shipTextureCoords.ContainsKey(param))
                return;

            RectInt rect = shipTextureCoords[param];
            Texture2D tex = GetShipTexture();
            if (tex == null)
                return;
            SetTextureBlockColor(tex, rect, color);
        }

        public static void SetShipShaderParamColor(string param, Color color)
        {
            if (EngineHub.Spaceship == null)
                return;

            Renderer r = GetShipRenderer();
            Material mat = null;
            switch (param)
            {
                case ShipKeyNames.shipAntennaBall:
                    mat = GetMaterialWithName(r, "SpaceshipBallMat");
                    mat.color = color;
                    break;

                default:
                    CustomizerPlugin.Logger.LogInfo($"Unrecognized shader parameter key: {param}");
                    break;
            }
        }

        public static void ApplyAllShipColors()
        {
            foreach (var kvp in CustomizerPlugin.shipSettings)
            {
                if (kvp.Value.Value != (Color)kvp.Value.DefaultValue)
                {
                    shipColorFunctions[kvp.Key](kvp.Value.Value);
                }
            }
        }

        private static Renderer GetShipRenderer()
        {
            return EngineHub.Spaceship?.transform.Find("SpaceshipModel").GetComponent<SkinnedMeshRenderer>();
        }

        private static Texture2D GetShipTexture()
        {
            if (EngineHub.Spaceship == null)
                return null;

            Renderer r = GetShipRenderer();
            Material shipMaterial = GetMaterialWithName(r, "SpaceshipBaseMat");
            if (shipMaterial == null)
            {
                CustomizerPlugin.Logger.LogError("Could not find main ship material");
                return null;
            }

            Texture2D tex = (Texture2D)shipMaterial.GetTexture("_MainTexture");
            if (!tex.isReadable)
            {
                tex = MakeReadableTexture(tex);
                shipMaterial.SetTexture("_MainTexture", tex);
            }
            return tex;
        }

        #endregion

        public static T CopyComponent<T>(T original, GameObject destination) where T : Component
        {
            // from https://discussions.unity.com/t/copy-a-component-at-runtime/71172/6
            Type type = original.GetType();
            var dst = destination.GetComponent(type) as T;
            if (!dst) dst = destination.AddComponent(type) as T;
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                if (field.IsStatic) continue;
                field.SetValue(dst, field.GetValue(original));
            }
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
                prop.SetValue(dst, prop.GetValue(original, null), null);
            }
            return dst as T;
        }

        private static void SetTextureBlockColor(Texture2D tex, RectInt rect, Color color)
        {
            //CustomizerPlugin.Logger.LogInfo($"Setting texture {tex.ToString()} rect {rect.ToString()} color {color.ToString()}");
            for (int ml = 0; ml < tex.mipmapCount; ml++)
            {
                int div = (int)Mathf.Pow(2, ml);
                for (int x = rect.x / div; x < (rect.x + rect.width) / div; x++)
                {
                    for (int y = rect.y / div; y < (rect.y + rect.height) / div; y++)
                    {
                        tex.SetPixel(x, tex.height - y - 1, color, ml);
                    }
                }
            }
            tex.Apply();
        }

        private static Texture2D MakeReadableTexture(Texture2D tex)
        {
            // If a Texture2D isn't readable (`isReadable` is false) you can't read or write to it on the CPU.
            // To fix this we can use Graphics.Blit to copy from the GPU then copy that to the CPU in a new
            // read/writeable Texture2D.
            // https://stackoverflow.com/a/44734346
            RenderTexture renderTex = RenderTexture.GetTemporary(
                tex.width,
                tex.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.sRGB
            );

            Graphics.Blit(tex, renderTex);

            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;

            Texture2D readableText = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, tex.mipmapCount, false);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();

            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);

            return readableText;
        }

        private static Material GetMaterialWithName(Renderer renderer, string name)
        {
            Material mat = null;
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                if (renderer.materials[i].name.Contains(name))
                {
                    mat = renderer.materials[i];
                    break;
                }
            }
            return mat;
        }
    
        private static Color GetColorFromString(string str)
        {
            Color col = Color.white;
            string colstr = str.StartsWith("#") ? str : "#" + str;
            ColorUtility.TryParseHtmlString(colstr, out col);
            return col;
        }

        public static void PrintRewiredConfiguration()
        {
            CustomizerPlugin.Logger.LogInfo("Rewired Configuration:");
            UserData inputUserData = ReInput.rewiredInputManager.userData;
            Dictionary<int, string> actionNames = new Dictionary<int, string>();
            Dictionary<int, string> categoryNames = new Dictionary<int, string>();
            Dictionary<int, string> mapNames = new Dictionary<int, string>();
            Dictionary<int, string> mouseControls = new Dictionary<int, string>
            {
                { 0, "Mouse Move X" },
                { 1, "Mouse Move Y" },
                { 2, "Mouse Wheel Scroll" },
                { 3, "Left Click" },
                { 4, "Right Click" },
                { 5, "UNKNOWN" },
                { 6, "UNKNOWN" },
            };
            Dictionary<int, string> joystickControls = new Dictionary<int, string>
            {
                { 0, "Left Stick X" },
                { 1, "Left Stick Y" },
                { 2, "Right Stick X" },
                { 3, "Right Stick Y" },
                { 4, "A" },
                { 5, "B" },
                { 6, "UNKNOWN" },
                { 7, "X" },
                { 8, "Y" },
                { 9, "UNKNOWN" },
                { 10, "Left Shoulder" },
                { 11, "Left Trigger" },
                { 12, "Right Shoulder" },
                { 13, "Right Trigger" },
                { 14, "Select" },
                { 15, "Start" },
                { 16, "UNKNOWN" },
                { 17, "UNKNOWN" },
                { 18, "UNKNOWN" },
                { 19, "D-Pad Up" },
                { 20, "D-Pad Right" },
                { 21, "D-Pad Down" },
                { 22, "D-Pad Left" },
                { 23, "UNKNOWN" },
                { 24, "UNKNOWN" },
                { 25, "UNKNOWN" },
                { 26, "UNKNOWN" },
            };

            CustomizerPlugin.Logger.LogInfo("\tInput Actions:");
            foreach (var action in inputUserData.actions)
            {
                CustomizerPlugin.Logger.LogInfo($"\t\t{action.id} : {action.name} ({action.descriptiveName})");
                actionNames.Add(action.id, action.name);
            }

            CustomizerPlugin.Logger.LogInfo("\tInput Categories:");
            foreach (var category in inputUserData.actionCategories)
            {
                CustomizerPlugin.Logger.LogInfo($"\t\t{category.id} : {category.name} ({category.descriptiveName})");
                categoryNames.Add(category.id, category.name);
            }

            CustomizerPlugin.Logger.LogInfo("\tInput Action Category Map:");
            foreach (var category in inputUserData.actionCategoryMap.list)
            {
                int categoryId = category.categoryId;
                CustomizerPlugin.Logger.LogInfo($"\t\t{categoryNames[categoryId]}:");
                foreach (var action in category.ActionIds)
                {
                    CustomizerPlugin.Logger.LogInfo($"\t\t\t{action} : {actionNames[action]}");
                }
            }

            CustomizerPlugin.Logger.LogInfo("\tInput Map Categories:");
            foreach (var category in inputUserData.mapCategories)
            {
                CustomizerPlugin.Logger.LogInfo($"\t\t{category.id} : {category.name} ({category.descriptiveName})");
                mapNames.Add(category.id, category.name);
            }

            CustomizerPlugin.Logger.LogInfo("\tInput Behaviors:");
            foreach (var behavior in inputUserData.inputBehaviors)
            {
                CustomizerPlugin.Logger.LogInfo($"\t\t{behavior.id} : {behavior.name}");
            }

            CustomizerPlugin.Logger.LogInfo("\tKeyboard Maps:");
            foreach (var map in inputUserData.keyboardMaps)
            {
                CustomizerPlugin.Logger.LogInfo($"\t\t{map.id} : Category {map.categoryId} ({mapNames[map.categoryId]}) : Layout {map.layoutId}");
                foreach (var actionElementMap in map.ActionElementMaps)
                {
                    string an = actionNames.ContainsKey(actionElementMap.actionId) ? actionNames[actionElementMap.actionId] : "None";
                    string dir = actionElementMap.axisContribution == Pole.Positive ? "Pos" : "Neg";
                    CustomizerPlugin.Logger.LogInfo($"\t\t\t{actionElementMap.id} : {an} ({dir}) : {actionElementMap.keyboardKeyCode}");
                }
            }

            CustomizerPlugin.Logger.LogInfo("\tMouse Maps:");
            foreach (var map in inputUserData.mouseMaps)
            {
                CustomizerPlugin.Logger.LogInfo($"\t\t{map.id} : Category {map.categoryId} ({mapNames[map.categoryId]}) : Layout {map.layoutId}");
                foreach (var actionElementMap in map.ActionElementMaps)
                {
                    string an = actionNames.ContainsKey(actionElementMap.actionId) ? actionNames[actionElementMap.actionId] : "None";
                    CustomizerPlugin.Logger.LogInfo($"\t\t\t{actionElementMap.id} : {an} : {mouseControls[actionElementMap.elementIdentifierId]}");
                }
            }

            CustomizerPlugin.Logger.LogInfo("\tController Maps:");
            foreach (var map in inputUserData.joystickMaps)
            {
                CustomizerPlugin.Logger.LogInfo($"\t\t{map.id} : Category {map.categoryId} ({mapNames[map.categoryId]}) : Layout {map.layoutId}");
                foreach (var actionElementMap in map.ActionElementMaps)
                {
                    string an = actionNames.ContainsKey(actionElementMap.actionId) ? actionNames[actionElementMap.actionId] : "None";
                    CustomizerPlugin.Logger.LogInfo($"\t\t\t{actionElementMap.id} : {an} : {joystickControls[actionElementMap.elementIdentifierId]}");
                }
            }
        }

        public static void PrintRewiredConfiguration2()
        {
            // Log assigned Joystick information for all joysticks regardless of whether or not they've been assigned
            CustomizerPlugin.Logger.LogInfo("Rewired found " + ReInput.controllers.joystickCount + " joysticks attached.");
            for (int i = 0; i < ReInput.controllers.joystickCount; i++)
            {
                Joystick j = ReInput.controllers.Joysticks[i];
                CustomizerPlugin.Logger.LogInfo(
                    "[" + i + "] Joystick: " + j.name + "\n" +
                    "Hardware Name: " + j.hardwareName + "\n" +
                    "Is Recognized: " + (j.hardwareTypeGuid != System.Guid.Empty ? "Yes" : "No") + "\n" +
                    "Is Assigned: " + (ReInput.controllers.IsControllerAssigned(j.type, j) ? "Yes" : "No")
                );
            }

            // Log assigned Joystick information for each Player
            foreach (var p in ReInput.players.Players)
            {
                CustomizerPlugin.Logger.LogInfo("PlayerId = " + p.id + " is assigned " + p.controllers.joystickCount + " joysticks.");

                // Log information for each Joystick assigned to this Player
                foreach (var j in p.controllers.Joysticks)
                {
                    CustomizerPlugin.Logger.LogInfo(
                      "Joystick: " + j.name + "\n" +
                      "Is Recognized: " + (j.hardwareTypeGuid != System.Guid.Empty ? "Yes" : "No")
                    );

                    // Log information for each Controller Map for this Joystick
                    foreach (var map in p.controllers.maps.GetMaps(j.type, j.id))
                    {
                        CustomizerPlugin.Logger.LogInfo("Controller Map:\n" +
                            "Category = " +
                            ReInput.mapping.GetMapCategory(map.categoryId).name + "\n" +
                            "Layout = " +
                            ReInput.mapping.GetJoystickLayout(map.layoutId).name + "\n" +
                            "enabled = " + map.enabled
                        );
                        foreach (var aem in map.GetElementMaps())
                        {
                            var action = ReInput.mapping.GetAction(aem.actionId);
                            if (action == null) continue; // invalid Action
                            CustomizerPlugin.Logger.LogInfo("Action \"" + action.name + "\" is bound to \"" +
                                aem.elementIdentifierName + "\""
                            );
                        }
                    }
                }
            }
        }
    }
    public static class PlayerKeyNames
    {
        public const string
            // Texture keys
            helmetMain          = "helmetMain",
            helmetWindowRing    = "helmetWindowRing",
            suitCollar          = "suitCollar",
            helmetAntennaStem   = "helmetAntennaStem",
            suitMain            = "suitMain",
            suitArmsLegs        = "suitArmsLegs",
            suitCuffs           = "suitCuffs",
            suitLapel1          = "suitLapel1",
            suitLapel2          = "suitLapel2",
            suitPocketBottom    = "suitPocketBottom",
            suitPocketTop       = "suitPocketTop",
            
            helmetDepthMain         = "helmetDepthMain",
            helmetDepthWindowRing   = "helmetDepthWindowRing",
            suitDepthCollar         = "suitDepthCollar",
            helmetDepthAntennaStem  = "helmetDepthAntennaStem",
            suitDepthMain           = "suitDepthMain",
            suitDepthArmsLegs       = "suitDepthArmsLegs",
            suitDepthCuffs          = "suitDepthCuffs",
            suitDepthLapel1         = "suitDepthLapel1",
            suitDepthLapel2         = "suitDepthLapel2",
            suitDepthPocketBottom   = "suitDepthPocketBottom",
            suitDepthPocketTop      = "suitDepthPocketTop",

            jetpackTop                      = "jetpackTop",
            jetpackBottom                   = "jetpackBottom",
            jetpackTopBooster1              = "jetpackTopBooster1",
            jetpackTopBooster2              = "jetpackTopBooster2",
            jetpackBottomBooster1           = "jetpackBottomBooster1",
            jetpackBottomBooster2           = "jetpackBottomBooster2",
            jetpackPropellerConnector       = "jetpackPropellerConnector",
            jetpackPropellerConnectorInside = "jetpackPropellerConnectorInside",
            jetpackPipeConnector            = "jetpackPipeConnector",
            jetpackPropellerBlades          = "jetpackPropellerBlades",
            jetpackPropellerShaft           = "jetpackPropellerShaft",
            jetpackPropellerHub             = "jetpackPropellerHub",
            jetpackOxygenMeter              = "jetpackOxygenMeter",
            jetpackOxygenMeterLow           = "jetpackOxygenMeterLow",
            jetpackOxygenMeterOff           = "jetpackOxygenMeterOff",
            jetpackOxygenUpgradeLight       = "jetpackOxygenUpgradeLight",
            jetpackOxygenUpgradeCap         = "jetpackOxygenUpgradeCap",
            jetpackOxygenUpgradeRing1       = "jetpackOxygenUpgradeRing1",
            jetpackOxygenUpgradeRing2       = "jetpackOxygenUpgradeRing2",

            // Shader keys
            suitAntennaOrbInner = "suitAntennaOrbInner",
            suitAntennaOrbOuter = "suitAntennaOrbOuter",
            helmetWindow        = "helmetWindow",
            helmetCheeks        = "helmetCheeks",

            jetpackPipe                 = "jetpackPipe",
            jetpackArrows               = "jetpackArrows",
            jetpackArrowsOff            = "jetpackArrowsOff",
            jetpackBoostMeter           = "jetpackBoostMeter",
            jetpackBoostMeterUpgrade    = "jetpackBoostMeterUpgrade",
            jetpackBoostMeterOverdrive  = "jetpackBoostMeterOverdrive",
            jetpackBoostMeterOff        = "jetpackBoostMeterOff",
            jetpackOxygenUpgradeGlow    = "jetpackOxygenUpgradeGlow";
    }

    public static class ShipKeyNames
    {
        public const string
            // Texture keys
            shipBodyMain        = "shipBodyMain",
            shipBumperRing      = "shipBumperRing",
            shipBumper1         = "shipBumper1",
            shipBumper2         = "shipBumper2",
            shipPortholeRings   = "shipPortholeRings",
            shipPortholeWindows = "shipPortholeWindows",
            shipGrill1          = "shipGrill1",
            shipGrill2          = "shipGrill2",
            shipThrusterOutside = "shipThrusterOutside",
            shipThrusterInside  = "shipThrusterInside",
            shipTopperUpper     = "shipTopperUpper",
            shipTopperLower     = "shipTopperLower",
            shipBottom1         = "shipBottom1",
            shipBottom2         = "shipBottom2",
            shipBottom3         = "shipBottom3",
            shipAntennaStem     = "shipAntennaStem",
            // Shader keys
            shipAntennaBall     = "shipAntennaBall";
    }

    public static class KeybindingNames
    {
        public const string
            openColorMenu       = "openColorMenu",
            rotateModelLeft     = "rotateModelLeft",
            rotateModelRight    = "rotateModelRight";
    }
}

using HarmonyLib;
using UnityEngine;

namespace ColorCustomizer
{
    [HarmonyPatch(typeof(PlayerMovementController))]
    public static class PlayerMovementControllerPatches
    {
        public static Material playerJetpackMaterial = null;
        public static Material jetpackUpArrowMaterial = null;
        public static Material jetpackDownArrowMaterial = null;
        public static Material jetpackTubeMaterial = null;
        public static Material jetpackBoostMeterMaterial = null;
        public static Material jetpackOxygenMeterMaterial = null;
        public static Material jetpackPropellerMaterial = null;
        
        const int jetpackMaterialIndex = 2;
        const int upArrowMaterialIndex = 4;
        const int downArrowMaterialIndex = 5;
        const int tubeMaterialIndex = 1;
        const int boostMaterialIndex = 3;
        const int oxygenMaterialIndex = 0;

        [HarmonyPatch(nameof(PlayerMovementController.Awake))]
        [HarmonyPostfix]
        public static void Awake_Postfix(PlayerMovementController __instance)
        {
            playerJetpackMaterial       = __instance.jetpackMeshRenderer.materials[jetpackMaterialIndex];
            jetpackUpArrowMaterial      = __instance.jetpackMeshRenderer.materials[upArrowMaterialIndex];
            jetpackDownArrowMaterial    = __instance.jetpackMeshRenderer.materials[downArrowMaterialIndex];
            jetpackTubeMaterial         = __instance.jetpackMeshRenderer.materials[tubeMaterialIndex];
            jetpackBoostMeterMaterial   = __instance.jetpackMeshRenderer.materials[boostMaterialIndex];
            jetpackOxygenMeterMaterial  = __instance.jetpackMeshRenderer.materials[oxygenMaterialIndex];
            Transform propeller = __instance.transform.Find("PlayerModelRoot/Armature/core_bone/chest/JetpackModel/PropellerPivot/PropellerModel");
            if (propeller != null)
            {
                jetpackPropellerMaterial = propeller.GetComponent<MeshRenderer>().material;
            }
        }
    }
}

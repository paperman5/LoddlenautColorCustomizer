using HarmonyLib;
using UnityEngine;

namespace ColorCustomizer
{
    [HarmonyPatch(typeof(PlayerMovementController))]
    public static class PlayerMovementControllerPatches
    {
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
            CustomizerMod.playerJetpackMaterial       = __instance.jetpackMeshRenderer.materials[jetpackMaterialIndex];
            CustomizerMod.jetpackUpArrowMaterial      = __instance.jetpackMeshRenderer.materials[upArrowMaterialIndex];
            CustomizerMod.jetpackDownArrowMaterial    = __instance.jetpackMeshRenderer.materials[downArrowMaterialIndex];
            CustomizerMod.jetpackTubeMaterial         = __instance.jetpackMeshRenderer.materials[tubeMaterialIndex];
            CustomizerMod.jetpackBoostMeterMaterial   = __instance.jetpackMeshRenderer.materials[boostMaterialIndex];
            CustomizerMod.jetpackOxygenMeterMaterial  = __instance.jetpackMeshRenderer.materials[oxygenMaterialIndex];
            Transform propeller = __instance.transform.Find("PlayerModelRoot/Armature/core_bone/chest/JetpackModel/PropellerPivot/PropellerModel");
            if (propeller != null)
            {
                CustomizerMod.jetpackPropellerMaterial = propeller.GetComponent<MeshRenderer>().material;
            }
            Transform oxygenCapsule = __instance.transform.Find("PlayerModelRoot/Armature/core_bone/chest/JetpackModel/OxygenSlotsParent/OxygenSlot1/OxygenUpgradeCapsule");
            if (oxygenCapsule != null)
            {
                // shared material here to change all of the oxygen capsules at once
                CustomizerMod.jetpackOxygenUpgradeMaterial = oxygenCapsule.GetComponent<MeshRenderer>().sharedMaterial;
            }
            Transform oxygenSlot = __instance.transform.Find("PlayerModelRoot/Armature/core_bone/chest/JetpackModel/OxygenSlotsParent/OxygenSlot1/oxygen_slot_closed");
            if (oxygenSlot != null)
            {
                // shared material here to change all of the oxygen slots at once
                CustomizerMod.jetpackOxygenUpgradeSlotMaterial = oxygenSlot.GetComponent<MeshRenderer>().sharedMaterial;
            }
        }
    }
}

using HarmonyLib;
using UnityEngine;

namespace CollectionPin.Patchs
{
    [HarmonyPatch(typeof(PlayerData))]
    public static class PlayerDataPatch
    {
        [HarmonyPatch(nameof(PlayerData.SetToolData))]
        [HarmonyPrefix]
        private static void SetToolData(PlayerData __instance, string toolName, ToolItemsData.Data data)
        {
            if (!CollectionPin.ModConfig.DebugMode.Value)
                return;
            if (__instance.Tools.GetData(toolName).IsUnlocked)
                return;
            Debug.Log("Unlock tool: " + toolName);
        }

        [HarmonyPatch("get_nailDamage")]
        [HarmonyPostfix]
        private static void GetNailDamage(ref int __result)
        {
            if (CollectionPin.ModConfig.DebugMode.Value)
                __result = 999;
        }

        [HarmonyPatch(nameof(PlayerData.TakeHealth))]
        [HarmonyPrefix]
        private static void TakeHeart(ref int amount)
        {
            if (CollectionPin.ModConfig.DebugMode.Value)
                amount = 0;
        }
    }
}

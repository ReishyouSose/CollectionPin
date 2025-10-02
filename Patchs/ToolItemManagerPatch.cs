using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace CollectionPin.Patchs
{
    [HarmonyPatch(typeof(ToolItemManager))]
    public static class ToolItemManagerPatch
    {

        [HarmonyPatch(nameof(ToolItemManager.SetEquippedTools))]
        [HarmonyPrefix]
        private static void SetEquippedTools(string crestId, List<string> equippedTools)
        {
            if (string.IsNullOrEmpty(crestId))
                return;
            if (PlayerData.instance.ToolEquips.GetData(crestId).IsUnlocked)
                return;
            Debug.Log("Unlock Crest: " + crestId);
        }
    }
}

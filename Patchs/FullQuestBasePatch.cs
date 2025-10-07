using CollectionPin.Scripts;
using HarmonyLib;
using UnityEngine;

namespace CollectionPin.Patchs
{
    [HarmonyPatch(typeof(FullQuestBase))]
    public static class FullQuestBasePatch
    {
        [HarmonyPatch(nameof(FullQuestBase.BeginQuest))]
        [HarmonyPostfix]
        private static void BeginQuest(FullQuestBase __instance)
        {
            if (ModConfig.Ins.DebugMode.Value)
                Debug.Log("Begin Quest:" + __instance.name);
        }
        [HarmonyPatch(nameof(FullQuestBase.TryEndQuest))]
        [HarmonyPostfix]
        private static void TryEndQuest(FullQuestBase __instance, bool __result)
        {
            if (!ModConfig.Ins.DebugMode.Value || !__result)
                return;
            Debug.Log("Begin Quest:" + __instance.name);
        }
    }
}

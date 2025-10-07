using CollectionPin.Scripts;
using GlobalEnums;
using HarmonyLib;
using UnityEngine;

namespace CollectionPin.Patchs
{
    [HarmonyPatch(typeof(GameMap))]
    public static class GameMapPatch
    {
        private static CollectionPinManager manager = null!;

        [HarmonyPatch(nameof(GameMap.OnAwake))]
        [HarmonyPostfix]
        private static void OnAwake(GameMap __instance, bool __result)
        {
            if (__result)
            {
                manager = new CollectionPinManager();
                manager.Create(__instance);
            }
        }

        [HarmonyPatch(nameof(GameMap.TryOpenQuickMap))]
        [HarmonyPostfix]
        private static void TryOpenQuickMap(GameMap __instance,  bool __result)
        {
            if (__result)
            {
                var zone = __instance.GetCurrentMapZone();
                Debug.Log((zone, manager.zoneToMap[zone]));
                manager.CheckPinActive(zone);
            }
        }

        [HarmonyPatch(nameof(GameMap.WorldMap))]
        [HarmonyPostfix]
        private static void WorldMap()
        {
            manager.CheckPinActive(MapZone.NONE);
        }

        [HarmonyPatch(nameof(GameMap.Update))]
        [HarmonyPostfix]
        private static void Update(GameMap __instance)
        {
            if (!ModConfig.Ins.DebugMode.Value)
                return;
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.P))
            {
                var data = PlayerData.instance.QuestCompletionData.GetData("Destroy Thread Cores");
                Debug.Log($"cpl{data.IsCompleted} amt{data.CompletedCount} ever{data.WasEverCompleted}");
            }
        }
    }
}

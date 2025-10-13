using CollectionPin.Scripts;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace CollectionPin.Patchs
{
    [HarmonyPatch(typeof(MapMarkerMenu))]
    public static class MapMarkerMenuPatch
    {
        [HarmonyPatch(nameof(MapMarkerMenu.Update))]
        [HarmonyPostfix]
        private static void Update(MapMarkerMenu __instance)
        {
            if (!ModConfig.Ins.DebugMode.Value)
                return;
            CollectionPinManager.Ins.HandleInput(__instance.inPlacementMode, __instance.placementBox.transform);
        }

        [HarmonyPatch(nameof(MapMarkerMenu.Close))]
        [HarmonyPostfix]
        private static void Close()
        {
            CollectionPinManager.Ins.HideAllContainers();
        }
        [HarmonyPatch(nameof(MapMarkerMenu.PanMap))]
        [HarmonyTranspiler]
        private static List<CodeInstruction> PanMap_IL(IEnumerable<CodeInstruction> codes)
        {
            var list = codes.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                var code = list[i];
                if (code.opcode != OpCodes.Call)
                    continue;
                if (!code.operand.ToString().Contains("get_unscaledDeltaTime"))
                    continue;
                list.Insert(i + 1, new CodeInstruction(OpCodes.Call,
                    AccessTools.Method(typeof(MapMarkerMenuPatch), nameof(LowSpeed))));
                break;
            }
            return list;
        }

        [HarmonyPatch(nameof(MapMarkerMenu.PanMap))]
        [HarmonyPostfix]
        private static void PanMap_PostFix(MapMarkerMenu __instance, bool __result)
        {
            if (!__result)
                return;
            CollectionPinManager.Ins.CollideContainerPin(__instance.placementBox.transform.position);
        }
        private static float LowSpeed(float value)
        {
            if (ModConfig.Ins.DebugMode.Value && Input.GetKey(KeyCode.Keypad0))
                return value / 5f;
            return value;
        }
    }
}

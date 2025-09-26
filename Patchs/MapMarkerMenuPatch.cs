using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace CollectionPin.Patchs
{
    [HarmonyPatch(typeof(MapMarkerMenu))]
    public static class MapMarkerMenuPatch
    {
        [HarmonyPatch(nameof(MapMarkerMenu.Update))]
        [HarmonyPostfix]
        private static void Update(MapMarkerMenu __instance)
        {
            if (!__instance.inPlacementMode)
                return;
            CollectionPinManager.Ins.HandleInput(__instance.placementBox.transform);
        }
    }
}

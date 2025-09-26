using HarmonyLib;
using UnityEngine;

namespace CollectionPin.Patchs
{
    [HarmonyPatch(typeof(MapPin))]
    public static class MapPinPatch
    {
        [HarmonyPatch("ApplyState")]
        [HarmonyPostfix]
        private static void ApplyState(MapPin __instance, MapPin.PinVisibilityStates state)
        {
        }
    }
}

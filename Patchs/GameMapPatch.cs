using HarmonyLib;

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
    }
}

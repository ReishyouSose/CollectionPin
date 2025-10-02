using HarmonyLib;
using UnityEngine;

namespace CollectionPin.Patchs
{
    [HarmonyPatch(typeof(GameManager))]
    public static class GameManagerPatch
    {
        [HarmonyPatch(nameof(GameManager.AddToScenesVisited))]
        [HarmonyPrefix]
        private static void AddToScenesVisited(GameManager __instance, string scene)
        {
            scene = scene.Trim();
            var list = __instance.playerData.scenesVisited;
            if (string.IsNullOrWhiteSpace(scene))
            {
                return;
            }
            if (list.Contains(scene))
            {
                return;
            }
            Debug.Log("Visit new scene: " + scene);
        }
    }
}

using HarmonyLib;
using UnityEngine;

namespace CollectionPin.Patchs
{
    [HarmonyPatch(typeof(PersistentBoolItem))]
    public static class PersistenItemPatch
    {
        [HarmonyPatch(nameof(PersistentBoolItem.Awake))]
        [HarmonyPostfix]
        private static void Awake(PersistentBoolItem __instance)
        {
            string name = __instance.name;
            if (name.Contains("Heart Piece") || name.Contains("Silk Spool"))
            {
                GameObject obj = __instance.gameObject;
                if (!obj.TryGetComponent<CollectableCheck>(out _))
                {
                    obj.AddComponent<CollectableCheck>();
                }
            }
        }
    }
}

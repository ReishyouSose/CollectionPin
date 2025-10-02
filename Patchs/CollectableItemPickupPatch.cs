using HarmonyLib;
using UnityEngine;

namespace CollectionPin.Patchs
{
    [HarmonyPatch(typeof(CollectableItemPickup))]
    public static class CollectableItemPickupPatch
    {
        [HarmonyPatch(nameof(CollectableItemPickup.EndInteraction))]
        [HarmonyPostfix]
        private static void EndInteraction(CollectableItemPickup __instance)
        {
            if (!__instance.TryGetComponent<PersistentBoolItem>(out var item))
                return;
            var data = item.itemData;
            Debug.Log($"Pickup {data.SceneName} {data.ID}");
            CollectionPinManager.Ins.MatchCollectable(item.itemData);
        }
    }
}

using HarmonyLib;
using UnityEngine;

namespace CollectionPin.Patchs
{
    [HarmonyPatch(typeof(CollectableItem))]
    public static class CollectableItemPatch
    {
        [HarmonyPatch(nameof(CollectableItem.Collect))]
        [HarmonyPostfix]
        private static void Collect(CollectableItem __instance)
        {
            string name = __instance.name;
            //Debug.Log(name);
            string id = name switch
            {
                "Mossberry" => "moss_berry_fruit",
                "Shell Flower" => "Nectar Pickup",
                "Plasmium" => "pustule_set_small (1)",
                _ => string.Empty
            };
            if (string.IsNullOrEmpty(id))
                return;
            var objs = Object.FindObjectsByType<PersistentBoolItem>(FindObjectsSortMode.None);
            foreach (var obj in objs)
            {
                if (obj.ItemData.ID == id)
                {
                    CollectionPinManager.Ins.MatchCollectable(obj.ItemData);
                    break;
                }
            }
        }
    }
}

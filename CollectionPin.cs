using BepInEx;
using HarmonyLib;
using System;

namespace CollectionPin
{
    [BepInPlugin(Guid, "MapCollectionMarks", "1.0.0.0")]
    public class CollectionPin : BaseUnityPlugin
    {
        public const string Guid = "Reits.MapCollectionMarks";
        public void Awake()
        {
            Harmony harmony = new Harmony(Guid);
            harmony.PatchAll();
        }
    }
}

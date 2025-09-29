using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace CollectionPin
{
    [BepInPlugin(Guid, "MapCollectionMarks", "1.0.0.0")]
    public class CollectionPin : BaseUnityPlugin
    {
        internal static ConfigEntry<bool> DebugMode = null!;
        public const string Guid = "Reits.MapCollectionMarks";
        public void Awake()
        {
            DebugMode = Config.Bind(new ConfigDefinition("General", "DebugMode"), false);
            Harmony harmony = new Harmony(Guid);
            harmony.PatchAll();
        }
    }
}

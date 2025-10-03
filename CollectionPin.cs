using BepInEx;
using HarmonyLib;

namespace CollectionPin
{
    [BepInPlugin(Guid, "CollectionPin", Version)]
    public class CollectionPin : BaseUnityPlugin
    {
        internal static ModConfig ModConfig = null!;
        public const string Guid = "Reits.CollectionPin";
        public const string Version = "1.0.1.2";
        public void Awake()
        {
            ModConfig = new ModConfig(Config);
            new Harmony(Guid).PatchAll();
        }
    }
}

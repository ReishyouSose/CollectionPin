using BepInEx;
using HarmonyLib;

namespace CollectionPin.Scripts
{
    [BepInPlugin(Guid, "CollectionPin", Version)]
    public class CollectionPin : BaseUnityPlugin
    {
        public const string Guid = "Reits.CollectionPin";
        public const string Version = "1.0.3.0";
        public void Awake()
        {
            ModConfig.Create(Config);
            new Harmony(Guid).PatchAll();
        }
    }
}

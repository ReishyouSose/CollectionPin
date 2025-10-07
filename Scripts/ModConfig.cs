using BepInEx.Configuration;

namespace CollectionPin.Scripts
{
    public class ModConfig
    {
        internal static ModConfig Ins { get; private set; }
        public ConfigEntry<bool> DebugMode;
        //public ConfigEntry<float> Opacity;
        public ConfigEntry<bool> MapLock;
        public ConfigEntry<bool> AbilityLock;
        public ModConfig(ConfigFile file)
        {
            Ins = this;
            DebugMode = file.Bind("General", "DebugMode", false);
            /*Opacity = file.Bind(new ConfigDefinition("General", "Opacity"), 0f,
                new ConfigDescription("Pin opacity collected\n标记收集后不透明度",
                new AcceptableValueRange<float>(0, 0.5f)));*/
            MapLock = file.Bind("Lock", "Map", true, "Hide when hasn't map\n未获得地图时隐藏标记");
            AbilityLock = file.Bind("Lock", "Ability", true, "Hide when ability is lock\n未获得能力时隐藏");
        }
        public static void Create(ConfigFile file)
        {
            Ins = new ModConfig(file);
        }
    }
}

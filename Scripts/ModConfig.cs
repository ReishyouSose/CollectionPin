using BepInEx.Configuration;
using CollectionPin.Scripts.Enums;
using System;
using System.Collections.Generic;

namespace CollectionPin.Scripts
{
    public class ModConfig
    {
        internal static ModConfig Ins { get; private set; }
        public ConfigEntry<bool> DebugMode;
        //public ConfigEntry<float> Opacity;
        public ConfigEntry<bool> MapLock;
        public ConfigEntry<bool> AbilityLock;
        public Dictionary<PinType, ConfigEntry<bool>> PinsFilter;
        public ModConfig(ConfigFile file)
        {
            Ins = this;
            DebugMode = file.Bind("General", "DebugMode", false);
            /*Opacity = file.Bind(new ConfigDefinition("General", "Opacity"), 0f,
                new ConfigDescription("Pin opacity collected\n标记收集后不透明度",
                new AcceptableValueRange<float>(0, 0.5f)));*/
            MapLock = file.Bind("Lock", "Map", true, "Hide when hasn't map\n未获得地图时隐藏标记");
            AbilityLock = file.Bind("Lock", "Ability", true, "Hide when ability is lock\n未获得能力时隐藏");
            PinsFilter = new Dictionary<PinType, ConfigEntry<bool>>();
            var dict = new Dictionary<string, ConfigEntry<bool>>();
            foreach (var pin in Enum.GetValues(typeof(PinType)))
            {
                PinType pinType = (PinType)pin;
                string key = GetPinConfigKey(pinType);
                if (string.IsNullOrEmpty(key))
                    continue;
                if(!dict.TryGetValue(key, out var entry))
                    entry = file.Bind("PinsFilter", key, true);
                PinsFilter[pinType] = entry;
            }
        }
        private string GetPinConfigKey(PinType pin)
        {
            int type = (int)pin;
            if (InRange(type, PinType.HunterV2, PinType.Spell))
                return "Crest";
            else if (InRange(type, PinType.MossBerry, PinType.SteelSpines))
                return "Quest";
            else if (InRange(type, PinType.MossSoup, PinType.SoulChurch))
                return "Quest";
            else if (InRange(type, PinType.SimpleKey, PinType.Farsight))
                return "KeyItem";
            else if (InRange(type, PinType.BoneScroll, PinType.ArcanaEgg))
                return "Relic";
            else if (pin == PinType.Container)
                return string.Empty;
            else
                return pin.ToString();
        }
        private static bool InRange(int pin, PinType left, PinType right)
            => (int)left <= pin && pin <= (int)right;
        public static void Create(ConfigFile file)
        {
            Ins = new ModConfig(file);
        }
    }
}

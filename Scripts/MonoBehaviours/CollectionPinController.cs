using CollectionPin.Scripts;
using CollectionPin.Scripts.DataStruct;
using CollectionPin.Scripts.Enums;
using CollectionPin.Scripts.MonoBehaviours;
using System;

namespace CollectionPin
{
    public partial class CollectionPinController : PinController
    {
        public AbilityType Ability { get; private set; }
        public string MapUnlock { get; private set; } = string.Empty;
        public int DataIndex { get; private set; }


        public void Initialize(CollectionPinData data, string mapUnlock)
        {
            Pin = (PinType)data.PinType;
            Ability = (AbilityType)data.Ability;
            GetBool = data.GetBool;
            MapUnlock = mapUnlock;
            DataIndex = data.Index;
            ExtraCondition = AnalysisExtra(data.Extra);
            AnalysisData();
            if (ExtraCondition != null)
                return;
            switch (GetBool)
            {
                //碎面甲，见到蚂蚁商人就不显示
                case "Fractured Mask":
                    ExtraCondition = new Func<PlayerData, bool>(pd
                        => !pd.scenesMapped.Contains("Ant_Merchant_left") || pd.SeenAntMerchantDead);
                    break;

                //弧爪，蚂蚁商人死了显示 SeenAntMerchantDead这是另一个pd
                case "Curve Claws":
                    ExtraCondition = new Func<PlayerData, bool>(pd
                        => !pd.scenesMapped.Contains("Ant_Merchant_left") && !pd.SeenAntMerchantDead);
                    break;
            }
        }

        public bool IsMatch(string key, string id) => key == Key && id == ID;

        public override void CheckActive(string map)
        {
            gameObject.SetActive(ShouldShow(map) == true);
        }

        public bool? ShouldShow(string map)
        {
            if (Collected)
                return null;

            if (ModConfig.Ins.PinsFilter.TryGetValue(Pin, out var entry) && !entry.Value)
                return false;

            PlayerData pd = PlayerData.instance;

            if (!string.IsNullOrEmpty(map) && MapUnlock != map)
                return false;

            if (ExtraCondition?.Invoke(pd) == false)
                return false;

            var config = ModConfig.Ins;
            bool isMapUnlock = pd.GetBool(MapUnlock);
            if (Pin == PinType.Map)
            {
                if (!pd.GetBool(ID))
                    return false;
                if (isMapUnlock)
                {
                    Collected = true;
                    return null;
                }
                return true;
            }
            else if (config.MapLock.Value && !isMapUnlock)
                return false;

            switch (Ability)
            {
                case AbilityType.NotAct3:
                    if (pd.act3_wokeUp)
                        return false;
                    break;
                case AbilityType.Act3:
                    if (!pd.act3_wokeUp)
                        return false;
                    break;
                case AbilityType.None:
                    break;
                default:
                    if (config.AbilityLock.Value && !pd.GetBool(Ability.ToString()))
                        return false;
                    break;
            }

            bool isContainerPin = Pin == PinType.ContainerPin;
            if (isContainerPin && !string.IsNullOrEmpty(ID) && !pd.GetBool(ID))
                return false;
            if (CollectedCheck(pd))
            {
                if (!isContainerPin)
                    Collected = true;
                return null;
            }
            return true;
        }
        public override string ToString()
        {
            return $"Pin:{Pin} Ability:{Ability} MapUnlock:{MapUnlock} GetBool:{GetBool} Key: {Key} ID: {ID}";
        }
    }
}
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

        /// <summary>检测额外条件失败时不允许显示</summary>
        public Func<PlayerData, bool>? ExtraCondition;

        public void Initialize(CollectionPinData data, string mapUnlock)
        {
            Pin = (PinType)data.PinType;
            Ability = (AbilityType)data.Ability;
            GetBool = data.GetBool;
            MapUnlock = mapUnlock;
            DataIndex = data.Index;
            AnalysisData();
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
            }
            else if (config.MapLock.Value && !isMapUnlock)
                return false;

            switch (Ability)
            {

                case AbilityType.NotAct3:
                    if(pd.act3_wokeUp)
                        return false;
                    break;
                case AbilityType.Act3:
                    if(!pd.act3_wokeUp)
                        return false;
                    break;
                case AbilityType.None:
                    break;
                default:
                    if (config.AbilityLock.Value && !pd.GetBool(Ability.ToString()))
                        return false;
                    break;
            }

            if (CollectedCheck())
            {
                Collected = true;
                return null;
            }
            return true;
        }
        public override string ToString()
        {
            return $"Pin:{Pin} Ability:{Ability} MapUnlock:{MapUnlock} GetBool:{GetBool}";
        }
    }
}
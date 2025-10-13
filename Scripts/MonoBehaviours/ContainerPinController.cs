using CollectionPin.Scripts.DataStruct;
using CollectionPin.Scripts.Enums;

namespace CollectionPin.Scripts.MonoBehaviours
{
    public class ContainerPinController : PinController
    {
        public bool Act3 { get; private set; }

        public void Initialize(ContainerPinData data)
        {
            Pin = (PinType)data.PinType;
            Act3 = data.Act3;
            GetBool = data.GetBool;
            ExtraCondition = AnalysisExtra(data.Extra);
            AnalysisData();
        }

        public override void CheckActive(string _)
        {
            gameObject.SetActive(ShouldActive());
        }

        public bool ShouldActive()
        {
            if (Collected)
                return false;

            if (ModConfig.Ins.PinsFilter.TryGetValue(Pin, out var entry) && !entry.Value)
                return false;

            var pd = PlayerData.instance;

            if (Act3 && !pd.act3_wokeUp)
                return false;

            if (ExtraCondition?.Invoke(pd) == false)
                return false;

            if (CollectedCheck(pd))
            {
                Collected = true;
                return false;
            }
            return true;
        }
        public override string ToString()
        {
            return $"Pin:{Pin} Act3:{Act3} GetBool:{GetBool}";
        }
    }
}

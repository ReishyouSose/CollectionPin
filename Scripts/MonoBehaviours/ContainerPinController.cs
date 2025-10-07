using CollectionPin.Scripts.DataStruct;
using CollectionPin.Scripts.Enums;
using UnityEngine;

namespace CollectionPin.Scripts.MonoBehaviours
{
    public class ContainerPinController : PinController
    {
        public bool Act3 { get; private set; }
        public GameObject CollectedMarker { get; set; } = null!;

        public void Initialize(ContainerPinData data)
        {
            Pin = (PinType)data.PinType;
            Act3 = data.Act3;
            GetBool = data.GetBool;
            AnalysisData();
        }

        public override void CheckActive(string _)
        {
            if (Pin == PinType.Inv)
                return;
            gameObject.SetActive(true);
            CollectedMarker.SetActive(IsCollected());
        }

        public bool IsCollected()
        {
            if (Collected)
                return true;

            if (CollectedCheck())
            {
                Collected = true;
                return true;
            }
            return false;
        }
        public override string ToString()
        {
            return $"Pin:{Pin} Act3:{Act3} GetBool:{GetBool}";
        }
    }
}

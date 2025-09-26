using UnityEngine;

namespace CollectionPin
{
    public class CollectionLabel : MonoBehaviour
    {
        public int PinType;
        public int Index;
        public string Unlock = string.Empty;
        public void SetInfo(int pinType, int counter, string unlock)
        {
            PinType = pinType;
            Index = counter;
            Unlock = unlock;
        }
    }
}

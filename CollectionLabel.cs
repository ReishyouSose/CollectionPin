using System;
using UnityEngine;

namespace CollectionPin
{
    public class CollectionLabel : MonoBehaviour
    {
        public int PinType;
        public int Index;
        public string MapUnlock = string.Empty;
        public Func<PlayerData, SceneData, bool>? Collected;
        public void SetInfo(string mapUnlock, int pinType, int counter, Func<PlayerData, SceneData, bool>? collected)
        {
            MapUnlock = mapUnlock;
            PinType = pinType;
            Index = counter;
            Collected = collected;
        }
        public void CheckActive()
        {
            var pd = PlayerData.instance;
            if (!pd.GetBool(MapUnlock))
            {
                gameObject.SetActive(false);
                return;
            }
            bool? collected = Collected?.Invoke(pd, SceneData.instance);
            Debug.Log((PinType)PinType + " at " + transform.localPosition + " - " + (collected ?? false));
            if (collected == true)
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
        }
    }
}

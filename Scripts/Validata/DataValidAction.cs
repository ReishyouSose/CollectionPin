using UnityEngine;

namespace CollectionPin.Scripts.Validata
{
    public abstract class DataValidAction : MonoBehaviour
    {
        public abstract void Init(string[] args);
        public abstract bool IsMet();
    }
}

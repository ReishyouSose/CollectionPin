using UnityEngine;

namespace CollectionPin
{
    public class CollectableCheck : MonoBehaviour
    {
        private PersistentBoolItem.PersistentBoolData? Data;
        private CircleCollider2D? Collider;
        private bool IsTriggered;
        public void Start()
        {
            Data = GetComponent<PersistentBoolItem>()?.itemData;
            Collider = GetComponent<CircleCollider2D>();
            if (Data == null || Collider == null)
            {
                Debug.Log("Any null");
                Destroy(this);
            }
        }
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (IsTriggered)
                return;
            if (!other.GetComponent<HeroController>())
                return;
            IsTriggered = true;
            CollectionPinManager.Ins.MatchCollectable(Data!);
            Destroy(this);
        }
    }
}

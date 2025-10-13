using CollectionPin.Scripts.DataStruct;
using System;
using UnityEngine;

namespace CollectionPin.Scripts.MonoBehaviours
{
    public class PinContainer : MonoBehaviour
    {
        public ContainerPinsList Info { get; private set; } = null!;
        public GameObject Container { get; private set; } = null!;
        private bool needRefresh;
        private int activeCount;

        public bool IsShowing
        {
            get => Container.gameObject.activeSelf;
            set => Container.gameObject.SetActive(value);
        }
        public void SetContainer(ContainerPinsList info, GameObject container)
        {
            if (Container != null)
                Destroy(Container);
            Container = container;
            Container.name = "Quest_" + info.Name;
            Container.SetActive(false);
            Info = info;
            foreach (var pin in Info.Pins)
            {
                AddPinToContainer(pin);
            }
        }
        public bool IsAllCollected()
        {
            activeCount = 0;
            foreach (Transform trans in Container.transform)
            {
                var pin = trans.GetComponent<ContainerPinController>();
                pin.CheckActive(string.Empty);
                if (pin.gameObject.activeSelf)
                {
                    activeCount++;
                }
            }
            needRefresh = true;
            return activeCount == 0;
        }
        public void ShowContainer()
        {
            if (IsShowing)
                return;
            Container.transform.localScale = Vector3.one;
            IsShowing = true;
            if (!needRefresh)
                return;
            needRefresh = false;

            int x = -1, y = 0;
            float spacing = 0.85f;

            float borderOffset = 0.25f;

            int amount = CalculateItemsPerRow(activeCount);

            foreach (Transform trans in Container.transform)
            {
                if (!trans.gameObject.activeSelf)
                    continue;
                x++;
                if (x >= amount)
                {
                    x = 0;
                    y++;
                }

                // 在border内部开始布局
                trans.localPosition = new Vector3(
                    (x + 0.5f) * spacing + borderOffset,
                    (-y - 0.5f) * spacing - borderOffset,
                    -0.1f
                );
            }

            var sr = Container.GetComponent<SpriteRenderer>();

            borderOffset *= 2f;
            var size = sr.size = new Vector2(amount * spacing + borderOffset, (y + 1) * spacing + borderOffset);

            var p = transform.localPosition;
            float xOffset = Info.Right ? 0.5f : (-size.x - 0.5f);
            Container.transform.localPosition = new Vector3(p.x + xOffset, p.y + size.y / 2f, p.z);
        }
        public void HideContainer()
        {
            if (!IsShowing)
                return;
            IsShowing = false;
        }
        private void AddPinToContainer(ContainerPinData info)
        {
            CollectionPinManager manager = CollectionPinManager.Ins;
            var pinTemplate = manager.pinTemplate;
            var sprites = manager.sprites;
            GameObject newPin = Instantiate(pinTemplate, Container.transform);
            DestroyImmediate(newPin.GetComponent<MapPin>());

            var pin = newPin.AddComponent<ContainerPinController>();
            pin.Initialize(info);
            int pinType = (int)pin.Pin;
            newPin.name = pin.Pin.ToString();

            var sr = newPin.GetComponent<SpriteRenderer>();
            sr.sprite = sprites[pinType];
        }
        public void OnDestroy()
        {
            if (Container != null)
            {
                Destroy(Container);
            }
        }
        public static int CalculateItemsPerRow(int totalElements, int minColumns = 1, int maxColumns = 10)
        {
            if (totalElements <= 0)
                return minColumns;

            // 计算理论最佳值
            int theoreticalColumns = (int)Math.Ceiling(Math.Sqrt(totalElements));

            // 应用约束
            int result = Math.Max(minColumns, Math.Min(maxColumns, theoreticalColumns));

            // 如果约束后单行放不下所有元素，调整到能放下
            if (result < totalElements && result * (int)Math.Ceiling((double)totalElements / result) < totalElements)
            {
                result = Math.Min(maxColumns, totalElements);
            }

            return result;
        }
    }
}

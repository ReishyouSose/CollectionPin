using CollectionPin.Scripts.DataStruct;
using CollectionPin.Scripts.Enums;
using System;
using UnityEngine;

namespace CollectionPin.Scripts.MonoBehaviours
{
    public class PinContainer : MonoBehaviour
    {
        public ContainerPinsList Info { get; private set; } = null!;
        public GameObject Container { get; private set; } = null!;
        public bool IsShowing
        {
            get => Container.gameObject.activeSelf;
            set => Container.gameObject.SetActive(value);
        }
        public void SetContainer(ContainerPinsList info, GameObject container, SpriteRenderer sr)
        {
            if (Container != null)
                Destroy(Container);
            Container = container;
            Container.name = "Quest_" + info.Name;
            Container.SetActive(false);
            Info = info;
            int x = -1, y = 0;
            int amount = CalculateItemsPerRow(info.Pins.Count);
            bool act3 = PlayerData.instance.act3_wokeUp;
            foreach (var pin in Info.Pins)
            {
                if (pin.Act3 && !act3)
                    continue;
                x++;
                if (x >= amount)
                {
                    x = 0;
                    y++;
                }
                Vector3 pos = new Vector3(x * 0.75f + 0.5f, -y * 0.75f - 0.5f, -0.1f);
                AddPinToContainer(pin, pos);
            }
            sr.drawMode = SpriteDrawMode.Sliced;
            var size = sr.size = new Vector2((amount - 1) * 0.75f + 1f, y * 0.75f + 1f);
            var p = sr.transform.localPosition;
            float xOffset = info.Right ? 0.5f : (-size.x - 0.5f);
            sr.transform.localPosition = new Vector3(p.x + xOffset, p.y + size.y / 2f, p.z);
        }
        public bool IsAllCollected()
        {
            foreach (Transform trans in Container.transform)
            {
                var pin = trans.GetComponent<ContainerPinController>();
                if (pin.Pin != PinType.Inv && !pin.IsCollected())
                    return false;
            }
            return true;
        }
        public void ShowContainer()
        {
            if (IsShowing)
                return;
            IsShowing = true;
            Container.transform.localScale = Vector3.one;
            foreach (Transform trans in Container.transform)
            {
                var pin = trans.GetComponent<ContainerPinController>();
                pin.CheckActive(string.Empty);
            }
        }
        public void HideContainer()
        {
            if (!IsShowing)
                return;
            IsShowing = false;
        }
        private void AddPinToContainer(ContainerPinData info, Vector3 pos)
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

            var trans = newPin.GetComponent<Transform>();
            trans.localPosition = pos;

            GameObject marker = Instantiate(pinTemplate, Container.transform);
            marker.SetActive(false);
            DestroyImmediate(marker.GetComponent<MapPin>());

            var markerPin = marker.AddComponent<ContainerPinController>();
            markerPin.Initialize(new ContainerPinData()
            {
                PinType = (int)PinType.Inv,
            });
            marker.name = "marker";
            var markerSR = marker.GetComponent<SpriteRenderer>();
            markerSR.sprite = sprites[^2];

            var markerTrans = marker.GetComponent<Transform>();
            markerTrans.localPosition = pos;

            pin.CollectedMarker = marker;
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

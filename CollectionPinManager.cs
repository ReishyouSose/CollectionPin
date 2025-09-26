using GlobalEnums;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UObj = UnityEngine.Object;

namespace CollectionPin
{
    public class CollectionPinManager
    {
        internal static CollectionPinManager Ins = null!;
        public Dictionary<string, ZonePinsInfo> zonePins = null!;
        public Transform collectionTransform = null!;
        private GameObject pinTemplate = null!;
        private Sprite[] sprites = null!;
        private GameMap _gameMap = null!;
        private string assetPath = string.Empty;
        private int counter;
        private int Counter => counter++;

        public void Create(GameMap gameMap)
        {
            Ins = this;
            _gameMap = gameMap;

            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string location = Path.GetDirectoryName(executingAssembly.Location);
            assetPath = Path.Combine(location, "assets");

            var map = gameMap.fleaPinParents[0];
            pinTemplate = map.GetChild(0).gameObject;//component Mappin SpriteRender
            var top = map.transform.parent;
            collectionTransform = new GameObject().transform;
            collectionTransform.SetParent(top);

            var bytes = File.ReadAllBytes(Path.Combine(assetPath, "Sprite.png"));
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(bytes);
            sprites = new Sprite[12];
            Vector2 center = Vector2.one / 2f;
            for (int i = 0; i < 12; i++)
            {
                sprites[i] = Sprite.Create(tex, new Rect((i % 6) * 160, (/*totalRow - 1*/1 - (i / 6)) * 160, 160, 160), center);
            }

            ReadJson();
        }

        public void HandleInput(Transform pointer)
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                var pos = pointer.position;
                var v2 = new Vector2(pos.x, pos.y);
                foreach (Transform trans in collectionTransform)
                {
                    var p = trans.position;
                    if (Vector2.Distance(v2, new Vector2(p.x, p.y)) > 0.3f)
                        continue;
                    GameObject obj = trans.gameObject;
                    CollectionLabel label = obj.GetComponent<CollectionLabel>();
                    foreach (var (map, info) in zonePins)
                    {
                        if (map != label.Unlock)
                            continue;
                        for (int i = 0; i < info.Pins.Count; i++)
                        {
                            var pin = info.Pins[i];
                            if (pin.Index != label.Index)
                                continue;
                            info.Pins.RemoveAt(i);
                            UObj.Destroy(trans.gameObject);
                            return;
                        }
                    }
                }
                return;
            }

            bool ctrl = Input.GetKey(KeyCode.LeftControl);
            bool alt = Input.GetKey(KeyCode.LeftAlt);
            if (!ctrl && !alt)
                return;

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                if (ctrl)
                {
                    string json = JsonConvert.SerializeObject(zonePins.Values, Formatting.Indented);
                    File.WriteAllText(Path.Combine(assetPath, "CollectionInfo.json"), json);
                    Debug.Log("Save collection, count " + zonePins.Count);
                }
                else if (alt)
                {
                    foreach (Transform obj in collectionTransform)
                    {
                        UObj.Destroy(obj.gameObject);
                    }
                    ReadJson();
                    Debug.Log("Reload collection");
                }
                return;
            }

            PinType? type = null;
            int alpha = 49;
            if (alt)
                alpha += 6;
            for (int i = 0; i < 6; i++)
            {
                int j = i + alpha;
                if (Input.GetKeyDown((KeyCode)j))
                {
                    type = (PinType)(j - 49);
                    break;
                }
            }

            if (type == null)
                return;

            MapZone mapZone = _gameMap.GetCurrentMapZone();
            var zoneInfo = _gameMap.mapZoneInfo[(int)mapZone];
            foreach (var parent in zoneInfo.Parents)
            {
                if (!parent.IsUnlocked)
                    continue;
                var pos = pointer.position;
                string key = parent.PlayerDataBool;
                if (!zonePins.TryGetValue(key, out ZonePinsInfo info))
                {
                    info = zonePins[key] = new ZonePinsInfo()
                    {
                        MapUnlock = key,
                        Pins = new List<MapPinInfo>()
                    };
                }
                int pinType = (int)type.Value;
                int counter = Counter;
                var local = AddPinToMap(key, pinType, counter, new Vector3(pos.x, pos.y, pinTemplate.transform.position.z), false);
                info.Pins.Add(new MapPinInfo()
                {
                    Type = pinType,
                    X = local.x,
                    Y = local.y,
                    Index = counter,
                });
                break;
            }
        }
        private void ReadJson()
        {
            string json = File.ReadAllText(Path.Combine(assetPath, "CollectionInfo.json"));
            var list = JsonConvert.DeserializeObject<List<ZonePinsInfo>>(json);
            if (list == null)
            {
                Debug.Log("Deserialize pins info failed");
                //return;
            }
            zonePins = list?.ToDictionary(x => x.MapUnlock) ?? new Dictionary<string, ZonePinsInfo>();
            float z = pinTemplate.transform.position.z;
            foreach (var (mapUnlock, zonePin) in zonePins)
            {
                foreach (var pin in zonePin.Pins)
                {
                    int counter = Counter;
                    pin.Index = counter;
                    AddPinToMap(mapUnlock, pin.Type, counter, new Vector3(pin.X, pin.Y, z), true);
                }
            }
        }

        private Vector3 AddPinToMap(string zoneName, int pinType, int counter, Vector3 pos, bool local)
        {
            GameObject newPin = UObj.Instantiate(pinTemplate, collectionTransform);
            var label = newPin.AddComponent<CollectionLabel>();
            label.SetInfo(pinType, counter, zoneName);
            newPin.name = ((PinType)pinType).ToString();
            var sr = newPin.GetComponent<SpriteRenderer>();
            sr.sprite = sprites[pinType];
            var transform = newPin.transform;
            if (local)
                transform.localPosition = pos;
            else
                transform.position = pos;
            newPin.SetActive(true);
            Debug.Log("Add collection pin at " + transform.localPosition);
            return transform.localPosition;
        }
    }
}

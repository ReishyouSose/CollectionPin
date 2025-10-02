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
        private CollectionPinData? copy;
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
            int col = 9, row = 3, total = col * row--;
            sprites = new Sprite[total];
            Vector2 center = Vector2.one / 2f;
            for (int i = 0; i < total; i++)
            {
                sprites[i] = Sprite.Create(tex, new Rect((i % col) * 160, (row - (i / col)) * 160, 160, 160), center);
            }

            ReadJson();
        }
        public void CheckPinActive()
        {
            foreach (Transform trans in collectionTransform)
            {
                trans.GetComponent<CollectionPinController>().CheckActive();
            }
        }
        public void HandleInput(bool placeMode, Transform pointer)
        {
            if (placeMode && Input.GetKeyDown(KeyCode.Delete))
            {
                var pos = pointer.position;
                var v2 = new Vector2(pos.x, pos.y);
                foreach (Transform trans in collectionTransform)
                {
                    var p = trans.position;
                    if (Vector2.Distance(v2, new Vector2(p.x, p.y)) > 0.3f)
                        continue;
                    GameObject obj = trans.gameObject;
                    var data = obj.GetComponent<CollectionPinController>();
                    foreach (var (_, info) in zonePins)
                    {
                        for (int i = 0; i < info.Pins.Count; i++)
                        {
                            var pin = info.Pins[i];
                            if (pin.Index != data.DataIndex)
                                continue;
                            info.Pins.RemoveAt(i);
                            Debug.Log("Delete " + data);
                            UObj.Destroy(trans.gameObject);
                            return;
                        }
                    }
                }
                return;
            }

            bool ctrl = Input.GetKey(KeyCode.LeftControl);
            bool shift = Input.GetKey(KeyCode.LeftShift);
            bool alt = Input.GetKey(KeyCode.LeftAlt);
            if (!ctrl && !shift && !alt)
                return;

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                if (shift)
                {
                    string json = JsonConvert.SerializeObject(zonePins.Values, Formatting.Indented);
                    File.WriteAllText(Path.Combine(assetPath, "CollectionInfo.json"), json);
                    Debug.Log("Save collection, count " + zonePins.Sum(x => x.Value.Pins.Count));
                    return;
                }
                if (alt)
                {
                    foreach (Transform obj in collectionTransform)
                    {
                        UObj.Destroy(obj.gameObject);
                    }
                    ReadJson();
                    Debug.Log("Reload collection");
                    return;
                }
                if (ctrl)
                {
                    var pos = pointer.position;
                    var v2 = new Vector2(pos.x, pos.y);
                    foreach (Transform trans in collectionTransform)
                    {
                        var p = trans.position;
                        if (Vector2.Distance(v2, new Vector2(p.x, p.y)) > 0.3f)
                            continue;
                        var data = trans.GetComponent<CollectionPinController>();
                        copy = GetData(data.DataIndex);
                        if (copy != null)
                            Debug.Log("Copy " + data);
                    }
                    return;
                }
            }

            if (!placeMode)
                return;

            PinType? type = null;
            int alpha = 49;
            for (int i = 0; i < 9; i++)
            {
                int j = i + alpha;
                if (Input.GetKeyDown((KeyCode)j))
                {
                    type = (PinType)(j - 49 + (alt ? 18 : shift ? 9 : 0));
                    break;
                }
            }

            if (type == null)
                return;

            if (int.TryParse(type.ToString(), out alpha))
            {
                Debug.Log(alpha + "not in PinType");
                return;
            }

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
                        Pins = new List<CollectionPinData>()
                    };
                }
                copy ??= new CollectionPinData()
                {
                    PinType = (int)type.Value,
                    Index = Counter
                };
                var local = AddPinToMap(copy, key,
                    new Vector3(pos.x, pos.y, pinTemplate.transform.position.z), false, true);
                copy.X = local.x;
                copy.Y = local.y;
                info.Pins.Add(copy);
                copy = null;
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
                    pin.Index = Counter;
                    AddPinToMap(pin, mapUnlock, new Vector3(pin.X, pin.Y, z), true);
                }
            }
        }

        private CollectionPinData? GetData(int index)
        {
            foreach (var (map, info) in zonePins)
            {
                foreach (var pin in info.Pins)
                {
                    if (pin.Index == index)
                        return pin;
                }
            }
            return null;
        }
        private Vector3 AddPinToMap(CollectionPinData info, string mapUnlock, Vector3 pos, bool local, bool log = false)
        {
            GameObject newPin = UObj.Instantiate(pinTemplate, collectionTransform);
            var pin = newPin.AddComponent<CollectionPinController>();
            pin.Initialize(info, mapUnlock);
            int pinType = (int)pin.Pin;
            newPin.name = ((PinType)pinType).ToString();
            var sr = newPin.GetComponent<SpriteRenderer>();
            sr.sprite = sprites[pinType];
            var transform = newPin.transform;
            if (local)
                transform.localPosition = pos;
            else
                transform.position = pos;
            newPin.SetActive(true);
            if (log)
                Debug.Log($"{pin} at {transform.localPosition}");
            return transform.localPosition;
        }
        public void MatchCollectable(PersistentItemData<bool> data)
        {
            string key = data.SceneName;
            string id = data.ID;
            Debug.Log($"Try match key: {key} id: {id}");
            foreach (Transform trans in collectionTransform)
            {
                var pin = trans.GetComponent<CollectionPinController>();
                if (!pin.IsMatch(key, id))
                    continue;
                pin.Collected = true;
                Debug.Log("Match success");
                break;
            }
        }
        public CollectionPinController GetClosetPin()
        {
            float dis = -1;
            GameObject obj = null!;
            Vector2 pos = V3toV2(_gameMap.compassIcon.transform.position);
            foreach (Transform trans in collectionTransform)
            {
                float distance = Vector2.Distance(trans.position, pos);
                if (dis < 0 || distance < dis)
                {
                    dis = distance;
                    obj = trans.gameObject;
                }
            }
            return obj.GetComponent<CollectionPinController>();
        }
        public static Vector2 V3toV2(Vector3 v) => new Vector2(v.x, v.y);
    }
}

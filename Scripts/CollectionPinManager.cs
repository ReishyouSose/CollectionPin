using CollectionPin.Scripts.DataStruct;
using CollectionPin.Scripts.Enums;
using CollectionPin.Scripts.MonoBehaviours;
using GlobalEnums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UObj = UnityEngine.Object;

namespace CollectionPin.Scripts
{
    public class CollectionPinManager
    {
        internal static CollectionPinManager Ins = null!;
        public Dictionary<string, CollectionPinList> collectionPins = null!;
        public Dictionary<string, ContainerPinsList> containerPins = null!;
        public Dictionary<MapZone, string> zoneToMap = null!;
        public Transform collectionTransform = null!;
        public GameObject pinTemplate = null!;
        public Sprite[] sprites = null!;
        public Sprite bg = null!;
        public GameMap _gameMap = null!;
        public string assetPath = string.Empty;
        public int counter;
        public CollectionPinData? copy;
        private bool isPlaceMode;
        private int Counter => counter++;
        private static readonly Func<PlayerData, bool> act3 = new Func<PlayerData, bool>(pd => pd.act3_wokeUp);
        private static readonly Func<PlayerData, bool> notAct3 = new Func<PlayerData, bool>(pd => !pd.act3_wokeUp);

        public void Create(GameMap gameMap)
        {
            Ins = this;
            _gameMap = gameMap;

            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string location = Path.GetDirectoryName(executingAssembly.Location);
            assetPath = Path.Combine(location, "assets");

            var map = gameMap.fleaPinParents[0];
            var top = map.transform.parent;
            collectionTransform = new GameObject().transform;
            collectionTransform.SetParent(top);
            collectionTransform.name = "CollectionPin";
            pinTemplate = map.GetChild(0).gameObject;//component Mappin SpriteRender

            var bytes = File.ReadAllBytes(Path.Combine(assetPath, "Sprite.png"));
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(bytes);
            int col = 9, row = 6, total = col * row--;
            sprites = new Sprite[total];
            Vector2 center = Vector2.one / 2f;
            for (int i = 0; i < total; i++)
            {
                var rect = new Rect(i % col * 180, (row - i / col) * 180, 180, 180);
                sprites[i] = Sprite.Create(tex, rect, center);
            }
            var bgTex = new Texture2D(2, 2);
            bgTex.LoadImage(File.ReadAllBytes(Path.Combine(assetPath, "Container.png")));
            bg = Sprite.Create(bgTex, new Rect(0, 0, 220, 220), new Vector2(0, 1), 100, 0,
                SpriteMeshType.FullRect, new Vector4(20, 20, 20, 20));
            zoneToMap = new Dictionary<MapZone, string>()
            {
                {MapZone.NONE, ""},
                {MapZone.TEST_AREA, ""},
                {MapZone.PATH_OF_BONE, "HasBoneforestMap"},
                {MapZone.GREYMOOR, "HasGreymoorMap"},
                {MapZone.SHELLWOOD_THICKET, "HasShellwoodMap"},
                {MapZone.RED_CORAL_GORGE, "HasCoralMap"},
                {MapZone.CITY_OF_SONG, "HasHallsMap"},
                {MapZone.THE_SLAB, "HasSlabMap"},
                {MapZone.GLOOM, ""},
                {MapZone.DUSTPENS, "HasDustpensMap"},
                {MapZone.BELLTOWN, "HasBellhartMap"},
                {MapZone.HUNTERS_NEST, "HasHuntersNestMap"},
                {MapZone.BONETOWN, "HasMossGrottoMap"},
                {MapZone.MOSS_CAVE, "HasMossGrottoMap"},
                {MapZone.PHARLOOM_BAY, ""},
                {MapZone.DOCKS, "HasDocksMap"},
                {MapZone.WILDS, "HasWildsMap"},
                {MapZone.WEAVER_SHRINE, "HasWeavehomeMap"},
                {MapZone.BONECHURCH, "HasMossGrottoMap"},
                {MapZone.MOSSTOWN, "HasMossGrottoMap"},
                {MapZone.LIBRARY, "HasLibraryMap"},
                {MapZone.CLOVER, "HasCloverMap"},
                {MapZone.UNDERSTORE, "HasCitadelUnderstoreMap"},
                {MapZone.COG_CORE, "HasCogMap"},
                {MapZone.PEAK, "HasPeakMap"},
                {MapZone.DUST_MAZE, ""},
                {MapZone.WARD, "HasWardMap"},
                {MapZone.HANG, "HasHangMap"},
                {MapZone.ARBORIUM, "HasArboriumMap"},
                {MapZone.CRADLE, "HasCradleMap"},
                {MapZone.PILGRIMS_REST, ""},
                {MapZone.HALFWAY_HOUSE, "HasGreymoorMap"},
                {MapZone.JUDGE_STEPS, "HasJudgeStepsMap"},
                {MapZone.MEMORY, ""},
                {MapZone.CRAWLSPACE, "HasCrawlMap"},
                {MapZone.WISP, "visitedWisp"},
                {MapZone.SWAMP, "HasSwampMap"},
                {MapZone.ABYSS, "HasAbyssMap"},
                {MapZone.AQUEDUCT, "HasAqueductMap"},
                {MapZone.SURFACE, ""},
                {MapZone.FRONT_GATE, "HasSongGateMap"},
                {MapZone.CORAL_CAVERNS,""}
            };
            ReadJson();
        }
        public void CheckPinActive(MapZone zone)
        {
            zoneToMap.TryGetValue(zone, out var map);
            foreach (Transform trans in collectionTransform)
            {
                if (trans.TryGetComponent<CollectionPinController>(out var pin))
                    pin.CheckActive(map);
                if (trans.TryGetComponent<PinContainer>(out var container))
                    container.Container.SetActive(false);
            }
        }
        public void HandleInput(bool placeMode, Transform pointer)
        {
            if (isPlaceMode != placeMode)
            {
                isPlaceMode = placeMode;
                if (!isPlaceMode)
                {
                    foreach (Transform trans in collectionTransform)
                    {
                        if (trans.TryGetComponent<PinContainer>(out var pin))
                            pin.HideContainer();
                    }
                }
            }
            if (placeMode && Input.GetKeyDown(KeyCode.Delete))
            {
                var pos = pointer.position;
                var v2 = new Vector2(pos.x, pos.y);
                foreach (Transform trans in collectionTransform)
                {
                    if (!trans.TryGetComponent<CollectionPinController>(out var data))
                        continue;
                    var p = trans.position;
                    if (Vector2.Distance(v2, new Vector2(p.x, p.y)) > 0.3f)
                        continue;
                    foreach (var (_, info) in collectionPins)
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
                    string json = JsonConvert.SerializeObject(collectionPins.Values, Formatting.Indented);
                    File.WriteAllText(Path.Combine(assetPath, "CollectionInfo.json"), json);
                    Debug.Log("Save collection, count " + collectionPins.Sum(x => x.Value.Pins.Count));
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
                        if (!trans.TryGetComponent<CollectionPinController>(out var data))
                            continue;
                        var p = trans.position;
                        if (Vector2.Distance(v2, new Vector2(p.x, p.y)) > 0.3f)
                            continue;
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
                if (!collectionPins.TryGetValue(key, out CollectionPinList info))
                {
                    info = collectionPins[key] = new CollectionPinList()
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
            var pins = JsonConvert.DeserializeObject<List<CollectionPinList>>(json);
            if (pins == null)
            {
                Debug.Log("Deserialize collection pins info failed");
                //return;
            }
            collectionPins = pins?.ToDictionary(x => x.MapUnlock) ?? new Dictionary<string, CollectionPinList>();
            json = File.ReadAllText(Path.Combine(assetPath, "ContainerInfo.json"));
            var contaienrs = JsonConvert.DeserializeObject<List<ContainerPinsList>>(json);
            if (contaienrs == null)
            {
                Debug.Log("Deserialize container pins info failed");
                //return;
            }
            containerPins = contaienrs?.ToDictionary(x => x.Name) ?? new Dictionary<string, ContainerPinsList>();
            float z = pinTemplate.transform.position.z;
            foreach (var (mapUnlock, zonePin) in collectionPins)
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
            foreach (var (map, info) in collectionPins)
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
            UObj.DestroyImmediate(newPin.GetComponent<MapPin>());

            var pin = newPin.AddComponent<CollectionPinController>();
            pin.Initialize(info, mapUnlock);
            int pinType = (int)pin.Pin;
            newPin.name = pin.Pin.ToString();

            var sr = newPin.GetComponent<SpriteRenderer>();
            sr.sprite = sprites[pinType];

            var transform = newPin.transform;
            if (local)
                transform.localPosition = pos;
            else
                transform.position = pos;

            if (pin.Pin == PinType.ContainerPin)
            {
                string[] keyAndID = pin.GetBool.Split('|');
                string key = pin.Key = keyAndID[0];
                if (keyAndID.Length > 1)
                    pin.ID = keyAndID[1];
                if (containerPins.TryGetValue(key, out var data))
                {
                    var containerPin = newPin.AddComponent<PinContainer>();
                    GameObject container = UObj.Instantiate(pinTemplate, collectionTransform);
                    UObj.DestroyImmediate(container.GetComponent<MapPin>());

                    var controller = container.AddComponent<CollectionPinController>();
                    controller.Initialize(new CollectionPinData()
                    {
                        PinType = (int)PinType.Container,
                    }, string.Empty);

                    var containerSR = container.GetComponent<SpriteRenderer>();
                    containerSR.sprite = bg;
                    containerSR.drawMode = SpriteDrawMode.Sliced;

                    container.transform.localPosition = new Vector3(pos.x, pos.y, pos.z);
                    containerPin.SetContainer(data, container);

                    if (data.Hide)
                        UObj.Destroy(sr);
                }
                else
                    Debug.Log("Not found quest data " + key);
            }
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
                if (!trans.TryGetComponent<CollectionPinController>(out var pin))
                    continue;
                if (!pin.IsMatch(key, id))
                    continue;
                pin.Collected = true;
                Debug.Log("Match success");
                break;
            }
        }
        public void CollideContainerPin(Vector2 pos)
        {
            foreach (Transform trans in collectionTransform)
            {
                if (!trans.gameObject.activeInHierarchy)
                    continue;
                var container = trans.GetComponent<PinContainer>();
                if (!container)
                    continue;
                bool canShow = false;
                if (Vector2.Distance(trans.position, pos) < 0.3f)
                {
                    if (container.Info.Hide)
                    {
                        bool collide = false;
                        foreach (var pin in UObj.FindObjectsByType<MapPin>(FindObjectsSortMode.None))
                        {
                            if (!pin.name.Contains("Shop") && !pin.name.Contains("Flea"))
                                continue;
                            if (Vector2.Distance(pin.transform.position, pos) < 0.3f)
                            {
                                collide = true;
                                break;
                            }
                        }
                        if (!collide)
                            continue;
                    }
                    canShow = true;
                }
                if (canShow)
                    container.ShowContainer();
                else
                    container.HideContainer();
            }
        }
        public static Vector2 V3toV2(Vector3 v) => new Vector2(v.x, v.y);
    }
}

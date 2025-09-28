using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CollectionPin
{
    public enum PinType
    {
        MaskShard,
        SilkSpool,
        MemoryLocket,
        CraftMetal,
        MossBerry,//苔梅
        PollipHeart,//花芯
        PlasmifiedBlood,//蓝血
        BoneScroll,//骨卷轴
        WeaverEffigy,//编织者雕像
        ChoralCommandment,//圣咏戒律
        RuneHarp,//符文竖琴
        Cylinder,//音筒
    }
    [Serializable]
    public class MapPinInfo : MonoBehaviour
    {
        private enum DataValidType
        {
            None,
            PlayerData,
            SceneData,
            Relic
        }
        public string GetBool { get; set; } = string.Empty;

        [JsonProperty("Type")]
        public int PinType { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        [JsonIgnore]
        public bool Collected;

        [JsonIgnore]
        public string MapUnlock = string.Empty;

        [JsonIgnore]
        public string Key = string.Empty;

        [JsonIgnore]
        public string ID = string.Empty;

        [JsonIgnore]
        public int Index;

        [JsonIgnore]
        public bool FirstLoad;

        [JsonIgnore]
        private DataValidType ValidType;

        public bool IsMatch(string key, string id) => key == Key && id == ID;
        public void CheckActive() => gameObject.SetActive(ShouldActive());
        public bool ShouldActive()
        {
            if (Collected)
                return false;

            PlayerData pd = PlayerData.instance;
            if (!pd.GetBool(MapUnlock))
                return false;

            switch (ValidType)
            {
                case DataValidType.SceneData:
                    var scene = SceneData.instance.persistentBools;
                    if (!scene.TryGetValue(Key, ID, out var data))
                    {
                        Debug.Log($"{Key} {ID} not in scene dict");
                        return true;
                    }
                    if (!data.Value)
                        return true;
                    Debug.Log($"{Key} {ID} collected");
                    Collected = true;
                    return false;
                case DataValidType.PlayerData:
                    if (pd.GetBool(Key))
                    {
                        Collected = true;
                        Debug.Log($"{Key} collected");
                        return false;
                    }
                    break;
                case DataValidType.Relic:
                    if (pd.Relics.GetData(GetBool).IsCollected)
                    {
                        Collected = true;
                        Debug.Log($"{GetBool} collected");
                        return false;
                    }
                    break;
            }
            return true;
        }
        public void AnalysisData(MapPinInfo info)
        {
            MapUnlock = info.MapUnlock;
            GetBool = info.GetBool;
            PinType = info.PinType;
            if (7 <= PinType && PinType <= 11)
            {
                ValidType = DataValidType.Relic;
                return;
            }

            if (GetBool.StartsWith("pd|"))
            {
                ValidType = DataValidType.PlayerData;
                Key = GetBool[3..];
                return;
            }

            if (string.IsNullOrEmpty(GetBool))
            {
                Debug.Log("Empty Getbool");
                return;
            }

            ValidType = DataValidType.SceneData;
            string[] keyAndOverride = GetBool.Split('|');
            Key = keyAndOverride[0];
            ID = keyAndOverride.Length == 2 ? keyAndOverride[1] : PinType switch
            {
                0 => "Heart Piece",
                1 => "Silk Spool",
                2 => "Collectable Item Pickup",
                3 => "Collectable Item Pickup - Tool Metal",
                4 => "moss_berry_fruit",//部分是直接存在玩家存档的bool字段，以StartWith("dp|")来判定
                5 => "Nectar Pickup",
                6 => "pustule_set_small (1)",//需要使用结果的value（bool）来判定
                _ => string.Empty,
                //遗物有专门的数据
            };
            ShouldActive();
        }
    }

    [Serializable]
    public class ZonePinsInfo
    {
        /// <summary>所属的区域地图（要收集/购买以解锁的那个）</summary>
        public string MapUnlock { get; set; } = string.Empty;
        public List<MapPinInfo> Pins = null!;
    }
}

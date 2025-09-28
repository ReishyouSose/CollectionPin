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
    public class MapPinInfo
    {
        public string GetBool { get; set; } = string.Empty;
        public int Type { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        [JsonIgnore]
        public int Index;

        [JsonIgnore]
        public Vector2 Pos => new Vector2(X, Y);
        public Func<PlayerData, SceneData, bool>? CollectedFunc()
        {
            if (7 <= Type && Type <= 11)
            {
                string target = GetBool;
                return new Func<PlayerData, SceneData, bool>((pd, _) => pd.Relics.GetData(target).IsCollected);
            }

            if (GetBool.StartsWith("pd|"))
            {
                string target = GetBool[3..];
                return new Func<PlayerData, SceneData, bool>((pd, _) => pd.GetBool(target));
            }

            if (string.IsNullOrEmpty(GetBool))
                return null;

            string[] keyAndOverride = GetBool.Split('|');
            string key = keyAndOverride[0];
            string id = keyAndOverride.Length == 2 ? keyAndOverride[1] : Type switch
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
            return new Func<PlayerData, SceneData, bool>((_, sd)
                => SceneDataCheck(sd, (PinType)Type, key, id));
        }
        private static bool SceneDataCheck(SceneData sd, PinType pinType, string key, string id)
        {
            if (sd.persistentBools.TryGetValue(key, id, out var data))
            {
                return data.Value;
            }
            Debug.LogWarning(pinType + $" [key: {key} id: {id} ] not in scenedata");
            return false;
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

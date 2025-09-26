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
        CraftMatel,
        MossBerry,//苔梅
        PollipHeart,//花芯
        PlasmifiedBolld,//蓝血
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
        public  Vector2 Pos => new Vector2(X, Y);
    }

    [Serializable]
    public class ZonePinsInfo
    {
        /// <summary>所属的区域地图（要收集/购买以解锁的那个）</summary>
        public string MapUnlock { get; set; } = string.Empty;
        public List<MapPinInfo> Pins = null!;
    }
}

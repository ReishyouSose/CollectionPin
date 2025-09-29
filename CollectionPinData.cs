using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
    public class CollectionPinData
    {
        [JsonProperty("Type")]
        public int PinType { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public bool Act3 { get; set; }
        public string GetBool { get; set; } = string.Empty;

        [JsonIgnore]
        public int Index { get; set; }
    }

    [Serializable]
    public class ZonePinsInfo
    {
        /// <summary>所属的区域地图（要收集/购买以解锁的那个）</summary>
        public string MapUnlock { get; set; } = string.Empty;
        public List<CollectionPinData> Pins = null!;
    }
}

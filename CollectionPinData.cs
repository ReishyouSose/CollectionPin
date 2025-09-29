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
        BoneScroll,//骨卷轴
        WeaverEffigy,//编织者雕像
        ChoralCommandment,//圣咏戒律
        RuneHarp,//符文竖琴
        Cylinder,//音筒
        MossBerry,//苔梅
        PollipHeart,//花芯
        PlasmifiedBlood,//蓝血
        CogHeart,//机心
        SimpleKey,//简易钥匙
        PaleOil,//苍白油,
        RedTool = 18,
        BlueTool,
        YellowTool,
        Skill,//法术
        Ability,//能力
        SilkHeart,//丝之心
        Map,
    }
    public enum AbilityType
    {
        None,
        hasDash,//冲刺
        hasBrolly,//漂浮
        hasWallJump,//蹬墙跳
        hasNeedolin,//梦钉
        hasHarpoonDash,//飞针
        hasDoubleJump,//二段跳
        hasSuperJump//超冲
    }


    [Serializable]
    public class CollectionPinData
    {
        [JsonProperty("Type")]
        public int PinType { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public int Ability { get; set; }
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

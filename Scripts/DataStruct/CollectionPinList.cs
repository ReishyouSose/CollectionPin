using System;
using System.Collections.Generic;

namespace CollectionPin.Scripts.DataStruct
{
    [Serializable]
    public class CollectionPinList
    {
        /// <summary>所属的区域地图（要收集/购买以解锁的那个）</summary>
        public string MapUnlock { get; set; } = string.Empty;
        public List<CollectionPinData> Pins = null!;
    }
}

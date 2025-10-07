using Newtonsoft.Json;
using System;

namespace CollectionPin.Scripts.DataStruct
{
    [Serializable]
    public class CollectionPinData
    {
        [JsonProperty("Type")]
        public int PinType { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public int Ability { get; set; }
        public string GetBool { get; set; } = string.Empty;
        public string? Extra { get; set; }

        [JsonIgnore]
        public int Index { get; set; }
    }
}

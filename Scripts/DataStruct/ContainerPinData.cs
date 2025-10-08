using Newtonsoft.Json;
using System;

namespace CollectionPin.Scripts.DataStruct
{
    [Serializable]
    public class ContainerPinData
    {

        [JsonProperty("Type")]
        public int PinType { get; set; }
        public bool Act3 { get; set; }
        public string GetBool { get; set; } = string.Empty;
        public string? Extra { get; set; }
    }
}

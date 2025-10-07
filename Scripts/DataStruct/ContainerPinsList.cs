using System;
using System.Collections.Generic;

namespace CollectionPin.Scripts.DataStruct
{
    [Serializable]
    public class ContainerPinsList
    {
        public string Name { get; set; } = string.Empty;
        public bool Hide { get; set; }
        public bool Right { get; set; }
        public List<ContainerPinData> Pins { get; set; } = null!;
    }
}

using UnityEngine;

namespace CollectionPin
{
    public class CollectionPinController : MonoBehaviour
    {
        private enum DataValidType
        {
            None,
            PlayerData,
            SceneData,
            Relic
        }

        public PinType Type { get; private set; }
        public string GetBool { get; private set; } = string.Empty;
        public string MapUnlock { get; private set; } = string.Empty;
        public int DataIndex { get; private set; }
        public bool Act3 { get; private set; }
        public bool Collected { get; set; }
        public string Key { get; private set; } = string.Empty;
        public string ID { get; private set; } = string.Empty;

        private DataValidType validType;

        public void Initialize(CollectionPinData data, string mapUnlock)
        {
            // 直接提取数据字段
            Type = (PinType)data.PinType;
            Act3 = data.Act3;
            GetBool = data.GetBool;
            MapUnlock = mapUnlock;
            DataIndex = data.Index;

            AnalysisData();
            ShouldActive();
        }

        public bool IsMatch(string key, string id) => key == Key && id == ID;

        public void CheckActive() => gameObject.SetActive(ShouldActive());

        public bool ShouldActive()
        {
            if (Collected)
                return false;

            PlayerData pd = PlayerData.instance;

            if (Act3 && !pd.hasSuperJump)
                return false;

            if (!pd.GetBool(MapUnlock))
                return false;

            switch (validType)
            {
                case DataValidType.SceneData:
                    var scene = SceneData.instance.persistentBools;
                    if (!scene.TryGetValue(Key, ID, out var data))
                        return true;
                    if (!data.Value)
                        return true;
                    Collected = true;
                    return false;
                case DataValidType.PlayerData:
                    if (pd.GetBool(Key))
                    {
                        Collected = true;
                        return false;
                    }
                    break;
                case DataValidType.Relic:
                    if (pd.Relics.GetData(GetBool).IsCollected)
                    {
                        Collected = true;
                        return false;
                    }
                    break;
            }
            return true;
        }

        private void AnalysisData()
        {
            if (7 <= (int)Type && (int)Type <= 11)
            {
                validType = DataValidType.Relic;
                return;
            }

            if (GetBool.StartsWith("pd|"))
            {
                validType = DataValidType.PlayerData;
                Key = GetBool[3..];
                return;
            }

            if (string.IsNullOrEmpty(GetBool))
            {
                Debug.Log("Empty Getbool");
                return;
            }

            validType = DataValidType.SceneData;
            string[] keyAndOverride = GetBool.Split('|');
            Key = keyAndOverride[0];
            ID = keyAndOverride.Length == 2 ? keyAndOverride[1] : Type switch
            {
                PinType.MaskShard => "Heart Piece",
                PinType.SilkSpool => "Silk Spool",
                PinType.MemoryLocket => "Collectable Item Pickup",
                PinType.CraftMetal => "Collectable Item Pickup - Tool Metal",
                PinType.MossBerry => "moss_berry_fruit",
                PinType.PollipHeart => "Nectar Pickup",
                PinType.PlasmifiedBlood => "pustule_set_small (1)",
                _ => string.Empty,
            };
        }
        public override string ToString()
        {
            return $"{Type} {MapUnlock} {GetBool}";
        }
    }
}
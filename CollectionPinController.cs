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

        public PinType Pin { get; private set; }
        public AbilityType Ability { get; private set; }
        public string MapUnlock { get; private set; } = string.Empty;
        public string GetBool { get; private set; } = string.Empty;
        public string Key { get; private set; } = string.Empty;
        public string ID { get; private set; } = string.Empty;
        public int DataIndex { get; private set; }
        public bool Collected { get; set; }

        private DataValidType validType;

        public void Initialize(CollectionPinData data, string mapUnlock)
        {
            // 直接提取数据字段
            Pin = (PinType)data.PinType;
            Ability = (AbilityType)data.Ability;
            GetBool = data.GetBool;
            MapUnlock = mapUnlock;
            DataIndex = data.Index;

            AnalysisData();
            ShouldActive();
        }

        public bool IsMatch(string key, string id) => key == Key && id == ID;

        public void CheckActive()
        {
            bool? active = ShouldActive();
            if (active == null)
            {
                float opacity = CollectionPin.ModConfig.Opacity.Value;
                if (opacity > 0)
                {
                    gameObject.SetActive(true);
                    gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, opacity);
                    return;
                }
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(active.Value);
        }

        public bool? ShouldActive()
        {
            if (Collected)
                return null;

            PlayerData pd = PlayerData.instance;

            if (Pin == PinType.Map)
            {
                if (!pd.GetBool(MapUnlock))
                    return true;
                Collected = true;
                return null;
            }

            var config = CollectionPin.ModConfig;
            if (config.MapLock.Value && !pd.GetBool(MapUnlock))
                return false;

            if (config.AbilityLock.Value && Ability != AbilityType.None && !pd.GetBool(Ability.ToString()))
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
                    return null;
                case DataValidType.PlayerData:
                    if (pd.GetBool(Key))
                    {
                        Collected = true;
                        return null;
                    }
                    break;
                case DataValidType.Relic:
                    if (pd.Relics.GetData(GetBool).IsCollected)
                    {
                        Collected = true;
                        return null;
                    }
                    break;
            }
            return true;
        }

        private void AnalysisData()
        {
            if (4 <= (int)Pin && (int)Pin <= 8)
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
            ID = keyAndOverride.Length == 2 ? keyAndOverride[1] : Pin switch
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
            return $"{Pin} {Ability} {MapUnlock} {GetBool}";
        }
    }
}
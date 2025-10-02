using System;
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
            Relic,
            Tool,
            Crest,//纹章
            SceneVisited,
        }

        public PinType Pin { get; private set; }
        public AbilityType Ability { get; private set; }
        public string MapUnlock { get; private set; } = string.Empty;
        public string GetBool { get; private set; } = string.Empty;
        public string Key { get; private set; } = string.Empty;
        public string ID { get; private set; } = string.Empty;
        public int DataIndex { get; private set; }
        public bool Collected { get; set; }

        /// <summary>检测额外条件失败时不允许显示</summary>
        private Func<CollectionPinController, PlayerData, bool>? extraCheck;
        private static Func<CollectionPinController, PlayerData, bool> act3 =
            new Func<CollectionPinController, PlayerData, bool>((_, pd) => pd.GetBool("Act3_wokeUp"));
        private static Func<CollectionPinController, PlayerData, bool> notAct3 =
            new Func<CollectionPinController, PlayerData, bool>((_, pd) => !pd.GetBool("Act3_wokeUp"));

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
            gameObject.SetActive(ShouldActive() == true);
        }

        public bool? ShouldActive()
        {
            if (Collected)
                return null;

            PlayerData pd = PlayerData.instance;

            if (extraCheck?.Invoke(this, pd) == false)
                return false;

            var config = CollectionPin.ModConfig;
            if (Pin != PinType.Map && config.MapLock.Value && !pd.GetBool(MapUnlock))
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
                    if (pd.GetBool(ID))
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
                case DataValidType.Tool:
                    var tools = pd.Tools;
                    var keys = GetBool.Split('|');
                    foreach (var key in keys)
                    {
                        if (tools.GetData(key).IsUnlocked)
                        {
                            Collected = true;
                            return null;
                        }
                    }
                    break;
                case DataValidType.Crest:
                    if (pd.ToolEquips.GetData(ID).IsUnlocked)
                    {
                        Collected = true;
                        return null;
                    }
                    break;
                case DataValidType.SceneVisited:
                    if (pd.scenesVisited.Contains(GetBool))
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
            extraCheck = ExtraCheck();
            switch (Pin)
            {
                case PinType.BoneScroll:
                case PinType.WeaverEffigy:
                case PinType.ChoralCommandment:
                case PinType.RuneHarp:
                case PinType.Cylinder:
                    validType = DataValidType.Relic;
                    return;
                case PinType.RedTool:
                case PinType.BlueTool:
                case PinType.YellowTool:
                case PinType.WhiteTool:
                    validType = DataValidType.Tool;
                    return;
                case PinType.SilkHeart:
                    validType = DataValidType.SceneVisited;
                    return;
            }

            if (string.IsNullOrEmpty(GetBool))
            {
                Debug.Log("Empty Getbool");
                return;
            }

            string[] info = GetBool.Split('|');
            Key = info[0];
            switch (Key)
            {
                case "pd":
                    validType = DataValidType.PlayerData;
                    ID = info[1];
                    break;
                case "cr":
                    validType = DataValidType.Crest;
                    ID = info[1];
                    break;
                default:
                    validType = DataValidType.SceneData;
                    ID = info.Length == 2 ? info[1] : Pin switch
                    {
                        PinType.MaskShard => "Heart Piece",
                        PinType.SilkSpool => "Silk Spool",
                        PinType.CraftMetal => "Collectable Item Pickup - Tool Metal",
                        PinType.MossBerry => "moss_berry_fruit",
                        PinType.PollipHeart => "Nectar Pickup",
                        PinType.PlasmifiedBlood => "pustule_set_small (1)",
                        _ => "Collectable Item Pickup",
                    };
                    break;
            }
        }
        private Func<CollectionPinController, PlayerData, bool>? ExtraCheck()
        {
            return GetBool switch
            {
                "pd|collectedWardKey" => new Func<CollectionPinController, PlayerData, bool>((pin, pd)
                                        => !pd.GetBool("MerchantEnclaveWardKey")),//商人没拿走钥匙时显示
                "Curve Claws" => new Func<CollectionPinController, PlayerData, bool>((pin, pd)
                                        => pd.GetBool("antMerchantKilled")),//蚂蚁商人死了显示
                                                                            //SeenAntMerchantDead这是另一个pd，需要测试
                "pd|bonetownAspidBerryCollected" => Ability == AbilityType.hasDash ? act3 : notAct3,
                "Magnetite Dice" => notAct3,
                "Bone_10|Collectable Item Pickup Locket" => new Func<CollectionPinController, PlayerData, bool>((pin, pd) =>
                {
                    if (pd.QuestCompletionData.GetData("Rock Rollers").IsCompleted)
                        return false;
                    if (!pd.GetBool("Act3_wokeUp"))
                        return false;
                    return true;
                }),
                _ => null,
            };
        }
        public override string ToString()
        {
            return $"Pin:{Pin} Ability:{Ability} MapUnlock:{MapUnlock} GetBool:{GetBool}";
        }
    }
}
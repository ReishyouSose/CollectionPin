using CollectionPin.Scripts.Enums;
using System;
using UnityEngine;
using static CollectionPin.CollectionPinController;

namespace CollectionPin.Scripts.MonoBehaviours
{
    public abstract class PinController : MonoBehaviour
    {
        public PinType Pin { get; protected set; }
        public string GetBool { get; protected set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string ID { get; set; } = string.Empty;
        public bool Collected { get; set; }
        public DataValidType ValidType { get; protected set; }
        /// <summary>检测额外条件失败时不允许显示</summary>
        public Func<PlayerData, bool>? ExtraCondition;
        public abstract void CheckActive(string currectMap);
        public void AnalysisData()
        {
            if (Pin == PinType.Container)
                return;

            if (string.IsNullOrEmpty(GetBool))
            {
                Debug.Log("Empty Getbool");
                return;
            }

            string[] info = GetBool.Split('|');
            Key = info[0];
            switch (Key)
            {
                case "pdb":
                    ValidType = DataValidType.PlayerDataBool;
                    ID = info[1];
                    break;
                case "pdi":
                    ValidType = DataValidType.PlayerDataInt;
                    Key = info[1];
                    ID = info[2];
                    break;
                case "qr":
                    ValidType = DataValidType.QuestReward;
                    ID = info[1];
                    break;
                case "inv":
                    ValidType = DataValidType.Inv;
                    ID = info[1];
                    break;
                default:
                    switch (Pin)
                    {
                        case PinType.BoneScroll:
                        case PinType.WeaverEffigy:
                        case PinType.ChoralCommandment:
                        case PinType.RuneHarp:
                        case PinType.Cylinder:
                        case PinType.ArcanaEgg:
                            ValidType = DataValidType.Relic;
                            return;
                        case PinType.RedTool:
                        case PinType.BlueTool:
                        case PinType.YellowTool:
                            ValidType = DataValidType.Tool;
                            return;
                        case PinType.WebShot:
                            if (ExtraCondition != null)
                                break;
                            ValidType = DataValidType.Tool;
                            return;
                        case PinType.Crest:
                            ValidType = DataValidType.Crest;
                            return;
                        case PinType.SilkHeart:
                            ValidType = DataValidType.SceneVisited;
                            return;
                        case PinType.ContainerPin:
                            ValidType = DataValidType.Container;
                            if (info.Length > 1)
                                ID = info[1];
                            return;
                    }
                    ValidType = DataValidType.SceneData;
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
        public bool CollectedCheck(PlayerData pd)
        {
            switch (ValidType)
            {
                case DataValidType.SceneData:
                    var scene = SceneData.instance.persistentBools;
                    return scene.TryGetValue(Key, ID, out var data) && data.Value;
                case DataValidType.PlayerDataBool:
                    return pd.GetBool(ID);
                case DataValidType.PlayerDataInt:
                    return pd.GetInt(Key) >= int.Parse(ID);
                case DataValidType.Relic:
                    return pd.Relics.GetData(GetBool).IsCollected;
                case DataValidType.Inv:
                    return pd.Collectables.GetData(ID).Amount > 0;
                case DataValidType.Tool:
                    var tools = pd.Tools;
                    var keys = GetBool.Split('|');
                    foreach (var key in keys)
                    {
                        if (tools.GetData(key).IsUnlocked)
                        {
                            return true;
                        }
                    }
                    break;
                case DataValidType.Crest:
                    return pd.ToolEquips.GetData(Key).IsUnlocked;
                case DataValidType.SceneVisited:
                    return pd.scenesVisited.Contains(GetBool);
                case DataValidType.QuestReward:
                    return pd.QuestCompletionData.GetData(ID).IsCompleted;
                case DataValidType.Container:
                    if (!string.IsNullOrEmpty(ID) && !pd.GetBool(ID))
                        return false;
                    return GetComponent<PinContainer>().IsAllCollected();
            }
            return false;
        }
        public Func<PlayerData, bool>? AnalysisExtra(string? Extra)
        {
            if (Extra == null)
                return null;
            string[] infos = Extra.Split('|');
            string key = infos[1];
            switch (infos[0])
            {
                case "pdb":
                    bool target = infos.Length < 3;
                    return new Func<PlayerData, bool>(pd => pd.GetBool(key) == target);
                case "qst":
                    if (!int.TryParse(infos[2], out int value))
                    {
                        Debug.Log("Quest state valid failed");
                        break;
                    }
                    switch (value)
                    {
                        case 0:
                            return new Func<PlayerData, bool>(pd => pd.QuestCompletionData.GetData(key).IsAccepted);
                        case 1:
                            return new Func<PlayerData, bool>(pd => QuestActive(pd.QuestCompletionData.GetData(key)));
                        case 2:
                            return new Func<PlayerData, bool>(pd => pd.QuestCompletionData.GetData(key).IsCompleted);
                        case 3:
                            return new Func<PlayerData, bool>(pd => !pd.QuestCompletionData.GetData(key).IsCompleted);
                    }
                    break;
                case "tool":
                    return new Func<PlayerData, bool>(pd => pd.Tools.GetData(key).IsUnlocked);
                case "inv":
                    if (!int.TryParse(infos[2], out value))
                    {
                        Debug.Log("Inv target valid failed");
                        break;
                    }
                    return new Func<PlayerData, bool>(pd => pd.Collectables.GetData(key).Amount >= value);
            }
            return null;
        }
        public static bool QuestActive(QuestCompletionData.Completion quest)
            => quest.IsAccepted && !quest.IsCompleted;
    }

}

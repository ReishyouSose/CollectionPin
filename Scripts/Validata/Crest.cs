namespace CollectionPin.Scripts.Validata
{
    public class Crest : DataValidAction
    {
        public string CrestName;
        public override void Init(string[] args)
        {
            CrestName = args[0];
        }
        public override bool IsMet() => PlayerData.instance.ToolEquips.GetData(CrestName).IsUnlocked;
    }
}

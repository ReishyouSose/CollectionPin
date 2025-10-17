namespace CollectionPin.Scripts.Validata
{
    public class Relic : DataValidAction
    {
        public string RelicName;
        public override void Init(string[] args)
        {
            RelicName = args[0];
        }
        public override bool IsMet() => PlayerData.instance.Relics.GetData(RelicName).IsCollected;
    }
}

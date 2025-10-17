namespace CollectionPin.Scripts.Validata
{
    public class InInventory : DataValidAction
    {
        public string ItemName;
        public int Target;
        public override void Init(string[] args)
        {
            ItemName = args[0];
            if (args.Length > 1)
                int.TryParse(args[1], out Target);
            else
                Target = 1;
        }
        public override bool IsMet() => PlayerData.instance.Collectables.GetData(ItemName).Amount >= Target;
    }
}

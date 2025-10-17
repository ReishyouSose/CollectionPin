namespace CollectionPin.Scripts.Validata
{
    public class PlayerDataBool : DataValidAction
    {
        public string FiledName;
        public bool Target;
        public override void Init(string[] args)
        {
            FiledName = args[0];
            if(args.Length > 1)
            {
                bool.TryParse(args[1], out Target);
            }
        }

        public override bool IsMet() => PlayerData.instance.GetBool(FiledName) == Target;
    }
}

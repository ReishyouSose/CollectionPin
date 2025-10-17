namespace CollectionPin.Scripts.Validata
{
    public class Tool : DataValidAction
    {
        public string[] Tools;
        public override void Init(string[] args)
        {
            Tools = args;
        }
        public override bool IsMet()
        {
            var t = PlayerData.instance.Tools;
            foreach (var tool in Tools)
            {
                if (t.GetData(tool).IsUnlocked)
                    return true;
            }
            return false;
        }
    }
}

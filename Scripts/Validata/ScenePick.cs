namespace CollectionPin.Scripts.Validata
{
    public class ScenePick : DataValidAction
    {
        public string Key;
        public string ID;
        public override void Init(string[] args)
        {
            Key = args[0];
            ID = args[1];
        }
        public override bool IsMet()
            => SceneData.instance.persistentBools.TryGetValue(Key, ID, out var data) && data.Value;
    }
}

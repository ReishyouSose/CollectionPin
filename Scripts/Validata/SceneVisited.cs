namespace CollectionPin.Scripts.Validata
{
    public class SceneVisited : DataValidAction
    {
        public string SceneName;
        public override void Init(string[] args)
        {
            SceneName = args[0];
        }
        public override bool IsMet() => PlayerData.instance.scenesVisited.Contains(SceneName);
    }
}

namespace CollectionPin.Scripts.Validata
{
    public class QuestState : DataValidAction
    {
        public enum QuestCompleteState
        {
            Accepted,
            Completed,
            NotComplete,
        }
        public string QuestName;
        public QuestCompleteState Target;
        public override void Init(string[] args)
        {
            QuestName = args[0];
            if (args.Length > 1)
            {
                Target = args[1] switch
                {
                    "1" => QuestCompleteState.Completed,
                    "2" => QuestCompleteState.NotComplete,
                    _ => QuestCompleteState.Accepted,
                };
            }
            else
            {
                Target = QuestCompleteState.Completed;
            }
        }
        public override bool IsMet()
        {
            var quest = PlayerData.instance.QuestCompletionData.GetData(QuestName);
            return Target switch
            {
                QuestCompleteState.Accepted => quest.IsAccepted && !quest.IsCompleted,
                QuestCompleteState.Completed => quest.IsCompleted,
                QuestCompleteState.NotComplete => !quest.IsCompleted,
                _ => false,
            };
        }
    }
}

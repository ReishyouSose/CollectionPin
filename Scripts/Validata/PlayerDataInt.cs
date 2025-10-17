using UnityEngine;

namespace CollectionPin.Scripts.Validata
{
    public class PlayerDataInt : DataValidAction
    {
        public enum CompareType
        {
            Equal,
            GreaterThan,
            LessThan,
        }
        public string FiledName;
        public int Target;
        public CompareType Compare;
        public override void Init(string[] args)
        {
            FiledName = args[0];
            if (args.Length > 1)
            {
                switch (args[1][0])
                {
                    case '>':
                        Compare = CompareType.GreaterThan;
                        break;
                    case '<':
                        Compare = CompareType.LessThan;
                        break;
                    case '=':
                        Compare = CompareType.Equal;
                        break;
                    default:
                        Debug.Log(string.Join(", ", args) + "Not valid data in [pd int]");
                        return;
                }
                if (!int.TryParse(args[1][1..], out Target))
                    Debug.Log("parse failed [pd int]");
            }
            else
            {
                Compare = CompareType.GreaterThan;
                Target = 0;
            }
        }

        public override bool IsMet()
        {
            var value = PlayerData.instance.GetInt(FiledName);
            return Compare switch
            {
                CompareType.Equal => value == Target,
                CompareType.GreaterThan => value > Target,
                CompareType.LessThan => value < Target,
                _ => false,
            };
        }
    }
}

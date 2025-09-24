namespace Assets.Feelings.Scripts
{
    public class LoveHateMap : FeelingsMap
    {
        public const string Love = "Love";
        public const string Hate = "Hate";

        public LoveHateMap()
        {
            SetEffect(Love, Hate, -1);
            SetEffect(Hate, Love, -1);
        }
    }
}
namespace Assets.Feelings.Scripts.Sample
{
    public class DishonestMerchangeMap : FeelingsMap
    {
        public const string Happy = "Happy";
        public const string Angry = "Angry";
        public const string Proud = "Proud";
        public const string Trusting = "Trusting";
        public const string Appreciated = "Appreciated";

        public DishonestMerchangeMap()
        {
            SetEffect(Happy, Trusting, 0.5f);
            SetEffect(Happy, Proud, 0.3f);
            SetEffect(Happy, Appreciated, 0.2f);

            SetEffect(Appreciated, Happy, 0.2f);

            SetEffect(Angry, Happy, -2f);
            SetEffect(Angry, Trusting, -2f);
            SetEffect(Angry, Appreciated, -2f);

            SetEffect(Proud, Happy, 0.5f);
            SetEffect(Proud, Trusting, 0.5f);
            SetEffect(Proud, Appreciated, 0.2f);

        }

        public void BuySomething()
        {
            ApplyFeeling(Happy, 0.5f);
        }

        public void Flatter()
        {
            ApplyFeeling(Trusting, -1);
        }

        public void Threaten()
        {
            ApplyFeeling(Angry, 4);
        }

        public void Bribe()
        {
            ApplyFeeling(Proud, 4);
        }

        public float CalcuatePrice(float basePrice)
        {
            var rate = -(GetFeeling(Happy) + GetFeeling(Proud) + GetFeeling(Trusting) + GetFeeling(Appreciated) - GetFeeling(Angry)) / 5 / 100;
            return basePrice * (1 + rate);
        }
    }
}
using System.Collections.Generic;

namespace Assets.Feelings.Scripts
{
    public class ComplexFeelingsMap : FeelingsMap
    {
        public const string Mad = "Mad";
        public const string Hurt = "Hurt";
        public const string Hostile = "Hostile";
        public const string Angry = "Angry";
        public const string Selfish = "Selfish";
        public const string Hateful = "Hateful";
        public const string Critical = "Critical";

        public const string Scared = "Scared";
        public const string Confused = "Confused";
        public const string Rejected = "Rejected";
        public const string Helpless = "Helpless";
        public const string Submissive = "Submissive";
        public const string Insecure = "Insecure";
        public const string Anxious = "Anxious";

        public const string Joyful = "Joyful";
        public const string Excited = "Excited";
        public const string Sensuous = "Sensuous";
        public const string Energetic = "Energetic";
        public const string Cheerful = "Cheerful";
        public const string Creative = "Creative";
        public const string Hopeful = "Hopeful";

        public const string Powerful = "Powerful";
        public const string Faithful = "Faithful";
        public const string Important = "Important";
        public const string Appreciated = "Appreciated";
        public const string Respected = "Respected";
        public const string Proud = "Proud";
        public const string Aware = "Aware";

        public const string Peaceful = "Peaceful";
        public const string Content = "Content";
        public const string Thoughtful = "Thoughtful";
        public const string Intimate = "Intimate";
        public const string Loving = "Loving";
        public const string Trusting = "Trusting";
        public const string Nurturing = "Nurturing";

        public const string Sad = "Sad";
        public const string Guilty = "Guilty";
        public const string Ashamed = "Ashamed";
        public const string Depressed = "Depressed";
        public const string Lonely = "Lonely";
        public const string Bored = "Bored";
        public const string Tired = "Tired";

        private readonly Dictionary<string, string[]> m_feelingToSubfeelings = new Dictionary<string, string[]>
        {
            {"Mad", new[] {Hurt, Hostile, Angry, Selfish, Hateful, Critical}},
            {"Scared", new[] {Confused, Rejected, Helpless, Submissive, Insecure, Anxious}},
            {"Joyful", new[] {Excited, Sensuous, Energetic, Cheerful, Creative, Hopeful}},
            {"Powerful", new[] {Faithful, Important, Appreciated, Respected, Proud, Aware}},
            {"Peaceful", new[] {Content, Thoughtful, Intimate, Loving, Trusting, Nurturing}},
            {"Sad", new[] {Guilty, Ashamed, Depressed, Lonely, Bored, Tired}}
        };

        public ComplexFeelingsMap()
        {
            SetEffect(Mad, Powerful, -1);
            SetEffect(Powerful, Mad, -1);
            SetEffect(Scared, Peaceful, -1);
            SetEffect(Peaceful, Scared, -1);
            SetEffect(Joyful, Sad, -1);
            SetEffect(Sad, Joyful, -1);

            SetEffect(Mad, Sad, 0.3f);
            SetEffect(Mad, Scared, 0.3f);
            SetEffect(Scared, Mad, 0.3f);
            SetEffect(Sad, Mad, 0.3f);

            SetEffect(Powerful, Peaceful, 0.3f);
            SetEffect(Powerful, Joyful, 0.3f);
            SetEffect(Joyful, Powerful, 0.3f);
            SetEffect(Peaceful, Powerful, 0.3f);

            foreach (var feelingToSubfeelings in m_feelingToSubfeelings)
            {
                foreach (var subfeeling in feelingToSubfeelings.Value)
                {
                    SetEffect(feelingToSubfeelings.Key, subfeeling, 0.1f);
                    SetEffect(subfeeling, feelingToSubfeelings.Key, 0.1f);
                }
            }
        }
    }
}
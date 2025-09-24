using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class FeelingsMap
{
    // Maintains the feelings, their current values, and their effects
    private readonly Dictionary<string, Feeling> m_feelings;

    public FeelingsMap()
    {
        m_feelings = new Dictionary<string, Feeling>();
    }

    /// <summary>
    /// Retrieves the current value of the requested feeling
    /// </summary>
    /// <param name="feelingName">The feeling to return</param>
    /// <returns>Current value of the feeling. Value is between -100.0 and 100.0</returns>
    public float GetFeeling(string feelingName)
    {
        Feeling feeling;
        if (!m_feelings.TryGetValue(feelingName, out feeling))
        {
            return 0;
        }
        return feeling.Value;
    }

    /// <summary>
    /// Applies the given value on the requested feeling (and apply all relevant effects on other feelings)
    /// </summary>
    /// <param name="feelingName">The feeling to apply value on</param>
    /// <param name="delta">The value to apply</param>
    /// <returns>New value of the feeling. Value is between -100.0 and 100.0</returns>
    public float ApplyFeeling(string feelingName, float delta)
    {
        return ApplyFeeling(feelingName, delta, new HashSet<string>());
    }

    protected void SetEffect(string feelingNameA, string feelingNameB, float ratio)
    {
        // Retrieve the current value of the feeling, create a new value if non exist
        Feeling feeling;
        if (!m_feelings.TryGetValue(feelingNameA, out feeling))
        {
            feeling = new Feeling { Name = feelingNameA, Effects = new List<Effect>(), Value = 0f };
            m_feelings.Add(feelingNameA, feeling);
        }

        // Update any existing effect, or create a new effect
        var existingEffect = feeling.Effects.FirstOrDefault(e => e.Feeling == feelingNameB);
        if (existingEffect == null)
        {
            feeling.Effects.Add(new Effect { Feeling = feelingNameB, Ratio = ratio });
        }
        else
        {
            existingEffect.Ratio = ratio;
        }
    }

    private float ApplyFeeling(string feelingName, float delta, HashSet<string> handled)
    {
        // Retrieve the current value of the feeling, create a new value if non exist
        Feeling feeling;
        if (!m_feelings.TryGetValue(feelingName, out feeling))
        {
            feeling = new Feeling {Name = feelingName, Effects = new List<Effect>(), Value = 0f};
            m_feelings.Add(feelingName, feeling);
        }

        // Update the feeling's value
        feeling.Value += delta;

        // Mark the feeling as handled
        handled.Add(feelingName);

        // Apply effects as needed
        if (feeling.Effects.Count > 0)
        {
            foreach (var effect in feeling.Effects)
            {
                if (handled.Contains(effect.Feeling))
                {
                    continue;
                }
                ApplyFeeling(effect.Feeling, delta * effect.Ratio, handled);
            }
        }

        return feeling.Value;
    }

    class Effect
    {
        public float Ratio;
        public string Feeling;
    }

    [DebuggerDisplay("{Name}: {Value}")]
    class Feeling
    {
        public string Name;

        private float m_value;
        public float Value
        {
            get { return m_value; }
            set
            {
                var val = value;
                if (val < -100.0f)
                {
                    val = -100.0f;
                }
                else if (val > 100.0f)
                {
                    val = 100.0f;
                }
                m_value = val;
            }
        }

        public List<Effect> Effects;
    }

}
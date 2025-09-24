using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class FeelingsMap
{
    // Maintains the feelings, their current values, and their effects
    private readonly Dictionary<string, Feeling> m_feelings;
    private readonly object m_lock = new object();

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
        lock (m_lock)
        {
            Feeling feeling;
            if (!m_feelings.TryGetValue(feelingName, out feeling))
            {
                return 0;
            }
            return feeling.Value;
        }
    }

    /// <summary>
    /// Applies the given value on the requested feeling (and apply all relevant effects on other feelings)
    /// </summary>
    /// <param name="feelingName">The feeling to apply value on</param>
    /// <param name="delta">The value to apply</param>
    /// <returns>New value of the feeling. Value is between -100.0 and 100.0</returns>
    public float ApplyFeeling(string feelingName, float delta)
    {
        lock (m_lock)
        {
            return ApplyFeelingInternal(feelingName, delta, new HashSet<string>());
        }
    }

    private Feeling GetOrCreateFeeling(string feelingName)
    {
        if (!m_feelings.TryGetValue(feelingName, out var feeling))
        {
            feeling = new Feeling 
            { 
                Name = feelingName, 
                Effects = new List<Effect>(), 
                Value = 0f 
            };
            m_feelings.Add(feelingName, feeling);
        }
        return feeling;
    }

    protected void SetEffect(string feelingNameA, string feelingNameB, float ratio)
    {
        lock (m_lock)
        {
            // Use optimized GetOrCreateFeeling method
            var feeling = GetOrCreateFeeling(feelingNameA);

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
    }

    private float ApplyFeelingInternal(string feelingName, float delta, HashSet<string> handled)
    {
        // Use optimized GetOrCreateFeeling method
        var feeling = GetOrCreateFeeling(feelingName);

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
                ApplyFeelingInternal(effect.Feeling, delta * effect.Ratio, handled);
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
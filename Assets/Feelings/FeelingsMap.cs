using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

/// <summary>
/// Manages interconnected emotions with cascading effects for AI behavior systems.
/// All feeling values are automatically clamped between MIN_FEELING_VALUE and MAX_FEELING_VALUE.
/// </summary>
public class FeelingsMap
{
    /// <summary>
    /// The minimum allowed value for any feeling.
    /// </summary>
    public const float MIN_FEELING_VALUE = -100.0f;
    
    /// <summary>
    /// The maximum allowed value for any feeling.
    /// </summary>
    public const float MAX_FEELING_VALUE = 100.0f;
    
    // Maintains the feelings, their current values, and their effects
    private readonly Dictionary<string, Feeling> m_feelings;
    private readonly object m_lock = new object();

    public FeelingsMap()
    {
        m_feelings = new Dictionary<string, Feeling>();
    }

    /// <summary>
    /// Retrieves the current value of the requested feeling.
    /// </summary>
    /// <param name="feelingName">The name of the feeling to retrieve. Cannot be null or empty.</param>
    /// <returns>Current value of the feeling between MIN_FEELING_VALUE and MAX_FEELING_VALUE, or 0 if feeling doesn't exist.</returns>
    /// <exception cref="ArgumentException">Thrown when feelingName is null, empty, or invalid.</exception>
    public float GetFeeling(string feelingName)
    {
        ValidateFeelingName(feelingName);
        
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
    /// Applies the given delta value to the requested feeling and triggers cascading effects on related feelings.
    /// </summary>
    /// <param name="feelingName">The name of the feeling to modify. Cannot be null or empty.</param>
    /// <param name="delta">The value to add to the feeling. Must be a valid number.</param>
    /// <returns>New value of the feeling between MIN_FEELING_VALUE and MAX_FEELING_VALUE after clamping.</returns>
    /// <exception cref="ArgumentException">Thrown when feelingName is invalid or delta is not a valid number.</exception>
    public float ApplyFeeling(string feelingName, float delta)
    {
        ValidateFeelingName(feelingName);
        ValidateDelta(delta);
        
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

    /// <summary>
    /// Validates that a feeling name is acceptable for use.
    /// </summary>
    /// <param name="feelingName">The feeling name to validate.</param>
    /// <exception cref="ArgumentException">Thrown when feeling name is invalid.</exception>
    private void ValidateFeelingName(string feelingName)
    {
        if (string.IsNullOrEmpty(feelingName))
            throw new ArgumentException("Feeling name cannot be null or empty.", nameof(feelingName));
            
        if (feelingName.Length > 100)
            throw new ArgumentException("Feeling name is too long (maximum 100 characters).", nameof(feelingName));
    }
    
    /// <summary>
    /// Validates that a delta value is acceptable for feeling modifications.
    /// </summary>
    /// <param name="delta">The delta value to validate.</param>
    /// <exception cref="ArgumentException">Thrown when delta is invalid.</exception>
    private void ValidateDelta(float delta)
    {
        if (float.IsNaN(delta) || float.IsInfinity(delta))
            throw new ArgumentException("Delta must be a valid number (not NaN or Infinity).", nameof(delta));
            
        if (Math.Abs(delta) > 1000.0f)
            throw new ArgumentException("Delta magnitude is too large (maximum 1000).", nameof(delta));
    }

    /// <summary>
    /// Configures the effect relationship between two feelings.
    /// </summary>
    /// <param name="feelingNameA">The source feeling that will trigger the effect.</param>
    /// <param name="feelingNameB">The target feeling that will be affected.</param>
    /// <param name="ratio">The multiplier applied to the delta when affecting the target feeling.</param>
    /// <exception cref="ArgumentException">Thrown when feeling names are invalid.</exception>
    protected void SetEffect(string feelingNameA, string feelingNameB, float ratio)
    {
        ValidateFeelingName(feelingNameA);
        ValidateFeelingName(feelingNameB);
        ValidateDelta(ratio);
        
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
                if (val < MIN_FEELING_VALUE)
                {
                    val = MIN_FEELING_VALUE;
                }
                else if (val > MAX_FEELING_VALUE)
                {
                    val = MAX_FEELING_VALUE;
                }
                m_value = val;
            }
        }

        public List<Effect> Effects;
    }

}
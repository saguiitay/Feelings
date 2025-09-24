using System.Collections.Generic;

namespace Assets.Feelings.Scripts
{

    /// <summary>
    /// Interface for reading feeling values from a feelings system.
    /// </summary>
    public interface IFeelingsReader
    {
        /// <summary>
        /// Retrieves the current value of the requested feeling.
        /// </summary>
        /// <param name="feelingName">The name of the feeling to retrieve.</param>
        /// <returns>Current value of the feeling, or 0 if feeling doesn't exist.</returns>
        float GetFeeling(string feelingName);

        /// <summary>
        /// Gets all feeling names currently tracked by this system.
        /// </summary>
        /// <returns>Collection of all feeling names.</returns>
        IEnumerable<string> GetAllFeelingNames();
    }
}
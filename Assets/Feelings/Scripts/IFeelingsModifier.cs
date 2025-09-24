namespace Assets.Feelings.Scripts
{

    /// <summary>
    /// Interface for modifying feeling values in a feelings system.
    /// </summary>
    public interface IFeelingsModifier
    {
        /// <summary>
        /// Applies a delta value to the specified feeling and triggers cascading effects.
        /// </summary>
        /// <param name="feelingName">The name of the feeling to modify.</param>
        /// <param name="delta">The value to add to the feeling.</param>
        /// <returns>New value of the feeling after applying the delta.</returns>
        float ApplyFeeling(string feelingName, float delta);
    }
}
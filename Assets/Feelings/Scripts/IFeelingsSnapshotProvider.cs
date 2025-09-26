using Feelings.Persistence;

namespace Assets.Feelings.Scripts
{
    /// <summary>
    /// Interface for objects that can provide and restore snapshots of their feelings state.
    /// This allows external persistence services to work with feelings without the feelings system 
    /// having direct knowledge of persistence implementations.
    /// </summary>
    public interface IFeelingsSnapshotProvider
    {
        /// <summary>
        /// Creates a snapshot of the current feelings state for serialization.
        /// </summary>
        /// <returns>Serializable data representing current state.</returns>
        FeelingsData CreateSnapshot();
        
        /// <summary>
        /// Restores the feelings state from serialized data.
        /// </summary>
        /// <param name="data">The data to restore from.</param>
        void RestoreFromSnapshot(FeelingsData data);
    }
}
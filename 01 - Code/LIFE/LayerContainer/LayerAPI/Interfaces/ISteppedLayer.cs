using Mono.Addins;

namespace LayerAPI.Interfaces
{
    [TypeExtensionPoint]
    public interface ISteppedLayer : ILayer
    {
        /// <summary>
        /// Advances the layer by one tick. A tick is
        /// the smallest unit of time possible in the current model.
        /// If this layer is a static one, advanceOneTick 
        /// may be an empty implementation which simply 
        /// increases the tickCount.
        /// <pre>initLayer() was called and returned true</pre>
        /// <post>getCurrentTick() returns a by one increased value</post>
        /// </summary>
        void AdvanceOneTick();

        /// <summary>
        /// The current tick this layer is in
        /// </summary>
        /// <returns>Positive long value in active simulation 
        /// or if simulation has ended, -1 otherwise</returns>
        long GetCurrentTick();
    }
}

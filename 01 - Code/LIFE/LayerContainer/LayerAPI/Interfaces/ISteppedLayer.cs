using Mono.Addins;

namespace LayerAPI.Interfaces {
    [TypeExtensionPoint]
    public interface ISteppedLayer : ILayer {
        /// <summary>
        ///     The current Tick this layer is in
        /// </summary>
        /// <returns>
        ///     Positive long value in active simulation
        ///     or if simulation has ended, -1 otherwise
        /// </returns>
        long GetCurrentTick();
    }
}
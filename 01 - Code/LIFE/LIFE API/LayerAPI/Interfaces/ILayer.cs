using System;

namespace LayerAPI.Interfaces {
    public delegate void RegisterAgent(ILayer layer, ITickClient tickClient);

    public delegate void UnregisterAgent(ILayer layer, ITickClient tickClient);

    /// <summary>
    ///     Base Interface for all layers.
    ///     DO NOT IMPLEMENT THIS UNLESS YOU KNOW WHAT YOU'RE DOING!
    ///     Instead implement either ISteppedLayer or one of its decendants depending on your requirements.
    /// </summary>
    public interface ILayer {
        /// <summary>
        ///     Initializes the layer with layerInitData.
        ///     Use this instead of the constructor, as it is
        ///     guaranteed to be called in the correct load order.
        ///     <pre>This layer was successfully added to its container.</pre>
        ///     <post>
        ///         This layer is in a state which allows
        ///         it to start the simulation.
        ///     </post>
        ///     <param name="layerInitData">
        ///         A datatype holding the
        ///         information of how to initialize a layer.
        ///     </param>
        ///     <param name="registerAgentHandle"> </param>
        /// </summary>
        /// <returns>True if init finished successfully, false otherwise</returns>
        Boolean InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle);

        /// <summary>
        ///     The current Tick this layer is in
        /// </summary>
        /// <returns>
        ///     Positive long value in active simulation
        ///     or if simulation has ended, -1 otherwise
        /// </returns>
        long GetCurrentTick();

        /// <summary>
        /// Sets the current Tick of the layer.
        /// This will be called by the RuntimeEnvironment to 
        /// deliver the currenTick into the layer.
        /// </summary>
        /// <param name="currentTick">A positive number</param>
        void SetCurrentTick(long currentTick);
    }
}
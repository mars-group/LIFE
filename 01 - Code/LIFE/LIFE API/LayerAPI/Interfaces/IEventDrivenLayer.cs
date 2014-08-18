using Mono.Addins;

namespace LayerAPI.Interfaces {
    /// <summary>
    ///     An event driven layer. Implement this interface if you want a purely event driven layer
    ///     without and synchronization between two consecutive steps. You may, however, use
    ///     the optional parameter "pseudoTickDuration" in StartLayer() to internally count the
    ///     ticks which are made by other, stepped Layers. This feature may be used to provide some kind of
    ///     un-precise logical time.
    /// </summary>
    [TypeExtensionPoint]
    public interface IEventDrivenLayer : ILayer {
        /// <summary>
        ///     Starts this layer at startTime or
        ///     immediatly if startTime is not specified.
        ///     A pseudo Tick duration may be provided to count a pseudo Tick number
        ///     for advancement tracking. This Tick does NOT control execution!
        /// </summary>
        /// <param name="pseudoTickDuration">Duration of one pseudo Tick, defaults to 0</param>
        void StartLayer(long pseudoTickDuration = 0);

        /// <summary>
        ///     Pauses this layer. Allows to continue execution.
        /// </summary>
        void PauseLayer();

        /// <summary>
        ///     Stops this layer. Caution: It will not be possible to continue exection once stopped!
        /// </summary>
        void StopLayer();

        /// <summary>
        ///     Returns the current status of this layer.
        /// </summary>
        /// <returns>The LayerStatus</returns>
        LayerStatus GetLayerStatus();
    }
}
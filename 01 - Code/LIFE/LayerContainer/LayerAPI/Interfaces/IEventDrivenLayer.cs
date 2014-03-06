
namespace LayerAPI.Interfaces
{
    interface IEventDrivenLayer : ILayer
    {
        /// <summary>
        /// Starts this layer at startTime or
        /// immediatly if startTime is not specified.
        /// A pseudo tick duration may be provided to count a pseudo tick number 
        /// for advancement tracking. This tick does NOT control execution!
        /// </summary>
        /// <param name="startTime">Time at which to start, defaults to now</param>
        /// <param name="pseudoTickDuration">Duration of one pseudo tick, defaults to 0</param>
        void StartLayer(long startTime=0, long pseudoTickDuration=0);

        /// <summary>
        /// Pauses this layer. Allows to continue execution.
        /// </summary>
        void PauseLayer();

        /// <summary>
        /// Stops this layer. Caution: It will not be possible to continue exection once stopped!
        /// </summary>
        void StopLayer();

        /// <summary>
        /// Returns the current status of this layer.
        /// </summary>
        /// <returns>The LayerStatus</returns>
        LayerStatus GetLayerStatus();
    }
}

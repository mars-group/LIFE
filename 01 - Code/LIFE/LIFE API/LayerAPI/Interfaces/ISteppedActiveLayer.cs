namespace LayerAPI.Interfaces
{
    /// <summary>
    /// The ISteppedActiveLayer will get ticked by the LayerContainer, just as the average ITickClient.
    /// In Addition it provides to more methods which allow to hook into the moment just before and after a tick.
    /// </summary>
    public interface ISteppedActiveLayer : ISteppedLayer, ITickClient
    {
        /// <summary>
        /// Gets called just before all agents get ticked
        /// </summary>
        void PreTick();

        /// <summary>
        /// Gets called after all agents get ticked, but before
        /// the next Tick launches.
        /// </summary>
        void PostTick();
    }
}

namespace PedestrianModel.Agents.Reasoning.Pathfinding {

    /// <summary>
    ///     @author Christian Thiel
    /// </summary>
    public enum TargetListType {
        /// <summary>
        ///     Create move plan to the nearest target based on AStar.
        /// </summary>
        Parallel,

        /// <summary>
        ///     Sequentially achieve all target positions and then commit suicide.
        /// </summary>
        Sequential,

        /// <summary>
        ///     Sequentially achieve all target positions and start over.
        /// </summary>
        SequentialLoop
    }

}
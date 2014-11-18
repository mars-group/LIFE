using System.Collections.Generic;

namespace PedestrianModel.Agents.Reasoning.Pathfinding {

    /// <summary>
    ///     @author Christian Thiel
    /// </summary>
    public interface IPathfinder<TE> {
        /// <summary>
        ///     Returns the used search graph.
        /// </summary>
        /// <returns> the search graph </returns>
        ISearchGraph<TE> SearchGraph { get; }

        /// <summary>
        ///     Searches for a path from <code>from</code> to <code>to</code>.
        /// </summary>
        /// <param name="from"> the start node </param>
        /// <param name="to"> the target node </param>
        /// <returns> a list of by the node adapted objects. </returns>
        IList<TE> FindPath(IPathNode<TE> from, IPathNode<TE> to);
    }

}
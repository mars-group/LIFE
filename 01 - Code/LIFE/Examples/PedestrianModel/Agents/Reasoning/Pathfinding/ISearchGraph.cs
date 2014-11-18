using System.Collections.Generic;

namespace PedestrianModel.Agents.Reasoning.Pathfinding {

    /// <summary>
    ///     @author Christian Thiel
    /// </summary>
    public interface ISearchGraph<TE> {
        /// <summary>
        ///     Calculates the heuristic distance between the two nodes <code>from</code> and <code>to</code>. The
        ///     distance describes the cost to reach the target node. The cost to get from a to b must not be equal to
        ///     the cost to get from b to a.
        /// </summary>
        /// <param name="from"> the start node </param>
        /// <param name="to"> the target node </param>
        /// <returns> the estimated cost from start to target </returns>
        double GetHeuristic(IPathNode<TE> from, IPathNode<TE> to);

        /// <summary>
        ///     Returns a collection of all reachable neighbors of the given node.
        /// </summary>
        /// <param name="node"> the node to get the neighbors from </param>
        /// <returns> all neighbors of the given node </returns>
        ICollection<IPathNode<TE>> GetNeighbors(IPathNode<TE> node);

        /// <summary>
        ///     Calculates the distance between two adjacent nodes. The distance describes the cost to reach the target
        ///     node. The cost to get from a to b must not be equal to the cost to get from b to a.
        /// </summary>
        /// <param name="from"> the start node </param>
        /// <param name="to"> the target node </param>
        /// <returns> the distance </returns>
        double Distance(IPathNode<TE> from, IPathNode<TE> to);
    }

}
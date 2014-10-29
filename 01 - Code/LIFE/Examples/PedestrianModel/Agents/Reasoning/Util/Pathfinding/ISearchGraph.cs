using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedestrianModel.Agents.Reasoning.Util.Pathfinding
{

	/// 
	/// <summary>
	/// @author Christian Thiel
	/// </summary>
	/// @param <E> the node type of the search graph </param>
	public interface ISearchGraph<E>
	{

		/// <summary>
		/// Calculates the heuristic distance between the two nodes <code>from</code> and <code>to</code>. The
		/// distance describes the cost to reach the target node. The cost to get from a to b must not be equal to
		/// the cost to get from b to a.
		/// </summary>
		/// <param name="from"> the start node </param>
		/// <param name="to"> the target node </param>
		/// <returns> the estimated cost from start to target </returns>
		/// <exception cref="IllegalArgumentException"> if one of the nodes is not in the graph </exception>
		double GetHeuristic(IPathNode<E> from, IPathNode<E> to);

		/// <summary>
		/// Returns a collection of all reachable neighbors of the given node.
		/// </summary>
		/// <param name="node"> the node to get the neighbors from </param>
		/// <returns> all neighbors of the given node </returns>
		/// <exception cref="IllegalArgumentException"> if the given node is not in the graph </exception>
		ICollection<IPathNode<E>> GetNeighbors(IPathNode<E> node);

		/// <summary>
		/// Calculates the distance between two adjacent nodes. The distance describes the cost to reach the target
		/// node. The cost to get from a to b must not be equal to the cost to get from b to a.
		/// </summary>
		/// <param name="from"> the start node </param>
		/// <param name="to"> the target node </param>
		/// <returns> the distance </returns>
		/// <exception cref="IllegalArgumentException"> if one of the nodes is not in the graph or the nodes are not adjacent </exception>
		double Distance(IPathNode<E> from, IPathNode<E> to);
	}

}
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
	/// @param <E> the underlying node type of the search graph </param>
	public interface IPathfinder<E>
	{

		/// <summary>
		/// Searches for a path from <code>from</code> to <code>to</code>.
		/// </summary>
		/// <param name="from"> the start node </param>
		/// <param name="to"> the target node </param>
		/// <returns> a list of by the node adapted objects. </returns>
		IList<E> FindPath(IPathNode<E> from, IPathNode<E> to);

		/// <summary>
		/// Returns the used search graph.
		/// </summary>
		/// <returns> the search graph </returns>
		ISearchGraph<E> SearchGraph {get;}
	}

}
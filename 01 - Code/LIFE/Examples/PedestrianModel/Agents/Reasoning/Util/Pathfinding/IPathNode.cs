using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace de.haw.walk.agent.util.pathfinding
{

	/// 
	/// <summary>
	/// @author Christian Thiel
	/// </summary>
	/// @param <E> the node type of the underlying search graph </param>
	public interface IPathNode<E>
	{

		/// <summary>
		/// Returns the adapted object of this node.
		/// </summary>
		/// <returns> the adapted object </returns>
		E AdaptedObject {get;}

		/// <summary>
		/// returns the assigned predecessor node of this node. The predecessor node is used to create the entire
		/// path.
		/// </summary>
		/// <returns> the predecessor of this node </returns>
		IPathNode<E> Predecessor {get;set;}


		/// <summary>
		/// Returns true if <code>o</code> is a Node object with equal adapted objects.
		/// </summary>
		/// <param name="o"> the object to compare </param>
		/// <returns> true if the object o is equal to this node </returns>
		bool Equals(object o);

		int GetHashCode();
	}

}
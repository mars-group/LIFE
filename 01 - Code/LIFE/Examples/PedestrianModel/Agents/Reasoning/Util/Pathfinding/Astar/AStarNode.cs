using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace de.haw.walk.agent.util.pathfinding.astar
{

	/// 
	/// <summary>
	/// @author Christian Thiel
	/// </summary>
	/// @param <E> The type of the node used in the search graph </param>
	public class AStarNode<E>
	{

		/// <summary>
		/// The original search graph node.
		/// </summary>
		private readonly IPathNode<E> externalNode;

		/// <summary>
		/// The search graph containing the external node.
		/// </summary>
		private readonly ISearchGraph<E> graph;

		/// <summary>
		/// The predecessor of this AStar node. <code>null</code> if it has none.
		/// </summary>
		private AStarNode<E> predecessor = null;

		/// <summary>
		/// Cached value of the costs from start.
		/// </summary>
		private double? costFromStart = null;

		/// <summary>
		/// Cached value of the predicted costs to goal.
		/// </summary>
		private double? costToGoal = null;

		/// <summary>
		/// Creates a new AStar node.
		/// </summary>
		/// <param name="graphNode"> the graph node </param>
		/// <param name="graph"> the search graph </param>
		public AStarNode(IPathNode<E> graphNode, ISearchGraph<E> graph)
		{
			externalNode = graphNode;
			this.graph = graph;
		}

		/// <summary>
		/// Returns the complete costs from the start value of this node path to this node.
		/// </summary>
		/// <returns> the costFromStart </returns>
		public double CostFromStart
		{
			get
			{
                if (costFromStart == null)
				{
					if (predecessor == null)
					{
						costFromStart = 0d;
					}
					else
					{
						costFromStart = predecessor.CostFromStart + graph.Distance(predecessor.externalNode, externalNode);
					}
				}

                //return costFromStart;
				return costFromStart.Value;
			}
		}

		/// <summary>
		/// Returns the estimated cost to the given goal node.
		/// </summary>
		/// <param name="goalNode"> the goal node </param>
		/// <returns> the costToGoal </returns>
		public double GetCostToGoal(AStarNode<E> goalNode)
		{
            if (costToGoal == null)
			{
				costToGoal = graph.GetHeuristic(externalNode, goalNode.externalNode);
			}
            // return costToGoal;
			return costToGoal.Value;
		}

		/// <summary>
		/// Returns the predecessor of this node.
		/// </summary>
		/// <returns> the predecessor. </returns>
		public AStarNode<E> Predecessor
		{
			get
			{
				return predecessor;
			}
			set
			{
				this.predecessor = value;
				this.externalNode.Predecessor = value.externalNode;
				this.costFromStart = null;
			}
		}


		/// <summary>
		/// Returns a collection of all neighbors of this node based on the underlying search graph.
		/// </summary>
		/// <returns> a collection of all neighbors. </returns>
		public ICollection<AStarNode<E>> Neighbors
		{
			get
			{
				ICollection<AStarNode<E>> result = new HashSet<AStarNode<E>>();
    
				ICollection<IPathNode<E>> neighbors = graph.GetNeighbors(externalNode);
    
				foreach (IPathNode<E> n in neighbors)
				{
					AStarNode<E> nb = new AStarNode<E>(n, graph);
					nb.Predecessor = this;
					result.Add(nb);
				}
    
				return result;
			}
		}

		public override sealed int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			result = prime * result;

			if (!(externalNode == null))
			{
				result += externalNode.GetHashCode();
			}

			return result;
		}

		public override sealed bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (this.GetType() != obj.GetType())
			{
				return false;
			}
			AStarNode<E> other = (AStarNode<E>) obj;
			if (externalNode == null)
			{
				if (other.externalNode != null)
				{
					return false;
				}
			}
			else if (!externalNode.Equals(other.externalNode))
			{
				return false;
			}
			return true;
		}

		public override sealed string ToString()
		{
			return externalNode.ToString();
		}

		/// <summary>
		/// Returns the external node from the search graph.
		/// </summary>
		/// <returns> the externalNode </returns>
		public IPathNode<E> ExternalNode
		{
			get
			{
				return externalNode;
			}
		}

	}

}
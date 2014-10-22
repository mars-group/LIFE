using System.Collections.Generic;

namespace de.haw.walk.agent.util.pathfinding.astar
{

	/// 
	/// <summary>
	/// @author Christian Thiel
	/// </summary>
	/// @param <E> The external node type of the search graph </param>
	public class AStarPathfinder<E> : IPathfinder<E>
	{

		/// <summary>
		/// The search graph to use for the pathfinding.
		/// </summary>
		private readonly ISearchGraph<E> graph;

		/// <summary>
		/// Creates a new AStar pathfinder using the given search graph.
		/// </summary>
		/// <param name="graph"> the graph to use </param>
		public AStarPathfinder(ISearchGraph<E> graph)
		{
			this.graph = graph;
		}

		public IList<E> findPath(IPathNode<E> from, IPathNode<E> to)
		{

			AStarNode<E> fromNode = new AStarNode<E>(from, graph);
			AStarNode<E> toNode = new AStarNode<E>(to, graph);

			SortedSet<AStarNode<E>> openList = new SortedSet<AStarNode<E>>(new ComparatorAnonymousInnerClassHelper(this, toNode));
			Dictionary<IPathNode<E>, AStarNode<E>> openMap = new Dictionary<IPathNode<E>, AStarNode<E>>();
			Dictionary<IPathNode<E>, AStarNode<E>> closedMap = new Dictionary<IPathNode<E>, AStarNode<E>>();

			AStarNode<E> bestNode = null;

			openList.Add(fromNode);

			while (openList.Count > 0)
			{
				bestNode = openList.pollFirst();

				if (toNode.Equals(bestNode))
				{
					return buildPath(bestNode);
				}
				else
				{
					ICollection<AStarNode<E>> neighbors = bestNode.Neighbors;
					foreach (AStarNode<E> newNode in neighbors)
					{
						AStarNode<E> oldVer = null;

						oldVer = openMap[newNode.ExternalNode];

						if (oldVer != null && oldVer.CostFromStart <= newNode.CostFromStart)
						{
							continue;
						}

						oldVer = closedMap[newNode.ExternalNode];

						if (oldVer != null && oldVer.CostFromStart <= newNode.CostFromStart)
						{
							continue;
						}

						closedMap.Remove(newNode);
						openList.Add(newNode);
						openMap[newNode.ExternalNode] = newNode;
					}

				}

				closedMap[bestNode.ExternalNode] = bestNode;
			}

			return null;
		}

		private class ComparatorAnonymousInnerClassHelper : IComparer<AStarNode<E>>
		{
			private readonly AStarPathfinder<E> outerInstance;

			private de.haw.walk.agent.util.pathfinding.astar.AStarNode<E> toNode;

			public ComparatorAnonymousInnerClassHelper(AStarPathfinder<E> outerInstance, de.haw.walk.agent.util.pathfinding.astar.AStarNode<E> toNode)
			{
				this.outerInstance = outerInstance;
				this.toNode = toNode;
			}

			public int Compare(AStarNode<E> o1, AStarNode<E> o2)
			{
				int compare = o1.CostFromStart + o1.getCostToGoal(toNode).CompareTo(o2.CostFromStart + o2.getCostToGoal(toNode));

				if (compare == 0)
				{
					compare = (new int?(o1.GetHashCode())).compareTo(o2.GetHashCode());
				}

				return compare;
			}
		}

		/// <summary>
		/// Returns the path from the given node to the start node of its path.
		/// </summary>
		/// <param name="bestNode"> the end node of the path </param>
		/// <returns> the path to the given node </returns>
		private IList<E> buildPath(AStarNode<E> bestNode)
		{
			IList<E> backtracingPath = new List<E>();
			IList<E> path = new List<E>();

			AStarNode<E> currentNode = bestNode;

			while (currentNode != null)
			{
				backtracingPath.Add(currentNode.ExternalNode.AdaptedObject);
				currentNode = currentNode.Predecessor;
			}

			for (int i = backtracingPath.Count - 2; i >= 0; i--)
			{
				path.Add(backtracingPath[i]);
			}

			return path;
		}

		public ISearchGraph<E> SearchGraph
		{
			get
			{
				return this.graph;
			}
		}

	}

}
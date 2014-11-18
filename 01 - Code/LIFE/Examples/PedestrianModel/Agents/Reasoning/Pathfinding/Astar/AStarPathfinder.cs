using System.Collections.Generic;
using System.Linq;

namespace PedestrianModel.Agents.Reasoning.Pathfinding.Astar {

    /// <summary>
    ///     @author Christian Thiel
    /// </summary>
    public class AStarPathfinder<TE> : IPathfinder<TE> {
        /// <summary>
        ///     The search graph to use for the pathfinding.
        /// </summary>
        private readonly ISearchGraph<TE> _graph;

        /// <summary>
        ///     Creates a new AStar pathfinder using the given search graph.
        /// </summary>
        /// <param name="graph"> the graph to use </param>
        public AStarPathfinder(ISearchGraph<TE> graph) {
            _graph = graph;
        }

        #region IPathfinder<E> Members

        public IList<TE> FindPath(IPathNode<TE> from, IPathNode<TE> to) {
            AStarNode<TE> fromNode = new AStarNode<TE>(from, _graph);
            AStarNode<TE> toNode = new AStarNode<TE>(to, _graph);

            SortedSet<AStarNode<TE>> openList = new SortedSet<AStarNode<TE>>
                (new ComparatorAnonymousInnerClassHelper(toNode));
            Dictionary<IPathNode<TE>, AStarNode<TE>> openMap = new Dictionary<IPathNode<TE>, AStarNode<TE>>();
            Dictionary<IPathNode<TE>, AStarNode<TE>> closedMap = new Dictionary<IPathNode<TE>, AStarNode<TE>>();

            openList.Add(fromNode);

            while (openList.Count > 0) {
                //bestNode = openList.pollFirst();
                AStarNode<TE> bestNode = openList.First();
                openList.Remove(bestNode);

                if (toNode.Equals(bestNode)) return BuildPath(bestNode);
                ICollection<AStarNode<TE>> neighbors = bestNode.Neighbors;
                foreach (AStarNode<TE> newNode in neighbors) {
                    AStarNode<TE> oldVer;

                    //oldVer = openMap[newNode.ExternalNode];
                    if (openMap.ContainsKey(newNode.ExternalNode)) oldVer = openMap[newNode.ExternalNode];
                    else oldVer = null;

                    if (oldVer != null && oldVer.CostFromStart <= newNode.CostFromStart) continue;

                    //oldVer = closedMap[newNode.ExternalNode];
                    if (closedMap.ContainsKey(newNode.ExternalNode)) oldVer = closedMap[newNode.ExternalNode];
                    else oldVer = null;

                    if (oldVer != null && oldVer.CostFromStart <= newNode.CostFromStart) continue;
#warning Old "WALK" code seems to be incorrect here!
                    //closedMap.Remove(newNode); -> this should have no effect, because an AStarNode CANNOT be a key, only IPathNodes CAN
                    openList.Add(newNode);
                    openMap[newNode.ExternalNode] = newNode;
                }

                closedMap[bestNode.ExternalNode] = bestNode;
            }

            return null;
        }

        public ISearchGraph<TE> SearchGraph { get { return _graph; } }

        #endregion

        /// <summary>
        ///     Returns the path from the given node to the start node of its path.
        /// </summary>
        /// <param name="bestNode"> the end node of the path </param>
        /// <returns> the path to the given node </returns>
        private IList<TE> BuildPath(AStarNode<TE> bestNode) {
            IList<TE> backtracingPath = new List<TE>();
            IList<TE> path = new List<TE>();

            AStarNode<TE> currentNode = bestNode;

            while (currentNode != null) {
                backtracingPath.Add(currentNode.ExternalNode.AdaptedObject);
                currentNode = currentNode.Predecessor;
            }

            for (int i = backtracingPath.Count - 2; i >= 0; i--) {
                path.Add(backtracingPath[i]);
            }

            return path;
        }

        #region Nested type: ComparatorAnonymousInnerClassHelper

        private class ComparatorAnonymousInnerClassHelper : IComparer<AStarNode<TE>> {
            private readonly AStarNode<TE> _toNode;

            public ComparatorAnonymousInnerClassHelper(AStarNode<TE> toNode) {
                _toNode = toNode;
            }

            #region IComparer<AStarNode<E>> Members

            public int Compare(AStarNode<TE> o1, AStarNode<TE> o2) {
                int compare = (o1.CostFromStart + o1.GetCostToGoal(_toNode)).CompareTo
                    (o2.CostFromStart + o2.GetCostToGoal(_toNode));

                if (compare == 0) compare = (o1.GetHashCode()).CompareTo(o2.GetHashCode());

                return compare;
            }

            #endregion
        }

        #endregion
    }

}
using System.Collections.Generic;

namespace PedestrianModel.Agents.Reasoning.Pathfinding.Astar {

    /// <summary>
    ///     @author Christian Thiel
    /// </summary>
    public class AStarNode<TE> {
        /// <summary>
        ///     Returns the complete costs from the start value of this node path to this node.
        /// </summary>
        /// <returns> the costFromStart </returns>
        public double CostFromStart {
            get {
                if (_costFromStart == null) {
                    if (_predecessor == null) _costFromStart = 0d;
                    else {
                        _costFromStart = _predecessor.CostFromStart
                                        + _graph.Distance(_predecessor._externalNode, _externalNode);
                    }
                }

                //return costFromStart;
                return _costFromStart.Value;
            }
        }

        /// <summary>
        ///     Returns the predecessor of this node.
        /// </summary>
        /// <returns> the predecessor. </returns>
        public AStarNode<TE> Predecessor {
            get { return _predecessor; }
            set {
                _predecessor = value;
                _externalNode.Predecessor = value._externalNode;
                _costFromStart = null;
            }
        }


        /// <summary>
        ///     Returns a collection of all neighbors of this node based on the underlying search graph.
        /// </summary>
        /// <returns> a collection of all neighbors. </returns>
        public ICollection<AStarNode<TE>> Neighbors {
            get {
                ICollection<AStarNode<TE>> result = new HashSet<AStarNode<TE>>();

                ICollection<IPathNode<TE>> neighbors = _graph.GetNeighbors(_externalNode);

                foreach (IPathNode<TE> n in neighbors) {
                    AStarNode<TE> nb = new AStarNode<TE>(n, _graph) {Predecessor = this};
                    result.Add(nb);
                }

                return result;
            }
        }

        /// <summary>
        ///     Returns the external node from the search graph.
        /// </summary>
        /// <returns> the externalNode </returns>
        public IPathNode<TE> ExternalNode { get { return _externalNode; } }

        /// <summary>
        ///     The original search graph node.
        /// </summary>
        private readonly IPathNode<TE> _externalNode;

        /// <summary>
        ///     The search graph containing the external node.
        /// </summary>
        private readonly ISearchGraph<TE> _graph;

        /// <summary>
        ///     The predecessor of this AStar node. <code>null</code> if it has none.
        /// </summary>
        private AStarNode<TE> _predecessor;

        /// <summary>
        ///     Cached value of the costs from start.
        /// </summary>
        private double? _costFromStart;

        /// <summary>
        ///     Cached value of the predicted costs to goal.
        /// </summary>
        private double? _costToGoal;

        /// <summary>
        ///     Creates a new AStar node.
        /// </summary>
        /// <param name="graphNode"> the graph node </param>
        /// <param name="graph"> the search graph </param>
        public AStarNode(IPathNode<TE> graphNode, ISearchGraph<TE> graph) {
            _externalNode = graphNode;
            _graph = graph;
        }

        /// <summary>
        ///     Returns the estimated cost to the given goal node.
        /// </summary>
        /// <param name="goalNode"> the goal node </param>
        /// <returns> the costToGoal </returns>
        public double GetCostToGoal(AStarNode<TE> goalNode) {
            if (_costToGoal == null) _costToGoal = _graph.GetHeuristic(_externalNode, goalNode._externalNode);
            // return costToGoal;
            return _costToGoal.Value;
        }

        public override sealed int GetHashCode() {
            const int prime = 31;
            int result = 1;
            result = prime*result;

            if (!(_externalNode == null)) result += _externalNode.GetHashCode();

            return result;
        }

        public override sealed bool Equals(object obj) {
            if (this == obj) return true;
            if (obj == null) return false;
            if (GetType() != obj.GetType()) return false;
            AStarNode<TE> other = (AStarNode<TE>) obj;
            if (_externalNode == null) {
                if (other._externalNode != null) return false;
            }
            else if (!_externalNode.Equals(other._externalNode)) return false;
            return true;
        }

        public override sealed string ToString() {
            return _externalNode.ToString();
        }
    }

}
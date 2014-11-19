using System;
using System.Collections.Generic;
using System.Linq;
using GoapCommon.Exceptions;
using GoapCommon.Interfaces;

namespace GoapGraphConnector.SimpleGraph {

    /// <summary>
    ///     tasks are creating the management table of vertices, recalculating the entrys, add vertices to list, check for
    ///     reaching target. Condition for correct work is a graph with positiv edge weights.
    /// </summary>
    public class AStarSteppable {
        public IGoapNode Current { get { return _current; } }
        private readonly IGoapNode _root;
        private readonly Map _graph;
        private IGoapNode _current;
        private Dictionary<IGoapNode, object[]> _nodeTable;

        /// <summary>
        ///     the a star search providing class
        /// </summary>
        /// <param name="root"></param>
        /// <param name="graph"></param>
        public AStarSteppable(IGoapNode root, Map graph) {
            _root = root;
            _graph = graph;
            InitializeAStar();
        }

        /// <summary>
        ///     initialize the data list for the astar and the first node (the root node)
        /// </summary>
        private void InitializeAStar() {
            _nodeTable = new Dictionary<IGoapNode, object[]>();
            object[] entry = CreateNodeEntry(_root, _root.GetHeuristic(), 0, _root.GetHeuristic());
            _nodeTable.Add(_root, entry);
            _current = _root;
        }

        /// <summary>
        ///     set the node to closed list and calculate entries for sucessors: 
        ///     predecessor, heuristic, traveldistance and estimated
        /// </summary>
        public void CalculateCurrentNode() {
            SetOnClosedList(_current);
            Calculate(_current, _graph.GetReachableAdjcentVertices(_current));
        }

        /// <summary>
        ///     Select the first vertex from the open list sorted by estimate value
        /// </summary>
        public IGoapNode ChooseNextNodeFromOpenList() {
            if (!HasAnyVertexOnOpenList()) {
                throw new Exception("AStarSteppable: cannot choose next node from open list because it is empty ");
            }

            Dictionary<IGoapNode, object[]> openList = GetOpenList();
            IOrderedEnumerable<KeyValuePair<IGoapNode, object[]>> orderedOpenList = openList.OrderBy
                (entry => entry.Value[3]);
            IGoapNode cheapestFromOpenList = orderedOpenList.First().Key;

            if (cheapestFromOpenList == null) {
                throw new Exception("AStarSteppable: open list contains null elem");
            }

            _current = cheapestFromOpenList;
            return orderedOpenList.First().Key;
        }

        /// <summary>
        ///     get the nodes from the node table not closed
        /// </summary>
        /// <returns></returns>
        private Dictionary<IGoapNode, object[]> GetOpenList() {
            Dictionary<IGoapNode, object[]> openList = new Dictionary<IGoapNode, object[]>();

            foreach (KeyValuePair<IGoapNode, object[]> node in _nodeTable) {
                if (node.Value != null && (bool) node.Value[4] == false) {
                    openList.Add(node.Key, node.Value);
                }
            }
            return openList;
        }

        /// <summary>
        ///     check if open list is not empty
        /// </summary>
        /// <returns></returns>
        public bool HasAnyVertexOnOpenList() {
            return _nodeTable.Any(keyValuePair => keyValuePair.Value != null && (bool) keyValuePair.Value[4] == false);
        }

        /// <summary>
        ///     add a vertex to the algorithm table
        /// </summary>
        /// <param name="vertex"></param>
        public void AddVertex(IGoapNode vertex) {
            if (!_nodeTable.ContainsKey(vertex)) {
                _nodeTable.Add(vertex, null);
            }
            //TODO falls ein knoten schon in der Nodetable ist und noch open ist .... checken ob er geupdated werden muss
        }

        /// <summary>
        ///     get the list of all edges needed to walk to current node
        /// </summary>
        /// <returns></returns>
        public List<IGoapEdge> CreatePathToCurrentAsEdgeList() {
            List<IGoapEdge> pathEdges = new List<IGoapEdge>();
            IGoapNode actual = _current;
            IGoapNode pre = GetPredecessor(actual);

            while (actual != null && !actual.Equals(pre)) {
                pathEdges.Add(_graph.GetCheapestEdgeBySourceAndTarget(pre, actual));
                actual = pre;
                pre = GetPredecessor(actual);
            }
            pathEdges.Reverse();
            return pathEdges;
        }


        /// <summary>
        ///     get the predecessor of the node from the table from a star
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        private IGoapNode GetPredecessor(IGoapNode vertex) {
            if (!_nodeTable.ContainsKey(vertex)) {
                throw new AlgorithmException("vertex asked for predeseccor not in algoritm list");
            }

            object[] value;
            if (!_nodeTable.TryGetValue(vertex, out value)) {
                throw new AlgorithmException("node tab not in algoritm list");
            }
            return (IGoapNode) value[0];
        }

        /// <summary>
        ///     update or add entries for the sucessors of the current node
        /// </summary>
        /// <param name="current"></param>
        /// <param name="reachableVertices"></param>
        private void Calculate(IGoapNode current, List<IGoapNode> reachableVertices) {
            // check if all are in the node list
            if (reachableVertices.Any(v => !_nodeTable.ContainsKey(v))) {
                throw new AlgorithmException("Inconsistence in node list. a reachable vertex is not in the nodelist");
            }

            object[] currentValue;
            _nodeTable.TryGetValue(current, out currentValue);
            if (currentValue == null) {
                throw new AlgorithmException("Inconsistence current vertex: " + current + " has no entry in node table");
            }

            // filter out the vertices on closed list
            List<IGoapNode> reachableOnOpenList = new List<IGoapNode>();
            foreach (IGoapNode node in reachableVertices) {
                object[] value;
                _nodeTable.TryGetValue(node, out value);
                if (value == null || (bool) value[4] == false) {
                    reachableOnOpenList.Add(node);
                }
            }

            foreach (IGoapNode openVertex in reachableOnOpenList) {
                object[] value;
                _nodeTable.TryGetValue(openVertex, out value);

                // create the new values for comparison if predecessor would be current vertex
                int heuristic = openVertex.GetHeuristic();
                int travelDistanceG = (int) currentValue[2] + _graph.GetcheapestWayCost(current, openVertex);
                int estimatedValueF = travelDistanceG + heuristic;

                // check if the entry of the vertex must be created ...
                if (value == null) {
                    object[] entry = CreateNodeEntry(current, heuristic, travelDistanceG, estimatedValueF);
                    _nodeTable.Remove(openVertex);
                    _nodeTable.Add(openVertex, entry);
                }
                    // ... or updated because it was a cheaper way found
                else if ((int) value[2] > travelDistanceG) {
                    object[] entry = CreateNodeEntry(current, heuristic, travelDistanceG, estimatedValueF);
                    _nodeTable.Remove(openVertex);
                    _nodeTable.Add(openVertex, entry);
                }
            }
        }

        /// <summary>
        ///     manipulate entry for node in algorithm table - set status to in closed list
        /// </summary>
        /// <param name="vertex"></param>
        private void SetOnClosedList(IGoapNode vertex) {
            object[] entryForChange;
            _nodeTable.TryGetValue(vertex, out entryForChange);
            if (entryForChange == null) {
                throw new AlgorithmException
                    ("a* missing entry for predessor and so forth at node: " + vertex +
                     " in algorithm table");
            }
            if ((bool) entryForChange[4]) {
                throw new AlgorithmException
                    ("a* vertex: " + vertex +
                     " already on closed list was set to closed list again");
            }
            entryForChange[4] = true;
        }

        /// <summary>
        ///     create the entry to one vertex for the list of the astar algoritm
        /// </summary>
        /// <param name="predecessorP"></param>
        /// <param name="heuristicH"></param>
        /// <param name="travelDistanceG"></param>
        /// <param name="estimatedValueF"></param>
        /// <param name="onClosedListCl"></param>
        /// <returns></returns>
        private object[] CreateNodeEntry
            (IGoapNode predecessorP = null,
                int heuristicH = int.MaxValue,
                int travelDistanceG = int.MaxValue,
                int estimatedValueF = int.MaxValue,
                bool onClosedListCl = false) {
            object[] entry = new object[5];
            entry[0] = predecessorP;
            entry[1] = heuristicH;
            entry[2] = travelDistanceG;
            entry[3] = estimatedValueF;
            entry[4] = onClosedListCl;

            return entry;
        }
    }

}
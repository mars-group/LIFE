using System;
using System.Collections.Generic;
using System.Linq;
using GoapCommon.Exceptions;
using GoapCommon.Interfaces;

namespace GoapGraphConnector.CustomGraph {
    /// <summary>
    ///     tasks are creating the management table of vertices, recalculating the entrys, add vertices to list, check for
    ///     reaching target
    ///     workflow
    ///     1. create new list for vertices and fill in root (root is tagged with not on closed list)
    ///     repeat
    ///     2. take cheapest on open list
    ///     3. mark this as on closed list
    ///     4 inspect reachable nodes
    ///     each reachable
    ///     create an entry for reachable
    /// </summary>
    public class AStarSteppable {
        private IGoapVertex _current;
        private Dictionary<IGoapVertex, object[]> _nodeTable;
        private readonly IGoapVertex _root;
        private readonly IGoapVertex _target;
        private readonly Graph _graph;

        public IGoapVertex Current {
            get { return _current; }
        }

        /// <summary>
        /// </summary>
        /// <param name="root"></param>
        /// <param name="target"></param>
        /// <param name="graph"></param>
        public AStarSteppable(IGoapVertex root, IGoapVertex target, Graph graph) {
            _root = root;
            _target = target;
            _graph = graph;
            InitializeAStar();
        }

        /// <summary>
        ///     initialize the data list for the astar and the first node (the root node)
        /// </summary>
        private void InitializeAStar() {
            if (_nodeTable != null) return;
            _nodeTable = new Dictionary<IGoapVertex, object[]>();
            var entry = CreateNodeEntry(_root, _root.GetHeuristic(_target), 0, _root.GetHeuristic(_target));
            _nodeTable.Add(_root, entry);
            _current = _root;
        }

        /// <summary>
        ///     Select the vertex from the open list with the best estimate
        /// </summary>
        public IGoapVertex ChooseNextNodeFromOpenList() {
            int smallestF = int.MaxValue;
            IGoapVertex vertex = null;

            // TODO in welcher Reihenfolge werden die Elemente iteriert (ist vermutlich unterschiedlich bei dict) und führt das zu Fehlern?
            foreach (KeyValuePair<IGoapVertex, object[]> keyValuePair in _nodeTable) {
                // if not in closed list and estimated value smaller than actual smallestF
                if (keyValuePair.Value != null && (bool) keyValuePair.Value[4] == false &&
                    (int) keyValuePair.Value[3] < smallestF) {
                    smallestF = (int) keyValuePair.Value[3];
                    vertex = keyValuePair.Key;
                }
            }

            if (vertex == null) throw new NoVertexFoundException("a* no node found in open list");
            _current = vertex;
            return _current;
        }

        public bool HasVerticesOnOpenList() {
            return _nodeTable.Any(keyValuePair => keyValuePair.Value != null && (bool) keyValuePair.Value[4] == false);
        }

        /// <summary>
        ///     compare current vertex to target vertex
        /// </summary>
        /// <returns></returns>
        public bool CheckforTarget() {
            return _current.Equals(_target);
        }

        /// <summary>
        ///     create the new values for distance, estimated value and so forth
        ///     neccessary when the first way to a vertex is found or if a shorter way (new predeccessor) is found
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        private IGoapVertex UpdateEntry(IGoapVertex vertex) {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     add the new children of the current chosen
        /// </summary>
        /// <param name="vertices"></param>
        public void AddVertices(List<IGoapVertex> vertices) {
            foreach (var vertex in vertices) {
                if (_nodeTable.ContainsKey(vertex)) continue;
                _nodeTable.Add(vertex, null);
            }
        }

        public void AddVertex(IGoapVertex vertex) {
            if (!_nodeTable.ContainsKey(vertex)) _nodeTable.Add(vertex, null);
        }

        public List<IGoapEdge> CreateResultListToCurrent() {
            List<IGoapEdge> pathEdges = new List<IGoapEdge>();
            IGoapVertex actual = _current;
            IGoapVertex pre = GetPredecessor(actual);

            while (actual != null && !actual.Equals(pre)) {
                pathEdges.Add(_graph.GetEdge(pre, actual));
                actual = pre;
                pre = GetPredecessor(actual);
            }
            pathEdges.Reverse();
            return pathEdges;
        }

        public void Step() {
            SetOnClosedList(_current);
            Calculate(_current, _graph.GetReachableAdjcentVertices(_current));
            ChooseNextNodeFromOpenList();
        }

        private IGoapVertex GetPredecessor(IGoapVertex vertex) {
            if (!_nodeTable.ContainsKey(vertex))
                throw new AlgorithmException("vertex asked for predeseccor not in algoritm list");

            object[] value;
            if (!_nodeTable.TryGetValue(vertex, out value))
                throw new AlgorithmException("node tab not in algoritm list");

            return (IGoapVertex) value[0];
        }


        private void Calculate(IGoapVertex current, List<IGoapVertex> reachableVertices) {
            // check if all are in the node list
            if (reachableVertices.Any(v => !_nodeTable.ContainsKey(v)))
                throw new AlgorithmException("Inconsistence in node list. a reachable vertex is not in the nodelist");

            object[] currentValue;
            _nodeTable.TryGetValue(current, out currentValue);
            if (currentValue == null)
                throw new AlgorithmException("Inconsistence current vertex: " + current + " has no entry in node table");

            // filter out the vertices on closed list
            var reachableOnOpenList = new List<IGoapVertex>();
            foreach (var v in reachableVertices) {
                object[] value;
                _nodeTable.TryGetValue(v, out value);
                if (value == null || (bool) value[4] == false) reachableOnOpenList.Add(v);
            }

            foreach (IGoapVertex openVertex in reachableOnOpenList) {
                object[] value;
                _nodeTable.TryGetValue(openVertex, out value);

                // create the new values for comparison if predecessor would be current vertex
                var heuristic = openVertex.GetHeuristic(_target);
                var travelDistanceG = (int) currentValue[2] + _graph.GetWayCost(current, openVertex);
                var estimatedValueF = travelDistanceG + heuristic;


                // check if the entry of the vertex must be updated because it was a cheaper way found
                if (value == null) {
                    var entry = CreateNodeEntry(current, heuristic, travelDistanceG, estimatedValueF);
                    _nodeTable.Remove(openVertex);
                    _nodeTable.Add(openVertex, entry);
                }
                else if ((int) value[2] > travelDistanceG) {
                    var entry = CreateNodeEntry(current, heuristic, travelDistanceG, estimatedValueF);
                    _nodeTable.Remove(openVertex);
                    _nodeTable.Add(openVertex, entry);
                }
            }
        }


        /// <summary>
        ///     manipulate entry for node in algorithm table - set status to in closed list
        /// </summary>
        /// <param name="vertex"></param>
        private void SetOnClosedList(IGoapVertex vertex) {
            object[] entryForChange;
            _nodeTable.TryGetValue(vertex, out entryForChange);
            if (entryForChange == null) {
                throw new AlgorithmException("a* missing entry for predessor and so forth at node: " + vertex +
                                             " in algorithm table");
            }
            if ((bool) entryForChange[4]) {
                throw new AlgorithmException("a* vertex: " + vertex +
                                             " already on closed list was set to closed list again");
            }
            entryForChange[4] = true;
        }

        /// <summary>
        ///     create the entry to one vertex for the list of the astar algoritm
        /// </summary>
        /// <param name="predecessor"></param>
        /// <param name="heuristic"></param>
        /// <param name="travelDistanceG"></param>
        /// <param name="estimatedValueF"></param>
        /// <param name="onClosedList"></param>
        /// <returns></returns>
        private object[] CreateNodeEntry(IGoapVertex predecessor = null, int heuristic = int.MaxValue,
            int travelDistanceG = int.MaxValue,
            int estimatedValueF = int.MaxValue, bool onClosedList = false) {
            object[] entry = new object[5];
            entry[0] = predecessor;
            entry[1] = heuristic;
            entry[2] = travelDistanceG;
            entry[3] = estimatedValueF;
            entry[4] = onClosedList;

            return entry;
        }
    }
}
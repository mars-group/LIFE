using System;
using System.Collections.Generic;
using System.Linq;
using GoapCommon.Exceptions;

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
    ///     create an entry forreachable
    /// </summary>
    internal class AStarSteppable {
        private Vertex _current;
        private Dictionary<Vertex, List<Object>> _nodeTable;
        private readonly Vertex _root;
        private readonly Vertex _target;
        private readonly Graph _graph;
        private bool _targetFound = false;

        /// <summary>
        /// </summary>
        /// <param name="root"></param>
        /// <param name="target"></param>
        public AStarSteppable(Vertex root, Vertex target, Graph graph) {
            _root = root;
            _target = target;
            _graph = graph;
            InitializeAStar();
        }

        /// <summary>
        ///     initialize the data list for the astar and the first node (the root node)
        /// </summary>
        private void InitializeAStar() {
            if (_nodeTable == null) {
                _nodeTable = new Dictionary<Vertex, List<Object>>();
                var entry = CreateNodeEntry(_root, _root.GetHeuristic(_target), 0, _root.GetHeuristic(_target));
                _nodeTable.Add(_root, entry);
            }
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
        private List<Object> CreateNodeEntry(Vertex predecessor = null, int heuristic = int.MaxValue,
            int travelDistanceG = int.MaxValue,
            int estimatedValueF = int.MaxValue, bool onClosedList = false) {
            var entry = new List<object>();
            entry[0] = predecessor;
            entry[1] = heuristic;
            entry[2] = travelDistanceG;
            entry[3] = estimatedValueF;
            entry[4] = onClosedList;

            return entry;
        }

        /// <summary>
        ///     choose the next vertex from open list to handle
        /// </summary>
        private void ChooseNextNodeFromOpenList() {
            int smallestF = int.MaxValue;
            Vertex vertex = null;

            // TODO in welcher Reihenfolge werden die Elemente iteriert (ist vermutlich unterschiedlich bei dict) und führt das zu Fehlern?
            foreach (KeyValuePair<Vertex, List<object>> keyValuePair in _nodeTable) {
                // if not in closed list and estimated value smaller than actual smallestF
                if ((bool) keyValuePair.Value[4] && (int) keyValuePair.Value[3] < smallestF) {
                    smallestF = (int) keyValuePair.Value[3];
                    vertex = keyValuePair.Key;
                }
            }

            if (vertex == null) throw new NoVertexFoundException("a* no node found in open list");
            _current = vertex;
        }

        /// <summary>
        ///     compare current vertex to target vertex
        /// </summary>
        /// <returns></returns>
        private bool CheckforTarget() {
            return _current.Equals(_target);
        }

        /// <summary>
        ///     manipulate entry for node in algorithm table - set status to in closed list
        /// </summary>
        /// <param name="vertex"></param>
        private void SetOnClosedList(Vertex vertex) {
            List<Object> entryForChange;
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
        ///     create the new values for distance, estimated value and so forth
        ///     neccessary when the first way to a vertex is found or if a shorter way (new predeccessor) is found
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        private Vertex UpdateEntry(Vertex vertex) {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     add the new children of the current chosen
        /// </summary>
        /// <param name="vertices"></param>
        public void AddVertices(List<Vertex> vertices) {
            foreach (var vertex in vertices) {
                if (_nodeTable.ContainsKey(vertex)) continue;
                _nodeTable.Add(vertex, null);
            }
        }

        private List<Edge> CreateResultList() {
            throw new NotImplementedException();
        }

        private void Calculate(Vertex current, List<Vertex> reachableVertices) {
            // check if all are in the node list
            if (reachableVertices.Any(v => !_nodeTable.ContainsKey(v)))
                throw new AlgorithmException("Inconsistence in nodelist. a reachable vertex is not in the nodelist");

            List<Object> currentValue;
            _nodeTable.TryGetValue(current, out currentValue);
            if (currentValue == null)
                throw new AlgorithmException("Inconsistence current vertex: " + current + " has no entry in node table");

            // filter out the vertices on closed list
            var reachableOnOpenList = new List<Vertex>();
            foreach (var v in reachableVertices) {
                List<object> value;
                _nodeTable.TryGetValue(v, out value);
                if (value == null || (bool) value[4] == false) reachableOnOpenList.Add(v);
            }

            foreach (Vertex openVertex in reachableOnOpenList) {
                List<object> value;
                _nodeTable.TryGetValue(openVertex, out value);

                // create the new values for comparison if predecessor would be current vertex
                var heuristic = openVertex.GetHeuristic(_target);
                var travelDistanceG = (int)currentValue[2] + _graph.GetWayCost(current, openVertex);
                var estimatedValueF = travelDistanceG + heuristic;


                if (value != null && (int) value[3] <= estimatedValueF) continue;
                var entry = CreateNodeEntry(current, heuristic, travelDistanceG, estimatedValueF);
                _nodeTable.Add(openVertex, entry);
            }
            // check if entry must be updated (if there is a cheaper way) or if they need the first entry
        }
    }
}
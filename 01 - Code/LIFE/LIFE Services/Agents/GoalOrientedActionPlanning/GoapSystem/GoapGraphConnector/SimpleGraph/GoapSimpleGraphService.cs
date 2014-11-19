using System.Collections.Generic;
using GoapCommon.Interfaces;

namespace GoapGraphConnector.SimpleGraph {

    /// <summary>
    ///     fascade class for the services of holding graph data and usage of astar
    /// </summary>
    public class GoapSimpleGraphService : IGoapGraphService {
        private IGoapNode _root;
        private Map _map;
        private AStarSteppable _aStar;

        #region IGoapGraphService Members

        public void InitializeGoapGraph(IGoapNode root, int maximumGraphDept = 0) {
            _root = root;
            _map = new Map(_root);
            _aStar = new AStarSteppable(_root, _map);
        }

        /// <summary>
        ///     add vertices and edges to the graph data
        /// </summary>
        /// <param name="outEdges"></param>
        public void ExpandCurrentVertex(List<IGoapEdge> outEdges) {
            foreach (IGoapEdge edge in outEdges) {
                _map.AddVertex(edge.GetTarget());
                _map.AddEdge(edge);
                _aStar.AddVertex(edge.GetTarget());
            }
        }

        /// <summary>
        ///     set node on closed list and create values of child nodes
        /// </summary>
        public void CalculateCurrentNode() {
            _aStar.CalculateCurrentNode();
        }

        /// <summary>
        ///     check if open list contains a vertex
        /// </summary>
        /// <returns></returns>
        public bool HasNextVertexOnOpenList() {
            return _aStar.HasAnyVertexOnOpenList();
        }

        /// <summary>
        ///     get the cheapest node from open list if available
        /// </summary>
        public void ChooseNextNodeFromOpenList() {
            _aStar.ChooseNextNodeFromOpenList();
        }

        /// <summary>
        ///     get the next Vertex, which will be calculated by the algorithm
        /// </summary>
        /// <returns></returns>
        public IGoapNode GetNextVertex() {
            return _aStar.Current;
        }

        /// <summary>
        ///     get sorted list of actions representing the path.
        ///     starting at current and finish with root
        /// </summary>
        /// <returns></returns>
        List<IGoapEdge> IGoapGraphService.GetShortestPath() {
            List<IGoapEdge> edges = _aStar.CreatePathToCurrentAsEdgeList();
            return edges;
        }

        /// <summary>
        ///     walk path from root to current and retrun count of edges
        /// </summary>
        /// <returns></returns>
        public int GetActualDepthFromRoot() {
            return _aStar.CreatePathToCurrentAsEdgeList().Count;
        }

        #endregion
    }

}
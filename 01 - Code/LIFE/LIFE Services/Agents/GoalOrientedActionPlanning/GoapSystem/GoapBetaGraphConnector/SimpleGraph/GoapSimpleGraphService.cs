using System.Collections.Generic;
using GoapBetaCommon.Interfaces;

namespace GoapBetaGraphConnector.SimpleGraph {
    public class GoapSimpleGraphService : IGoapGraphService {
        private IGoapNode _root;
        private Map _map;
        private AStarSteppable _aStar;

        #region IGoapGraphService Members

        public void InitializeGoapGraph(IGoapNode root, int maximumGraphDept = 0) {
            _root = root;
            _map = new Map(new List<IGoapNode> {_root}, new List<IGoapEdge>());
            _aStar = new AStarSteppable(_root, _map);
        }

        public IGoapNode GetNextVertexFromOpenList() {
            return _aStar.Current;
        }

        public bool HasNextVertexOnOpenList() {
            return _aStar.HasVerticesOnOpenList();
        }

        public void ExpandCurrentVertex(List<IGoapEdge> outEdges) {
            foreach (IGoapEdge edge in outEdges) {
                _map.AddVertex(edge.GetTarget());
                _map.AddEdge(edge);
                _aStar.AddVertex(edge.GetTarget());
            }
        }

        public void AStarStep() {
            _aStar.Step();
        }

        /// <summary>
        ///     Sorted list of actions starting at current and finish with root
        /// </summary>
        /// <returns></returns>
        List<IGoapEdge> IGoapGraphService.GetShortestPath() {
            List<IGoapEdge> edges = _aStar.CreatePathToCurrentAsEdgeList();
            return edges;
        }

        /// <summary>
        ///     walk path from root to current and count edges
        /// </summary>
        /// <returns></returns>
        public int GetActualDepthFromRoot() {
            return _aStar.CreatePathToCurrentAsEdgeList().Count;
        }

        #endregion
    }
}
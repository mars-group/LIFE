using System;
using System.Collections.Generic;
using GoapCommon.Interfaces;

namespace GoapGraphConnector.CustomGraph {
    public class GoapCustomGraphService : IGoapGraph {
        private IGoapVertex _root;
        private IGoapVertex _target;
        private Graph _graph;
        private AStarSteppable _aStar;


        public void InitializeGoapGraph(List<IGoapWorldstate> rootState, List<IGoapWorldstate> targetState,
            int maximumGraphDept = 0) {
            _root = new Vertex(rootState);
            _target = new Vertex(targetState);
            InitializeGoapGraph(_root, _target, maximumGraphDept);
        }

        public void InitializeGoapGraph(IGoapVertex root, IGoapVertex target,
            int maximumGraphDept = 0) {
            _root = root;
            _target = target;
            _graph = new Graph(new List<IGoapVertex> {_root}, new List<IGoapEdge>());
            _aStar = new AStarSteppable(_root, _target, _graph);
        }

        public bool IsGraphEmpty() {
            if (_graph == null) return true;
            return _graph.IsEmpty();
        }

        public IGoapVertex GetNextVertexFromOpenList() {
            return _aStar.Current;
        }

        public Graph Graph {
            get { return _graph; }
        }

        public bool HasNextVertexOnOpenList() {
            throw new NotImplementedException();
        }

        public void ExpandCurrentVertex(List<IGoapAction> outEdges) {
            throw new NotImplementedException();
        }

        public void ExpandCurrentVertex(List<IGoapEdge> outEdges) {
            foreach (var edge in outEdges) {
                _graph.AddVertex(edge.GetTarget());
                _graph.AddEdge(edge);
                _aStar.AddVertex(edge.GetTarget());
            }
        }


        public bool IsCurrentVertexTarget() {
            return _aStar.CheckforTarget();
        }

        public void AStarStep() {
           
           
            _aStar.Step();


        }

        /// <summary>
        /// Sorted list of edges, where the first edge is outgoing from the start state. 
        /// List ends at the current Vertex.
        /// </summary>
        /// <returns></returns>
        public List<IGoapAction> GetShortestPath() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sorted list of edges, where the first edge is outgoing from the start state. 
        /// List ends at the current Vertex.
        /// </summary>
        /// <returns></returns>
        public List<IGoapEdge> GetEdgesList() {
            return _aStar.CreateResultListToCurrent();
        }

        public int GetActualDepthFromRoot() {
            throw new NotImplementedException();
        }
    }
}
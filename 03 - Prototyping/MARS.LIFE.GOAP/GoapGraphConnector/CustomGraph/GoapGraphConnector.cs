using System;
using System.Collections.Generic;
using GoapCommon.Interfaces;

namespace GoapGraphConnector.CustomGraph {

    public class GoapGraphConnector : IGoapGraph {
        private Vertex _root;
        private Vertex _target;
        private Graph _graph;
        private AStarSteppable _aStar;


        public void InitializeGoapGraph(List<IGoapWorldstate> rootState, List<IGoapWorldstate> targetState,
            int maximumGraphDept = 0) {
            _root = new Vertex(rootState);
            _target = new Vertex(targetState);
            _graph = new Graph(new List<IGoapVertex> {_root}, new List<IGoapEdge>());
            _aStar = new AStarSteppable(_root, _target, _graph);
        }

        public bool IsGraphEmpty() {
            if (_graph == null) return true;
            return _graph.IsEmpty();
        }

        public IGoapVertex GetNextVertexFromOpenList() {
            throw new NotImplementedException();
        }

        public bool HasNextVertexOnOpenList() {
            throw new NotImplementedException();
        }

        public bool ExpandCurrentVertex(List<IGoapAction> outEdges) {
            throw new NotImplementedException();
        }

        public bool IsCurrentVertexTarget() {
            throw new NotImplementedException();
        }

        public bool AStarStep() {
            throw new NotImplementedException();
        }

        public List<IGoapAction> GetShortestPath() {
            throw new NotImplementedException();
        }

        public int GetActualDepthFromRoot() {
            throw new NotImplementedException();
        }
    }
}
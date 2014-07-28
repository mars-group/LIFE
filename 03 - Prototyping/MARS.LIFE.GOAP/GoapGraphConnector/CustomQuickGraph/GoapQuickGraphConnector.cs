using System;
using System.Collections.Generic;
using GoapCommon.Interfaces;

namespace GoapGraphConnector.CustomQuickGraph {
    public class GoapQuickGraphConnector : IGoapGraph {

        public IGoapGraph CreateGoapGraph(List<IGoapWorldstate> rootState, List<IGoapWorldstate> targetState, int maximumGraphDept = 0) {
            throw new NotImplementedException();
        }

        public bool IsGraphEmpty() {
            throw new NotImplementedException();
        }

        public IGoapVertex GetNextVertexFromOpenList() {
            throw new NotImplementedException();
        }

        public bool HasNextVertexOnOpenList()
        {
            throw new NotImplementedException();
        }

        public bool ExpandCurrentVertex(List<IGoapAction> outEdges)
        {
            throw new NotImplementedException();
        }

        public bool IsCurrentVertexTarget()
        {
            throw new NotImplementedException();
        }

        public bool AStarStep() {
            throw new NotImplementedException();
        }

        public int GetActualDepthFromRoot() {
            throw new NotImplementedException();
        }

        public List<IGoapAction> GetShortestPath() {
            throw new NotImplementedException();
        }
    }
}
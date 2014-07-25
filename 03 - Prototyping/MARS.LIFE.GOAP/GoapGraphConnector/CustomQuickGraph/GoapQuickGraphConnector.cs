using System;
using System.Collections.Generic;
using GoapCommon.Interfaces;

namespace GoapGraphConnector.CustomQuickGraph {
    public class GoapQuickGraphConnector : IGoapGraph {
        public IGoapGraph CreateGoapGraph(List<IGoapWorldstate> rootState, List<IGoapWorldstate> targetState,
            int maximumGraphDept = 0) {
            throw new NotImplementedException();
        }

        public bool HasCircles() {
            throw new NotImplementedException();
        }

        public bool IsGraphEmpty() {
            throw new NotImplementedException();
        }

        public IGoapVertex GetNextVertexOnWhiteList() {
            throw new NotImplementedException();
        }

        public bool HasNextVertex() {
            throw new NotImplementedException();
        }

        public List<IGoapVertex> ExpandVertex(IGoapVertex vertex, List<IGoapAction> outEdges) {
            throw new NotImplementedException();
        }

        public bool ExpandNextVertex() {
            throw new NotImplementedException();
        }

        public bool IsVertexTarget(IGoapVertex vertex) {
            throw new NotImplementedException();
        }
       
    }
}
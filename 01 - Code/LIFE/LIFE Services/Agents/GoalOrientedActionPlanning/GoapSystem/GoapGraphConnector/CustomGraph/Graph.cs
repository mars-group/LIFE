using System;
using System.Collections.Generic;
using System.Linq;
using GoapCommon.Exceptions;
using GoapCommon.Interfaces;

namespace GoapGraphConnector.CustomGraph {
    public class Graph {
        private List<IGoapVertex> _vertices;
        private List<IGoapEdge> _edges;

        public List<IGoapVertex> GetVertices() {
            return _vertices;
        }

        public List<IGoapEdge> GetEdges(){
            return _edges;
        }

        public Graph(List<IGoapVertex> vertices, List<IGoapEdge> edges) {
            _vertices = vertices;
            _edges = edges;
        }

        public void AddEdge(IGoapEdge edge) {
            _edges.Add(edge);
        }

        public void AddVertex(IGoapVertex vertex) {
            if(!_vertices.Contains(vertex))_vertices.Add(vertex);
        }

        public bool IsEmpty() {
            if (_vertices == null) return true;
            return _vertices.Count == 0;
        }

        public override string ToString() {
            var text = _vertices.Aggregate("", (current, vertex) => current + (vertex + Environment.NewLine));

            text = text + _edges.Aggregate("", (current, edge) => current + (edge + Environment.NewLine));
            
            return text;
        }


        public IGoapEdge GetEdgeBySourceAndTarget(IGoapVertex source, IGoapVertex target) {
            if (!_vertices.Contains(source) || !_vertices.Contains(target))
                throw new GraphException("source or target of edge not available in graph");

            IGoapEdge bindingEdge = _edges.Find(e => e.GetSource().Equals(source) && e.GetTarget().Equals(target));
            if (bindingEdge == null)
                throw new GraphException("Edge not found by target and source");
            return bindingEdge;
        }


        public int GetWayCost(IGoapVertex sourceVertex, IGoapVertex targetVertex) {
            IGoapEdge edge = GetEdgeBySourceAndTarget(sourceVertex, targetVertex);
            return edge.GetCost();
        }

        public List<IGoapVertex> GetReachableAdjcentVertices(IGoapVertex vertex) {
            List<IGoapEdge> outEdges = GetEdgesBySourceVertex(vertex);

            return outEdges.Select(outEdge => outEdge.GetTarget()).ToList();
        }

        /// <summary>
        ///     get the positiv incident edges by vertex (source of edge is vertex)
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        private List<IGoapEdge> GetEdgesBySourceVertex(IGoapVertex vertex) {
            return _edges.Where(edge => edge.GetSource().Equals(vertex)).ToList();
        }
    }
}
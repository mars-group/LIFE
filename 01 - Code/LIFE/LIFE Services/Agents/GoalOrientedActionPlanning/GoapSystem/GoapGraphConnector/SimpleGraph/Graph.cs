using System;
using System.Collections.Generic;
using System.Linq;
using GoapCommon.Exceptions;
using GoapCommon.Interfaces;

namespace GoapGraphConnector.SimpleGraph {
    public class Graph {
        private List<IGoapNode> _vertices;
        private List<IGoapEdge> _edges;

        public List<IGoapNode> GetVertices() {
            return _vertices;
        }

        public List<IGoapEdge> GetEdges(){
            return _edges;
        }

        public Graph(List<IGoapNode> vertices, List<IGoapEdge> edges) {
            _vertices = vertices;
            _edges = edges;
        }

        public void AddEdge(IGoapEdge edge) {
            MaintainConsistence(edge);
            _edges.Add(edge);
        }

        private void MaintainConsistence(IGoapEdge addedEdge) {
            if (!ContainsVertex(addedEdge.GetSource())) {
                AddVertex(addedEdge.GetSource());
            }
            if (!ContainsVertex(addedEdge.GetTarget())) {
                AddVertex(addedEdge.GetTarget());
            }
        }

        public void AddVertex(IGoapNode vertex) {
            if(!_vertices.Contains(vertex))_vertices.Add(vertex);
        }

        public bool ContainsVertex(IGoapNode vertex){
            return _vertices.Contains(vertex);
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


        public List<IGoapEdge> GetEdgesBySourceAndTarget(IGoapNode source, IGoapNode target) {
            if (!ContainsVertex(source) || !ContainsVertex(target))
                throw new GraphException("source or target of edge not available in graph");

            List<IGoapEdge> bindingEdge = _edges.FindAll(e => e.GetSource().Equals(source) && e.GetTarget().Equals(target));
            if (bindingEdge == null)
                throw new GraphException("Edge not found by target and source");
            return bindingEdge;
        }

        public IGoapEdge GetCheapestEdgeBySourceAndTarget(IGoapNode source, IGoapNode target) {
            var query = GetEdgesBySourceAndTarget(source, target).OrderBy(e => e.GetCost());
            return query.First();
        }


        public int GetcheapestWayCost(IGoapNode sourceVertex, IGoapNode targetVertex) {
            IGoapEdge edge = GetCheapestEdgeBySourceAndTarget(sourceVertex, targetVertex);
            return edge.GetCost();
        }

        public List<IGoapNode> GetReachableAdjcentVertices(IGoapNode vertex) {
            List<IGoapEdge> outEdges = GetEdgesBySourceVertex(vertex);

            return outEdges.Select(outEdge => outEdge.GetTarget()).ToList();
        }

        /// <summary>
        ///     get the positiv incident edges by vertex (source of edge is vertex)
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public List<IGoapEdge> GetEdgesBySourceVertex(IGoapNode vertex) {
            return _edges.Where(edge => edge.GetSource().Equals(vertex)).ToList();
        }
    }
}
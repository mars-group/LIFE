using System;
using System.Collections.Generic;
using System.Linq;
using GoapCommon.Exceptions;
using GoapCommon.Interfaces;

namespace GoapGraphConnector.SimpleGraph {
    /// <summary>
    ///     hold the information of the build graph in search process.
    /// </summary>
    internal class Map {
        private readonly List<IGoapNode> _vertices;
        private readonly List<IGoapEdge> _edges;

        
        public Map(IGoapNode rootNode) {
            _vertices = new List<IGoapNode> { rootNode };
            _edges = new List<IGoapEdge>() ;
        }

        /// <summary>
        ///     add an edge to the graph.
        ///     if source or target are missing add them to graph, too.
        /// </summary>
        /// <param name="edge"></param>
        public void AddEdge(IGoapEdge edge) {
            MaintainConsistence(edge);
            _edges.Add(edge);
        }

        /// <summary>
        ///     ensure that every source or target node of an inserted edge is in the graph.
        /// </summary>
        /// <param name="addedEdge"></param>
        private void MaintainConsistence(IGoapEdge addedEdge) {
            if (!ContainsVertex(addedEdge.GetSource())) {
                AddVertex(addedEdge.GetSource());
            }
            if (!ContainsVertex(addedEdge.GetTarget())) {
                AddVertex(addedEdge.GetTarget());
            }
        }

        /// <summary>
        ///     add the vertex if not already in the list
        /// </summary>
        /// <param name="vertex"></param>
        public void AddVertex(IGoapNode vertex) {
            if (!_vertices.Contains(vertex)) {
                _vertices.Add(vertex);
            }
        }

        /// <summary>
        ///     check if vertex list of graph contains vertex
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        private bool ContainsVertex(IGoapNode vertex) {
            return _vertices.Contains(vertex);
        }

        public override string ToString() {
            string text = _vertices.Aggregate("", (current, vertex) => current + (vertex + Environment.NewLine));

            text = text + _edges.Aggregate("", (current, edge) => current + (edge + Environment.NewLine));

            return text;
        }

        /// <summary>
        ///     graph may be a multigraph. so there may be parallel edges. 
        ///     get all edges by given source and target node.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private List<IGoapEdge> GetEdgesBySourceAndTarget(IGoapNode source, IGoapNode target) {
            if (!ContainsVertex(source) || !ContainsVertex(target)) {
                throw new GraphException("source or target of edge not available in graph");
            }

            List<IGoapEdge> bindingEdge = _edges.FindAll
                (e => e.GetSource().Equals(source) && e.GetTarget().Equals(target));
            if (bindingEdge == null) {
                throw new GraphException("Edge not found by target and source");
            }
            return bindingEdge;
        }

        public IGoapEdge GetCheapestEdgeBySourceAndTarget(IGoapNode source, IGoapNode target) {
            IOrderedEnumerable<IGoapEdge> query = GetEdgesBySourceAndTarget(source, target).OrderBy(e => e.GetCost());
            return query.First();
        }

        /// <summary>
        ///     graph may be a multigraph. so there may be parallel edges. 
        ///     get always the cheapest edge by given source and target node. 
        /// </summary>
        /// <param name="sourceVertex"></param>
        /// <param name="targetVertex"></param>
        /// <returns></returns>
        public int GetcheapestWayCost(IGoapNode sourceVertex, IGoapNode targetVertex) {
            IGoapEdge edge = GetCheapestEdgeBySourceAndTarget(sourceVertex, targetVertex);
            return edge.GetCost();
        }

        /// <summary>
        ///     get the adjacent vertices by outgoing edges of given vertex
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public List<IGoapNode> GetReachableAdjcentVertices(IGoapNode vertex) {
            List<IGoapEdge> outEdges = GetEdgesBySourceVertex(vertex);

            return outEdges.Select(outEdge => outEdge.GetTarget()).ToList();
        }

        /// <summary>
        ///     get the positiv incident edges by vertex (source of edge is vertex)
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        private List<IGoapEdge> GetEdgesBySourceVertex(IGoapNode vertex) {
            return _edges.Where(edge => edge.GetSource().Equals(vertex)).ToList();
        }
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using GoapCommon.Exceptions;
using GoapCommon.Interfaces;

namespace GoapGraphConnector.CustomGraph
{
    public class Graph 
    {
        private List<IGoapVertex> _vertices;
        private List<IGoapEdge> _edges;
        /*
        private Dictionary<IGoapVertex, List<IGoapVertex>> _vertexToVertex = new Dictionary<IGoapVertex, List<IGoapVertex>>();
        private Dictionary<IGoapVertex, List<IGoapEdge>> _vertexOutEdges = new Dictionary<IGoapVertex, List<IGoapEdge>>(); 
        */
        public Graph(List<IGoapVertex> vertices, List<IGoapEdge> edges) {
            _vertices = vertices;
            _edges = edges;
        }

        public void AddEdge(IGoapEdge edge) {
            _edges.Add(edge);
            
        }

        public void AddVertex(IGoapVertex vertex) {
            _vertices.Add(vertex);
        }

        public bool IsEmpty() {
            if (_vertices == null) return true;
            return _vertices.Count == 0;
        }

        public override string ToString() {

            var text = _vertices.Aggregate("", (current, vertex) => current + (vertex + System.Environment.NewLine));

            text = text + _edges.Aggregate("", (current, edge) => current + (edge + System.Environment.NewLine));


            return text;
        }


        /*
        public void AddVertexList(List<IGoapVertex> vertices) {
            foreach (var newVertex in vertices) {
                AddKeyToVertexToVertex(newVertex);
                AddKeyToVertexOutEdges(newVertex);
            }
        }
        
        public Graph AddEdgeList(List<IGoapEdge> edges ) {
            foreach (var newEdge in edges) {
                if (!_vertexToVertex.ContainsKey(newEdge.GetSource())) {
                    
                }
            }
        }

        private void AddKeyToVertexToVertex(IGoapVertex vertex) {
            if (!_vertexToVertex.ContainsKey(vertex)){
                _vertexToVertex.Add(vertex, new List<IGoapVertex>());
            }
        }

        private void AddKeyToVertexOutEdges(IGoapVertex vertex) {
            if (!_vertexOutEdges.ContainsKey(vertex)){
                _vertexOutEdges.Add(vertex, new List<IGoapEdge>());
            }
        }

        private void AddValueToVertexToVertex(IGoapVertex keyVertex, IGoapVertex valueVertex) {
            AddKeyToVertexToVertex(keyVertex);
            List<IGoapVertex> valueOfKeyVertex;
            _vertexToVertex.TryGetValue(keyVertex, out valueOfKeyVertex);
            if (valueOfKeyVertex != null && !valueOfKeyVertex.Contains(valueVertex)) {
                valueOfKeyVertex.Add(valueVertex);
                _vertexToVertex.Add(keyVertex, valueOfKeyVertex);
            }
        }

        private void AddValueToVertexOutEdges(IGoapEdge edge) {
            AddKeyToVertexOutEdges(edge.GetSource());
            List<IGoapEdge> valueOfKeyVertex;
            _vertexOutEdges.TryGetValue(edge.GetSource(), out valueOfKeyVertex);
            if (valueOfKeyVertex != null && !valueOfKeyVertex.Contains(edge)) {
                valueOfKeyVertex.Add(edge);
                _vertexOutEdges.Add(edge.GetSource(), valueOfKeyVertex);
            }
        }*/




        public int GetWayCost(IGoapVertex current, IGoapVertex openVertex)
        {
            if (_vertices.Contains(current) && _vertices.Contains(openVertex)) {
                IGoapEdge bindingEdge = _edges.Find(e => e.GetSource().Equals(current) && e.GetTarget().Equals(openVertex));
                return bindingEdge.GetCost();
            }
            throw new GraphException("questioned edge is available in graph");
        }

        public List<IGoapVertex> GetReachableAdjcentVertices(IGoapVertex vertex) {
            List<IGoapEdge> outEdges = GetPositiveIncidentEdges(vertex);

            return outEdges.Select(outEdge => outEdge.GetTarget()).ToList();
        }

        /// <summary>
        /// get the positiv incident edges by vertex (source of edge is vertex)
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        private List<IGoapEdge> GetPositiveIncidentEdges(IGoapVertex vertex) {
           return _edges.Where(edge => edge.GetSource().Equals(vertex)).ToList();
        }
    }
}

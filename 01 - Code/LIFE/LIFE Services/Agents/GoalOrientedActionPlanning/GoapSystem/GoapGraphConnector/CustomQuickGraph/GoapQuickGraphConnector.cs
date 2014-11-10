using System;
using System.Collections.Generic;
using GoapCommon.Interfaces;
using QuickGraph;

namespace GoapGraphConnector.CustomQuickGraph {

    /// <summary>
    /// concrete class for usage with the planner.  class wraps all knowledge about the graph presentation through the quickgraph 
    /// library (http://quickgraph.codeplex.com/) . So the library is exchangeable with other technologies for representing the graph
    /// for the search.
    /// </summary>
    public class GoapQuickGraphConnector : IGoapGraph {
        private AdjacencyGraph<GoapQuickGraphVertex, GoapQuickGraphEdge<GoapQuickGraphVertex>> _quickGraph;
        private GoapQuickGraphVertex _rootVertex;
        private GoapQuickGraphVertex _targetVertex;
        private int _maximumGraphDepth;

       
        public void InitializeGoapGraph(List<IGoapWorldstate> rootState, List<IGoapWorldstate> targetState,
            int maximumGraphDept = 0) {
            _rootVertex = new GoapQuickGraphVertex(rootState);
            _targetVertex = new GoapQuickGraphVertex(targetState);
            _maximumGraphDepth = maximumGraphDept;
            _quickGraph = new AdjacencyGraph<GoapQuickGraphVertex, GoapQuickGraphEdge<GoapQuickGraphVertex>>();
            _quickGraph.AddVertex(_targetVertex);
        }

        public bool IsGraphEmpty() {
            return _quickGraph.IsVerticesEmpty;
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

        public int GetActualDepthFromRoot() {
            throw new NotImplementedException();
        }

        public List<IGoapAction> GetShortestPath() {
            throw new NotImplementedException();
        }

        /*
        public static void Main(string[] args) {

            
            /*
            var g = new AdjacencyGraph<GoapQuickGraphVertex, Edge<GoapQuickGraphVertex>>();

            var vA1 = new GoapQuickGraphVertex(new List<IGoapWorldstate>(), "A1");
            var vB = new GoapQuickGraphVertex(new List<IGoapWorldstate>(), "B");
            var vA2 = new GoapQuickGraphVertex(new List<IGoapWorldstate>(), "A2");

            //Console.WriteLine(vA1.Equals(vA2));

            var a = new HashSet<int>();
            var b = new HashSet<int>();

            Console.WriteLine(a.Equals(b));
            Console.WriteLine(a == b);

            /*
            g.AddVertex(vA1);
            //g.AddVertex(vA2);
            g.AddVertex(vB);
            g.AddEdge(new Edge<GoapQuickGraphVertex>(vB, vA1));
            //g.AddEdge(new Edge<GoapQuickGraphVertex>(vB, vA2));

            var astar = new AStarShortestPathAlgorithm<GoapQuickGraphVertex, Edge<GoapQuickGraphVertex>>(g, e => 1, i => 0);

            //astar.SetRootVertex(vB);
                
            

            TryFunc<GoapQuickGraphVertex, IEnumerable<Edge<GoapQuickGraphVertex>>> tryGetPath = g.ShortestPathsAStar(e => 1, i => 0, vB);
            //TryFunc<GoapQuickGraphVertex, IEnumerable<Edge<GoapQuickGraphVertex>>> tryGetPath = g.ShortestPathsDijkstra(e => 1, vB);
            
            IEnumerable<Edge<GoapQuickGraphVertex>> path;
            var target = vA2;
            if (tryGetPath(target, out path))
            {
                foreach (var e in path)
                {
                    Console.WriteLine(e.Source + " -> " + e.Target);
                    
                }
            }
            Console.WriteLine("------------------------------------");
            Console.WriteLine(g.ContainsVertex(vA1));
            Console.WriteLine(g.ContainsEdge(vB,vA1));
            

            

            
            Console.ReadKey();
            
            var v1 = new GoapQuickGraphVertex("V1");
            var v2 = new GoapQuickGraphVertex("V2");
            var v3 = new GoapQuickGraphVertex("V3");
            var v4 = new GoapQuickGraphVertex("V4");
            var v5 = new GoapQuickGraphVertex("V5");


            var e1 = new GoapQuickGraphEdge<GoapQuickGraphVertex>(v1, v2, new ActionA(), 1);
            var e2 = new GoapQuickGraphEdge<GoapQuickGraphVertex>(v3, v2, new ActionB(), 2);
            var e3 = new GoapQuickGraphEdge<GoapQuickGraphVertex>(v2, v4, new ActionC(), 1);
            var e4 = new GoapQuickGraphEdge<GoapQuickGraphVertex>(v3, v4, new ActionA(), 4);
            var e5 = new GoapQuickGraphEdge<GoapQuickGraphVertex>(v4, v5, new ActionB(), 1);

            var graph = new AdjacencyGraph<GoapQuickGraphVertex, GoapQuickGraphEdge<GoapQuickGraphVertex>>();
            /*
            graph.AddVertexRange(new[] {v1, v2, v3, v4, v5});
            graph.AddEdgeRange(new[] {e1, e2, e3, e4, e5});

            //IVertexAndEdgeListGraph<object, MyEdge<object>> citty = graph;


            //Func<MyEdge<object>, double> edgeCost = e => e.GetHeuristic();
            Func<MyEdge<MyVertex>, double> edgeCost = e => 1;
            var root = v3;
            var target = v5;

            //TryFunc<object, IEnumerable<MyEdge<object>>> tryGetPaths = citty.ShortestPathsDijkstra(edgeCost, root);
            //TryFunc<object, IEnumerable<MyEdge<object>>> tryGetPaths = graph.ShortestPathsDijkstra(edgeCost, root);

            var algo = new DijkstraShortestPathAlgorithm<MyVertex, MyEdge<MyVertex>>(graph, edgeCost);

            //var algo2 = new QuickGraph.Algorithms.
            //algo.Compute();

            /*
            IEnumerable<MyEdge<MyVertex>> path;

            if (tryGetPaths(target, out path)) {
                foreach (var edge in path) {
                    var node = (MyVertex) edge.Target;
                    Console.WriteLine(node.Name);
                }
            }
            

            Console.ReadKey();

            

            IVertexAndEdgeListGraph<TVertex, TEdge> graph = ...;
            Func<TEdge, double> edgeCost = e => 1; // constant cost
            TVertex root = ...;

            // compute shortest paths
            TryFunc<TVertex, TEdge> tryGetPaths = graph.ShortestPathDijkstra(edgeCost, root);

            // query path for given vertices
            TVertex target = ...;
            IEnumerable<TEdge> path;
            if (tryGetPaths(target, out path))
                foreach(var edge in path)
                    Console.WriteLine(edge);
            
        }*/
    
    }
}
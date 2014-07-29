using System;
using System.Collections.Generic;
using GoapCommon.Interfaces;
using QuickGraph;
using QuickGraph.Algorithms;

namespace GoapGraphConnector.CustomQuickGraph
{
    public class GoapQuickGraphConnector : IGoapGraph
    {

        public IGoapGraph CreateGoapGraph(List<IGoapWorldstate> rootState, List<IGoapWorldstate> targetState, int maximumGraphDept = 0)
        {
            //QuickGraph
            throw new NotImplementedException();


        }

        public bool IsGraphEmpty()
        {
            throw new NotImplementedException();
        }

        public IGoapVertex GetNextVertexFromOpenList()
        {
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

        public bool AStarStep()
        {
            throw new NotImplementedException();
        }

        public int GetActualDepthFromRoot()
        {
            throw new NotImplementedException();
        }

        public List<IGoapAction> GetShortestPath()
        {
            throw new NotImplementedException();
        }

        public static void Main(string[] args)
        {





            var g = new AdjacencyGraph<int, Edge<int>>();

            int v1 = 1;
            int v2 = 2;

            g.AddVertex(1);
            g.AddVertex(2);
            g.AddEdge(new Edge<int>(v1, v2));


            TryFunc<int, IEnumerable<Edge<int>>> tryGetPath = g.ShortestPathsAStar(e => 1, i => 1, v1);
            IEnumerable<Edge<int>> path;
            var target = 1;
            if (tryGetPath(target, out path))
            {

                foreach (var e in path)
                {
                    Console.WriteLine(e.Target);
                }
            }
            Console.WriteLine(path);
            Console.ReadKey();
            /*
            var v1 = new GoapVertex("V1");
            var v2 = new GoapVertex("V2");
            var v3 = new GoapVertex("V3");
            var v4 = new GoapVertex("V4");
            var v5 = new GoapVertex("V5");


            var e1 = new GoapEdge<GoapVertex>(v1, v2, new ActionA(), 1);
            var e2 = new GoapEdge<GoapVertex>(v3, v2, new ActionB(), 2);
            var e3 = new GoapEdge<GoapVertex>(v2, v4, new ActionC(), 1);
            var e4 = new GoapEdge<GoapVertex>(v3, v4, new ActionA(), 4);
            var e5 = new GoapEdge<GoapVertex>(v4, v5, new ActionB(), 1);

            var graph = new AdjacencyGraph<GoapVertex, GoapEdge<GoapVertex>>();
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
            */
        }
    }
}
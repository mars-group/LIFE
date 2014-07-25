using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CommonTypes.Interfaces;
using GoapActionSystem.Implementation;
using GoapCommon.Interfaces;
using GoapGraphConnector;
using GoapGraphConnector.CustomQuickGraph;
using GoapUser.Actions;
using GoapUser.Worldstates;
using QuickGraph;
using QuickGraph.Algorithms.ShortestPath;

namespace GoapUser {

    internal class MyEdge<TVertex> : Edge<TVertex>
    {
        public MyEdge(TVertex source, TVertex target, IAction action, int heuristic) : base(source, target) {
            Action = action;
            Heuristic = heuristic;
        }

        private IAction Action { get; set; }
        private int Heuristic { get; set; }
        
        public int GetHeuristic(List<IGoapWorldstate> targetWorldstates = null) {
            return Heuristic;
        }
    }


    internal class MyVertex {
        private readonly string _name;

        public MyVertex(string name) {
            _name = name;
        }

        public string Name {
            get { return _name; }
        }
    }


    internal static class Program {

        

        private static void Main(string[] args) {

     
       
            Happy h1 =  new Happy(true, WorldStateEnums.Happy);
            Happy h2 =  new Happy(true, WorldStateEnums.Happy);
            Happy h3 =  new Happy(false, WorldStateEnums.Happy);

            /*
            Console.WriteLine(h1.Equals(h1));
            Console.WriteLine(h1.Equals(null));
            Console.WriteLine(h1.Equals(h2));
            Console.WriteLine(h1.Equals(h3));
            */

            ActionPlay aplay = new ActionPlay();

            /*
            IWorldstate toytrue = new HasToy(true, WorldStateEnums.HasToy);
            
            HasToy toyfalse = new HasToy(false, WorldStateEnums.HasToy);
           

            List<IWorldstate> states = aplay.GetResultingWorldstate(new List<IWorldstate>() { h3, toytrue });

            foreach (var worldstate in states) {
                Console.WriteLine(worldstate.ToString() + ":  " + worldstate.IsValid());
                
            }*/


            AbstractGoapAction aClean = new ActionClean();
            AbstractGoapAction aGetToy = new ActionGetToy();

            Console.WriteLine("PRE");
            aClean.PreConditions.ForEach(ws => Console.WriteLine(ws));
            Console.WriteLine("EFFECT");
            aClean.Effects.ForEach(ws => Console.WriteLine(ws));


            Console.WriteLine("-------------------");

            Console.WriteLine("PRE");
            aGetToy.PreConditions.ForEach(ws => Console.WriteLine(ws));
            Console.WriteLine("EFFECT");
            aGetToy.Effects.ForEach(ws => Console.WriteLine(ws));

            Console.WriteLine("-------------------");

            Console.WriteLine("PRE");
            aplay.PreConditions.ForEach(ws => Console.WriteLine(ws));
            Console.WriteLine("EFFECT");
            aplay.Effects.ForEach(ws => Console.WriteLine(ws));

            Console.ReadKey();




            /*
            var connector =  new GoapQuickGraphConnector();

            IGoapGraph graph =  connector.CreateGoapGraph(IWorldstate rootState, IWorldstate targetState, List<IAction> allActions, 5)
        


























            /*
            var v1 = new MyVertex("V1");
            var v2 = new MyVertex("V2");
            var v3 = new MyVertex("V3");
            var v4 = new MyVertex("V4");
            var v5 = new MyVertex("V5");


            var e1 = new MyEdge<MyVertex>(v1, v2, new ActionA(), 1);
            var e2 = new MyEdge<MyVertex>(v3, v2, new ActionB(), 2);
            var e3 = new MyEdge<MyVertex>(v2, v4, new ActionC(), 1);
            var e4 = new MyEdge<MyVertex>(v3, v4, new ActionA(), 4);
            var e5 = new MyEdge<MyVertex>(v4, v5, new ActionB(), 1);

            var graph = new AdjacencyGraph<MyVertex, MyEdge<MyVertex>>();

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
            

            //Console.ReadKey();

            

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
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using CommonTypes.Interfaces;
using GoapActionSystemFactory.Implementation;
using GoapCommon.Interfaces;
using QuickGraph;

namespace GoapUser {
    internal class MyEdge<TVertex> : Edge<TVertex> {
        public MyEdge(TVertex source, TVertex target, IAction action, int heuristic)
            : base(source, target) {
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
            /*
            
            IGoapWorldstate moneytrue = new HasMoney(true, WorldStateEnums.HasMoney);
            IGoapWorldstate moneyfalse = new HasMoney(false, WorldStateEnums.HasMoney);

            IGoapWorldstate happytrue = new Happy(true, WorldStateEnums.Happy);
            IGoapWorldstate happyfalse = new Happy(false, WorldStateEnums.Happy);

            IGoapWorldstate toytrue = new HasToy(true, WorldStateEnums.HasToy);
            IGoapWorldstate toyfalse = new HasToy(false, WorldStateEnums.HasToy);


            
            var v1 = new Vertex(new List<IGoapWorldstate> {moneyfalse, happyfalse, toyfalse}, 45,"v1");
            var v2 = new Vertex(new List<IGoapWorldstate> { moneyfalse, happyfalse, toytrue }, 13, "v2");
            var v3 = new Vertex(new List<IGoapWorldstate> { moneyfalse, happytrue, toyfalse }, 11, "v3");
            var v4 = new Vertex(new List<IGoapWorldstate> { moneyfalse, happytrue, toytrue }, 8, "v4");
            var v5 = new Vertex(new List<IGoapWorldstate> { moneytrue, happyfalse, toyfalse }, 10, "v5");
            var v6 = new Vertex(new List<IGoapWorldstate> { moneytrue, happyfalse, toytrue }, 31, "v6");
            var v7 = new Vertex(new List<IGoapWorldstate> { moneytrue, happytrue, toyfalse }, 22, "v7");
            var v8 = new Vertex(new List<IGoapWorldstate> { moneytrue, happytrue, toytrue }, 0, "v8");


            var e1 = new Edge(18, v1, v6);
            var e1b = new Edge(18, v6, v1);

            var e2 = new Edge(16, v6, v7);
            var e2b = new Edge(16, v7, v6);

            var e3 = new Edge(18, v7, v5);
            var e3b = new Edge(18, v5, v7);

            var e4 = new Edge(55, v1, v2);
            var e4b = new Edge(55, v2, v1);

            var e5 = new Edge(33, v6, v2);
            var e5b = new Edge(33, v2, v6);

            var e6 = new Edge(12, v2, v7);
            var e6b = new Edge(12, v7, v2);
            
            var e7 = new Edge(40, v1, v3);
            var e7b = new Edge(40, v3, v1);

            var e8 = new Edge(5, v3, v2);
            var e8b = new Edge(5, v2, v3);

            var e9 = new Edge(7, v2, v5);
            var e9b = new Edge(7, v5, v2);

            var e10 = new Edge(13, v5, v8);
            var e10b = new Edge(13, v8, v5);

            var e11 = new Edge(5, v3, v4);
            var e11b = new Edge(5, v4, v3);

            var e12 = new Edge(15, v4, v8);
            var e12b = new Edge(15, v8, v4);

            var e13 = new Edge(6, v4, v5);
            var e13b = new Edge(6, v5, v4);


            var conn = new GoapCustomGraphService();
            
            
            // ersten knoten eintragen mit zugehörigen werten und als aktuellen markieren
            conn.InitializeGoapGraph(v1,v8);
            var vertex = conn.GetNextVertexFromOpenList(); 
            Console.WriteLine(vertex);
            Console.WriteLine(conn.IsCurrentVertexTarget());

            
            // v1->v6, v1->v2, ,v1->v3, 
            conn.ExpandCurrentVertex(new List<IGoapEdge>{ e1, e4, e7 });
            
            conn.AStarStep();
            var vertex1 = conn.GetNextVertexFromOpenList();
            Console.WriteLine(vertex1);
            Console.WriteLine(conn.IsCurrentVertexTarget());


            // v6->v1, v6->v2, v6->v7  
            conn.ExpandCurrentVertex(new List<IGoapEdge>{ e1b, e5, e2 });
            conn.AStarStep();
            var vertex2 = conn.GetNextVertexFromOpenList();
            Console.WriteLine(vertex2);
            Console.WriteLine(conn.IsCurrentVertexTarget());
            
            // v3->v1, v3->v2, v3->v4
            conn.ExpandCurrentVertex(new List<IGoapEdge>{ e7b, e8, e11 });
            conn.AStarStep();
            var vertex3 = conn.GetNextVertexFromOpenList();
            Console.WriteLine(vertex3);
            Console.WriteLine(conn.IsCurrentVertexTarget());
           
            
            // v4->v3, v4->v5, v4->v8
            conn.ExpandCurrentVertex(new List<IGoapEdge> { e11b, e13, e12 });
            conn.AStarStep();
            var vertex4 = conn.GetNextVertexFromOpenList();
            Console.WriteLine(vertex4);
            Console.WriteLine(conn.IsCurrentVertexTarget());
            
            
            // v7->v6, v7->v2, v7->v5
            conn.ExpandCurrentVertex(new List<IGoapEdge> { e2b, e6b, e3 });
            conn.AStarStep();
            var vertex5 = conn.GetNextVertexFromOpenList();
            Console.WriteLine(vertex5);
            Console.WriteLine(conn.IsCurrentVertexTarget());
            

            // v2->v1, v2->v6, v2->v7, v2->v5, v2->v3
            conn.ExpandCurrentVertex(new List<IGoapEdge> { e4b, e5b, e6, e9, e8b});
            conn.AStarStep();
            var vertex6 = conn.GetNextVertexFromOpenList();
            Console.WriteLine(vertex6);
            Console.WriteLine(conn.IsCurrentVertexTarget());

            
            var edgesList = conn.GetEdgesList();
            
            foreach (var edge in edgesList) {
                Console.WriteLine(edge.GetSource().GetIdentifier() + " -> " + edge.GetTarget().GetIdentifier());
            }


          
            var l1 = new Happy(true, WorldStateEnums.Happy);
            var l2 = new Happy(true, WorldStateEnums.Happy);

       
            var l4 = new Vertex(new List<IGoapWorldstate> {new HasToy(true, WorldStateEnums.HasToy), new Happy(true, WorldStateEnums.Happy)});
            var l5 = new Vertex(new List<IGoapWorldstate> {new Happy(true, WorldStateEnums.Happy),new HasToy(true, WorldStateEnums.HasToy)});

            var l6 = new Vertex(new List<IGoapWorldstate>());
            var l7 = new Vertex(new List<IGoapWorldstate>());
         
           Console.WriteLine(l4.Equals(l5));
           Console.WriteLine(l6.Equals(l7));

             */
            Console.WriteLine("-----------------------------------");



           

            IActionSystem goapActionSystem = GoapComponent.LoadAgentConfiguration("AgentConfig1", "GoapModelTest");


            Console.WriteLine(goapActionSystem.GetNextAction().GetType());

         


            Console.ReadKey();


        }
    }
}
using System;
using System.Collections.Generic;
using CommonTypes.Interfaces;
using GoapActionSystem.Implementation;
using GoapCommon.Interfaces;
using GoapUser.Actions;
using GoapUser.Worldstates;
using QuickGraph;

namespace GoapUser
{
    internal class MyEdge<TVertex> : Edge<TVertex>
    {
        public MyEdge(TVertex source, TVertex target, IAction action, int heuristic)
            : base(source, target)
        {
            Action = action;
            Heuristic = heuristic;
        }

        private IAction Action { get; set; }
        private int Heuristic { get; set; }

        public int GetHeuristic(List<IGoapWorldstate> targetWorldstates = null)
        {
            return Heuristic;
        }
    }


    internal class MyVertex
    {
        private readonly string _name;

        public MyVertex(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }


    internal static class Program
    {
        private static void Main(string[] args)
        {
            AbstractGoapAction clean = new ActionClean();
            AbstractGoapAction getToy = new ActionGetToy();
            AbstractGoapAction play = new ActionPlay();
            var actionList = new List<IGoapAction> { clean, clean, play };

            GoapPlanner planner = new GoapPlanner(10, actionList);

            List<IGoapWorldstate> currentWorld = new List<IGoapWorldstate> { new HasToy(true, WorldStateEnums.HasToy) };
            List<IGoapWorldstate> targetWorld = new List<IGoapWorldstate> {
                new Happy(true, WorldStateEnums.Happy),
                new HasToy(false, WorldStateEnums.HasToy)
            };

            planner.GetPlan(currentWorld, targetWorld);

            Console.WriteLine(planner);

            
            Console.ReadKey();


            /*
            var connector =  new GoapQuickGraphConnector();

            IGoapGraph graph =  connector.CreateGoapGraph(IWorldstate rootState, IWorldstate targetState, List<IAction> allActions, 5)
        
            */



        }
    }
}
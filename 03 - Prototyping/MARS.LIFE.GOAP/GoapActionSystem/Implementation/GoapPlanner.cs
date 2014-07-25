using System;
using System.Collections.Generic;
using CommonTypes.Interfaces;
using GoapActionSystem.Interfaces;
using GoapGraphConnector;
using GoapGraphConnector.CustomQuickGraph;


namespace GoapActionSystem.Implementation
{
    public class GoapPlanner : IPlanner
    {

        // Die Map, die die Suche nach passendan Actions erleichtert


        public List<CommonTypes.Interfaces.IAction> GetPlan(List<CommonTypes.Interfaces.IAction> availableActions, List<IGoapWorldstate> currentWorld, List<IGoapWorldstate> targetWorld) {
            
            IGoapGraph graph = InitializeGraph(currentWorld, targetWorld);



            while (!graph.IsTargetFound() && !graph.IsOpenListEmpty()) {

                IGoapVertex currentWorldStates = graph.GetNextVertexOnWhiteList();
                List<AbstractGoapAction> outgoingGoapActions = GetOutgoinGoapActions(currentWorldStates);
                graph.
                graph.Step();




               
                
                
            }


            throw new NotImplementedException();
        }

        private IGoapGraph InitializeGraph(List<IGoapWorldstate> currentWorld, List<IGoapWorldstate> targetWorld)
        {
            GoapQuickGraphConnector connector = new GoapQuickGraphConnector();
            IGoapGraph startGraph = connector.CreateGoapGraph(currentWorld, targetWorld);
        }

        private List<AbstractGoapAction> GetOutgoinGoapActions(IGoapVertex currentWorldStates)
        {
            throw new NotImplementedException();
        }

   
    }
}

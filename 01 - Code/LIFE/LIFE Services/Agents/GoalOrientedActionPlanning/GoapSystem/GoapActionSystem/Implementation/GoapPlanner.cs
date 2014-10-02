using System;
using System.Collections.Generic;
using System.Linq;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;
using GoapGraphConnector.SimpleGraph;

namespace GoapActionSystem.Implementation {
    /// <summary>
    ///     The goapplanner is responsible for the process of finding a valid plan from the actions, currentWorld and
    ///     targetWorld given to him. He uses the graph component for creating a new plan.
    ///     the planner saves the plan and check it for validity depending on the events and state of simulation.
    ///     The caller is responsible for giving well defined and corresponding actions and world states.
    ///     planner is responsible for the regressive search. the graph component knows nothing about that and the switched
    ///     root and target.
    /// </summary>
    public class GoapPlanner : IGoapPlanner {
        private readonly int _maximuxSearchDepth = int.MaxValue;
        private List<AbstractGoapAction> _currentPlan;
        private readonly List<AbstractGoapAction> _availableActions;

        /// <summary>
        /// </summary>
        /// <param name="maximuxSearchDepth"></param>
        /// <param name="availableActions"></param>
        public GoapPlanner(int maximuxSearchDepth, List<AbstractGoapAction> availableActions) {
            if (availableActions.Count == 0)
                throw new ArgumentException("Planner may not be instanciated with an empty list of actions");
            _maximuxSearchDepth = maximuxSearchDepth;
            _availableActions = availableActions;
        }

        /// <summary>
        ///     create the start graph, open list, closed list
        ///     note regressive seach so root node will be the targeted worldstate
        /// </summary>
        /// <param name="graphRoot"></param>
        /// <param name="graphTarget"></param>
        /// <returns></returns>
        private IGoapGraphService InitializeGraphService(List<IGoapWorldProperty> graphRoot,
            List<IGoapWorldProperty> graphTarget) {
            //var graphService = new GoapCustomGraphService();
            var graphService = new GoapSimpleGraphService();
            graphService.InitializeGoapGraph(graphRoot, graphTarget);
            return graphService;
        }

        private bool IsCurrentVertexSubsetOfTarget(List<IGoapWorldProperty> currentWorld, List<IGoapWorldProperty> targetWorld) {
            return (currentWorld.Where(x => targetWorld.Contains(x)).Count() == currentWorld.Count());
        }

        private bool IsSearchDepthLimitExceeded(IGoapGraphService graphService) {
            return graphService.GetActualDepthFromRoot() > _maximuxSearchDepth;
        }

        private bool HasPlan() {
            return (_currentPlan.Count > 0);
        }

        public AbstractGoapAction GetNextChosenAction() {
            if (HasPlan()) return _currentPlan.First();
            return new SurrogateAction();
        }

        public List<AbstractGoapAction> GetPlan(List<IGoapWorldProperty> currentWorld,
            List<IGoapWorldProperty> targetWorld) {
            List<IGoapWorldProperty> graphTarget = currentWorld;
            List<IGoapWorldProperty> graphRoot = targetWorld;

            IGoapGraphService graphService = InitializeGraphService(graphRoot, graphTarget);


            /* 1 graph erstellen (nur erster Knoten) und diesen in die open list
             2 step durch algorithmus : besten aus open list raussuchen und 
                 kostentabelle pflegen 
             
                 knotenname
                 Vorgänger
                 h (bisherige Wegsumme + heuristischer Wert für den knoten)
                 g (bisherige Wegsumme)
                 f (heuristischer Wert für den knoten)
                 CL flag ob Knoten in closed list
             
             3 kinder: anfordern durch planner 
             4 falls neue kinder dazu kamen - tabelle erweitern
             goto 2
             
             wenn kein weg  mehr günstiger ist als der bisherige weg zum ziel fertig
             */
            IGoapNode currentGoapNode = graphService.GetNextVertexFromOpenList();

            while (currentGoapNode != null &&
                   !IsCurrentVertexSubsetOfTarget(currentGoapNode.Worldstate(), graphTarget) &&
                   !IsSearchDepthLimitExceeded(graphService)) {
                List<AbstractGoapAction> children = GetIngoingGoapActions(currentGoapNode.Worldstate());
                graphService.ExpandCurrentVertex(children, currentGoapNode.Worldstate());
                graphService.AStarStep();
                currentGoapNode = graphService.GetNextVertexFromOpenList();
            }

            if (currentGoapNode != null && IsCurrentVertexSubsetOfTarget(currentGoapNode.Worldstate(), graphTarget)) {
                _currentPlan = graphService.GetShortestPath();
                _currentPlan.Reverse();
            }
            if (graphService.GetActualDepthFromRoot() >= _maximuxSearchDepth || !graphService.HasNextVertexOnOpenList())
                _currentPlan = new List<AbstractGoapAction> {new SurrogateAction()};

            // TODO ist die leere action besser als eine leere liste ?
            
            return _currentPlan;
        }


        /// <summary>
        ///     search for actions that effects correxpond to the state
        /// </summary>
        /// <param name="worldStates"></param>
        /// <returns></returns>
        private List<AbstractGoapAction> GetIngoingGoapActions(List<IGoapWorldProperty> worldStates) {
            // TODO alle actions untersuchen ob sie anwendbar sind - mithilfe der effect -> Action hashmap
            return _availableActions.Where(action => action.IsSatisfyingStateByEffects(worldStates)).ToList();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;
using GoapGraphConnector.CustomGraph;

namespace GoapActionSystem.Implementation {
    /// <summary>
    ///     The goapplanner is responsible for the process of finding a valid plan from the actions, currentWorld and
    ///     targetWorld given to him. He uses the grpah component for creating a new plan.
    ///     the planner saves the plan and check it for validity depending on the events and state of simulation.
    ///     The caller is responsible for giving well defined and corresponding actions and world states.
    /// </summary>
    public class GoapPlanner : IGoapPlanner {
        /// <summary>
        ///     needed for not running in an too huge graph / tree
        /// </summary>
        private readonly int _maximuxSearchDepth = int.MaxValue;

        /// <summary>
        ///     save the current plan in class if available
        /// </summary>
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

        public AbstractGoapAction GetNextChosenAction() {
            if (HasPlan()) return _currentPlan.First();
            return new SurrogateAction();
        }

        private bool HasPlan() {
            return (_currentPlan.Count > 0);
        }

        // TODO Die Map, die die Suche nach passendan Actions erleichtert

        public List<AbstractGoapAction> GetPlan(List<IGoapWorldstate> currentWorld,
            List<IGoapWorldstate> targetWorld) {
            // !! only the planner needs to know that goap is running with regressive search!!!
            IGoapGraph graph = InitializeGraph(targetWorld, currentWorld);

            /*
             1 grap erstellen (nur erster Knoten) und diesen in die open list
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

            while (!IsTargetReached(targetWorld, graph.GetNextVertexFromOpenList().Worldstate()) &&
                   !graph.IsCurrentVertexTarget() && graph.GetActualDepthFromRoot() < _maximuxSearchDepth) {
                IGoapVertex current = graph.GetNextVertexFromOpenList();
                List<AbstractGoapAction> children = GetIngoinGoapActions(current.Worldstate());
                graph.ExpandCurrentVertex(children, current.Worldstate());
                graph.AStarStep();
            }

            if (IsTargetReached(targetWorld, graph.GetNextVertexFromOpenList().Worldstate()))
            {
                _currentPlan = graph.GetShortestPath();
                _currentPlan.Reverse();
            }
            if (graph.GetActualDepthFromRoot() >= _maximuxSearchDepth || !graph.HasNextVertexOnOpenList())
                _currentPlan = new List<AbstractGoapAction> {new SurrogateAction()};

            // TODO ist die leere action besser als eine leere liste
            return _currentPlan;
        }

        /// <summary>
        ///     true if target is subset of current
        /// </summary>
        /// <param name="target"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        private bool IsTargetReached(List<IGoapWorldstate> target, List<IGoapWorldstate> current) {
            return target.Where(current.Contains).Count() == target.Count();
        }

        /// <summary>
        ///     create the start graph, open list, closed list
        /// </summary>
        /// <param name="currentWorld"></param>
        /// <param name="targetWorld"></param>
        /// <returns></returns>
        private IGoapGraph InitializeGraph(List<IGoapWorldstate> currentWorld, List<IGoapWorldstate> targetWorld) {
            var connector = new GoapCustomGraphService();
            connector.InitializeGoapGraph(targetWorld, currentWorld);
            return connector;
        }

        /// <summary>
        ///     search for actions corresponding to worldstate
        /// </summary>
        /// <param name="worldState"></param>
        /// <returns></returns>
        private List<AbstractGoapAction> GetIngoinGoapActions(List<IGoapWorldstate> worldState) {
            // TODO alle actions untersuchen ob sie anwendbar sind - mithilfe der effect -> Action hashmap
            return _availableActions.Where(action => action.IsEffectCorrespondingToState(worldState)).ToList();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using CommonTypes.Interfaces;
using GoapCommon.Interfaces;


namespace GoapActionSystem.Implementation
{
    /// <summary>
    ///     The goapplanner is responsible for the process of finding a valid plan from the actions, currentWorld and
    ///     targetWorld
    ///     given to him. The caller is responsible for giving well defined and corresponding actions and world states.
    /// </summary>
    public class GoapPlanner : IPlanner
    {
        /// <summary>
        ///     needed for not running in an too huge graph / tree
        /// </summary>
        private readonly int _maximuxSearchDepth = int.MaxValue;

        /// <summary>
        ///     save the current plan in class if available
        /// </summary>
        private List<IGoapAction> _currentPlan;

        private List<IGoapAction> _availableActions;

        /// <summary>
        /// </summary>
        /// <param name="maximuxSearchDepth"></param>
        /// <param name="availableActions"></param>
        public GoapPlanner(int maximuxSearchDepth, List<IGoapAction> availableActions)
        {
            if (availableActions.Count == 0)
            {
                throw new ArgumentException("Planner may not be instanciated with an empty list of actions");
            }
            _maximuxSearchDepth = maximuxSearchDepth;
            _availableActions = availableActions;
        }

        public IAction GetNextChosenAction()
        {
            if (HasPlan()) return _currentPlan.First();
            return new SurrogateAction();
        }

        private bool HasPlan()
        {
            return (_currentPlan.Count > 0);
        }

        // TODO Die Map, die die Suche nach passendan Actions erleichtert

        public List<IGoapAction> GetPlan(List<IGoapWorldstate> currentWorld,
            List<IGoapWorldstate> targetWorld)
        {
            IGoapGraph graph = InitializeGraph(currentWorld, targetWorld);

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

            while (!graph.IsCurrentVertexTarget() && graph.HasNextVertexOnOpenList() &&
                   graph.GetActualDepthFromRoot() < _maximuxSearchDepth)
            {
                IGoapVertex current = graph.GetNextVertexFromOpenList();
                List<IGoapAction> children = GetOutgoinGoapActions(current);
                graph.ExpandCurrentVertex(children);
                graph.AStarStep();
            }

            if (graph.IsCurrentVertexTarget()) _currentPlan = graph.GetShortestPath();
            if (graph.GetActualDepthFromRoot() >= _maximuxSearchDepth || !graph.HasNextVertexOnOpenList())
                _currentPlan = new List<IGoapAction> { new SurrogateAction() };

            // TODO ist die leere action besser als eine leere liste
            return _currentPlan;
        }

        /// <summary>
        ///     create the start graph, open list, closed list
        /// </summary>
        /// <param name="currentWorld"></param>
        /// <param name="targetWorld"></param>
        /// <returns></returns>
        private IGoapGraph InitializeGraph(List<IGoapWorldstate> currentWorld, List<IGoapWorldstate> targetWorld)
        {
            var connector = new GoapGraphConnector.CustomGraph.GoapGraphService();
            connector.InitializeGoapGraph(currentWorld, targetWorld);
            return connector;
        }

        private List<IGoapAction> GetOutgoinGoapActions(IGoapVertex currentWorldStates)
        {
            // TODO alle actions untersuchen ob sie anwendbar sind - mithilfe der effect -> Action hashmap
            // TODO ?? kann man irgendwie sinnvol die context effecte testen?

            throw new NotImplementedException();
        }
    }
}
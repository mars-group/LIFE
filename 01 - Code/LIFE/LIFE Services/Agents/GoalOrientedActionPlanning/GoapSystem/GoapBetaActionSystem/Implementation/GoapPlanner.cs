using System;
using System.Collections.Generic;
using System.Linq;
using GoapBetaGraphConnector.SimpleGraph;
using GoapBetaCommon.Abstract;
using GoapBetaCommon.Interfaces;


namespace GoapBetaActionSystem.Implementation {

    /// <summary>
    ///     The goapplanner is responsible for the process of finding a valid plan from the actions, currentWorld and
    ///     targetWorld given to him. He uses the graph component for creating a new plan.
    ///     the planner saves the plan and check it for validity depending on the events and state of simulation.
    ///     The caller is responsible for giving well defined and corresponding actions and world states.
    ///     planner is responsible for the regressive search. the graph component knows nothing about that and the switched
    ///     root and target.  
    /// </summary>
    internal class GoapPlanner  {
        private readonly int _maximuxSearchDepth = int.MaxValue;
        private readonly List<AbstractGoapAction> _availableActions;
        private readonly Dictionary<IGoapWorldProperty, List<AbstractGoapAction>> _effectToAction;
        private readonly List<IGoapWorldProperty> _startState;
        private List<AbstractGoapAction> _currentPlan;

        /// <summary>
        /// </summary>
        /// <param name="maximuxSearchDepth"></param>
        /// <param name="availableActions"></param>
        /// <param name="effectToAction"></param>
        /// <param name="startState"></param>
        internal GoapPlanner
            (int maximuxSearchDepth,
                List<AbstractGoapAction> availableActions,
                Dictionary<IGoapWorldProperty, List<AbstractGoapAction>> effectToAction,
                List<IGoapWorldProperty> startState) {
            if (availableActions.Count == 0)
                throw new ArgumentException("Planner may not be instanciated with an empty list of actions");
            _effectToAction = effectToAction;
            _startState = startState;
            _maximuxSearchDepth = maximuxSearchDepth;
            _availableActions = availableActions;
        }

        

        private IGoapGraphService InitializeGraphService(List<IGoapWorldProperty> graphRoot) {
            GoapSimpleGraphService graphService = new GoapSimpleGraphService();
            graphService.InitializeGoapGraph(graphRoot);
            return graphService;
        }

        private bool IsSearchDepthLimitExceeded(IGoapGraphService graphService) {
            return graphService.GetActualDepthFromRoot() > _maximuxSearchDepth;
        }

        public List<AbstractGoapAction> GetPlan(IGoapGoal goal) {
            List<IGoapWorldProperty> graphRoot = goal.GetTargetWorldstates();

            IGoapGraphService graphService = InitializeGraphService(graphRoot);

            IGoapNode currentNode = graphService.GetNextVertexFromOpenList();

            while (!IsSearchDepthLimitExceeded(graphService)
                   && (currentNode.HasUnsatisfiedProperties() && !currentNode.CanBeSatisfiedByStartState(_startState))) {
                List<AbstractGoapAction> satisfyingActions = GetActionsByUnsatisfiedProperties
                    (currentNode.GetUnsatisfiedGoalValues());
                List<AbstractGoapAction> applicableActions = FilterActionsByContextPreconditions(satisfyingActions);

                graphService.ExpandCurrentVertex(applicableActions);

                graphService.AStarStep();
                currentNode = graphService.GetNextVertexFromOpenList();
            }

            if (currentNode != null
                && (!currentNode.HasUnsatisfiedProperties() || currentNode.CanBeSatisfiedByStartState(_startState))) {
                _currentPlan = graphService.GetShortestPath();
                _currentPlan.Reverse();
            }
            if (graphService.GetActualDepthFromRoot() >= _maximuxSearchDepth || !graphService.HasNextVertexOnOpenList())
                _currentPlan = new List<AbstractGoapAction> {new SurrogateAction()};

            return _currentPlan;
        }
        
        /// <summary>
        ///     get the actions that could satisfy the current needed states. this is done by inspect the effectToAction map.
        /// </summary>
        /// <param name="unsatisfied"></param>
        /// <returns></returns>
        private List<AbstractGoapAction> GetActionsByUnsatisfiedProperties(List<IGoapWorldProperty> unsatisfied) {
            HashSet<AbstractGoapAction> relevantActions = new HashSet<AbstractGoapAction>();
            foreach (IGoapWorldProperty property in unsatisfied) {
                _effectToAction[property].ForEach(x => relevantActions.Add(x));
            }
            return relevantActions.ToList();
        }

        private List<AbstractGoapAction> FilterActionsByContextPreconditions(List<AbstractGoapAction> actionsToFilter) {
            return actionsToFilter.Where(action => action.ValidateContextPreconditions()).ToList();
        }
    }
}
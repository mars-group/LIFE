using System;
using System.Collections.Generic;
using System.Linq;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;
using GoapGraphConnector.SimpleGraph;
using TypeSafeBlackboard;

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
        private Dictionary<IGoapWorldProperty, List<AbstractGoapAction>> _effectToAction;
        private Blackboard _blackboard;

        /// <summary>
        /// </summary>
        /// <param name="maximuxSearchDepth"></param>
        /// <param name="availableActions"></param>
        /// <param name="effectToAction"></param>
        public GoapPlanner(int maximuxSearchDepth, List<AbstractGoapAction> availableActions, Dictionary<IGoapWorldProperty, List<AbstractGoapAction>> effectToAction, Blackboard blackboard)
        {
            if (availableActions.Count == 0)
                throw new ArgumentException("Planner may not be instanciated with an empty list of actions");
            _effectToAction = effectToAction;
            _blackboard = blackboard;
            _maximuxSearchDepth = maximuxSearchDepth;
            _availableActions = availableActions;
        }

        private IGoapGraphService InitializeGraphService(List<IGoapWorldProperty> graphRoot){
            var graphService = new GoapSimpleGraphService();
            graphService.InitializeGoapGraph(graphRoot);
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

        public List<AbstractGoapAction> GetPlan(IGoapGoal goal) {
            List<IGoapWorldProperty> graphRoot = goal.GetTargetWorldstates();

            IGoapGraphService graphService = InitializeGraphService(graphRoot);

            IGoapNode currentNode = graphService.GetNextVertexFromOpenList();

            while (!IsSearchDepthLimitExceeded(graphService) && currentNode.HasUnsatisfiedProperties()) {

                List<AbstractGoapAction> satisfyingActions = GetActionsByUnsatisfiedProperties(currentNode.GetUnsatisfiedGoalValues());
                List<AbstractGoapAction> filtered = FilterActionsByContextPreconditions(satisfyingActions);

                

                graphService.ExpandCurrentVertex(filtered);



                graphService.AStarStep();
                currentNode = graphService.GetNextVertexFromOpenList();
              }

            if (currentNode != null && !currentNode.HasUnsatisfiedProperties()){
                    _currentPlan = graphService.GetShortestPath();
                    _currentPlan.Reverse();
                }
                if (graphService.GetActualDepthFromRoot() >= _maximuxSearchDepth || !graphService.HasNextVertexOnOpenList())
                    _currentPlan = new List<AbstractGoapAction> { new SurrogateAction() };

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

        private List<AbstractGoapAction> GetActionsByUnsatisfiedProperties(List<IGoapWorldProperty> unsatisfied) {
            HashSet<AbstractGoapAction> relevantActions = new HashSet<AbstractGoapAction>();
            foreach (var property in unsatisfied) {
                _effectToAction[property].ForEach(x => relevantActions.Add(x));
            }
            return relevantActions.ToList();
        }

        private List<AbstractGoapAction> FilterActionsByContextPreconditions(List<AbstractGoapAction> actionsToFilter) {
            return actionsToFilter.Where(action => action.ValidateContextPreconditions()).ToList();
        } 


    }
}
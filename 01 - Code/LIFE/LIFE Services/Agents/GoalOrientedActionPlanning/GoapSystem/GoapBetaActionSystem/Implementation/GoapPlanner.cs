using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using GoapBetaCommon.Abstract;
using GoapBetaCommon.Implementation;
using GoapBetaCommon.Interfaces;
using GoapBetaGraphConnector.SimpleGraph;
using log4net;

namespace GoapBetaActionSystem.Implementation {

    /// <summary>
    ///     The goapplanner is responsible for the process of finding a valid plan from the actions, currentWorld and
    ///     targetWorld given to him. He uses the graph component for creating a new plan.
    ///     the planner saves the plan and check it for validity depending on the events and state of simulation.
    ///     The caller is responsible for giving well defined and corresponding actions and world states.
    ///     planner is responsible for the regressive search. the graph component knows nothing about that and the switched
    ///     root and target. planner is also responsible for the correct node creation.
    /// </summary>
    internal class GoapPlanner {
        private readonly int _maximuxSearchDepth = int.MaxValue;
        private readonly List<AbstractGoapAction> _availableActions;
        private readonly Dictionary<WorldstateSymbol, List<AbstractGoapAction>> _effectToAction;
        private readonly List<WorldstateSymbol> _startState;
        private List<AbstractGoapAction> _currentPlan;

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// </summary>
        /// <param name="maximuxSearchDepth"></param>
        /// <param name="availableActions"></param>
        /// <param name="effectToAction"></param>
        /// <param name="startState"></param>
        internal GoapPlanner
            (int maximuxSearchDepth,
                List<AbstractGoapAction> availableActions,
                Dictionary<WorldstateSymbol, List<AbstractGoapAction>> effectToAction,
                List<WorldstateSymbol> startState) {
            if (availableActions.Count == 0) {
                throw new ArgumentException("Planner may not be instanciated with an empty list of actions");
            }
            _effectToAction = effectToAction;
            _startState = startState;
            _maximuxSearchDepth = maximuxSearchDepth;
            _availableActions = availableActions;
        }


        private IGoapGraphService InitializeGraphService(List<WorldstateSymbol> graphRoot) {
            GoapSimpleGraphService graphService = new GoapSimpleGraphService();

            IGoapNode root = new Node(graphRoot, new List<WorldstateSymbol>(), graphRoot.Count);
            Log.Info("NODE CREATED: " + root);
            //Console.WriteLine("NODE CREATED: " + root);
            graphService.InitializeGoapGraph(root);
            return graphService;
        }

        /// <summary>
        ///     main method of the planner called from goap manager
        /// </summary>
        /// <param name="goal"></param>
        /// <returns></returns>
        public List<AbstractGoapAction> GetPlan(IGoapGoal goal) {
            List<WorldstateSymbol> graphRoot = goal.GetTargetWorldstates();
            IGoapGraphService graphService = InitializeGraphService(graphRoot);
            
            IGoapNode currentNode = graphService.GetNextVertexFromOpenList();

            while (!IsSearchDepthLimitExceeded(graphService)
                   && (currentNode.HasUnsatisfiedProperties() && !currentNode.CanBeSatisfiedByStartState(_startState))) {

                List<AbstractGoapAction> satisfyingActions = GetActionsByUnsatisfiedProperties(currentNode.GetUnsatisfiedGoalValues());
                List<AbstractGoapAction> contextPreconditionsFulfilled = FilterActionsByContextPreconditions(satisfyingActions);

                List<AbstractGoapAction>  applicableActions = FilterActions(contextPreconditionsFulfilled, currentNode);
                List<IGoapEdge> edges = GetResultingEdges(applicableActions, currentNode);

                graphService.ExpandCurrentVertex(edges);
                graphService.AStarStep();
                currentNode = graphService.GetNextVertexFromOpenList();
            }

            if (currentNode != null
                && (!currentNode.HasUnsatisfiedProperties() || currentNode.CanBeSatisfiedByStartState(_startState))) {
                List<IGoapEdge> planEdges = graphService.GetShortestPath();
                _currentPlan = planEdges.Select(goapEdge => goapEdge.GetAction()).ToList();
                _currentPlan.Reverse();
            }
            if (graphService.GetActualDepthFromRoot() >= _maximuxSearchDepth || !graphService.HasNextVertexOnOpenList()) {
                _currentPlan = new List<AbstractGoapAction> {new SurrogateAction()};
            }

            return _currentPlan;
        }

        /// <summary>
        ///     check if the limit of graph depth is exceedet
        /// </summary>
        /// <param name="graphService"></param>
        /// <returns></returns>
        private bool IsSearchDepthLimitExceeded(IGoapGraphService graphService) {
            return graphService.GetActualDepthFromRoot() > _maximuxSearchDepth;
        }

        /// <summary>
        ///     get the actions that could satisfy the current needed states. this is done by inspect the effectToAction map.
        /// </summary>
        /// <param name="unsatisfied"></param>
        /// <returns></returns>
        private List<AbstractGoapAction> GetActionsByUnsatisfiedProperties(List<WorldstateSymbol> unsatisfied) {
            HashSet<AbstractGoapAction> relevantActions = new HashSet<AbstractGoapAction>();
            foreach (WorldstateSymbol property in unsatisfied) {
                _effectToAction[property].ForEach(x => relevantActions.Add(x));
            }
            return relevantActions.ToList();
        }

        /// <summary>
        ///     get the action which are executable by context preconditions
        /// </summary>
        /// <param name="actionsToFilter"></param>
        /// <returns></returns>
        private List<AbstractGoapAction> FilterActionsByContextPreconditions(List<AbstractGoapAction> actionsToFilter) {
            var correct = new List<AbstractGoapAction>();
            foreach (var action in actionsToFilter) {
                if (action.ValidateContextPreconditions()) {
                    correct.Add(action);
                }
            }
            return correct;
        }

        /// <summary>
        ///     node creation protocoll
        /// </summary>
        /// <param name="action"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private IGoapNode GetChildNodeByActionAndParent(AbstractGoapAction action, IGoapNode parent) {
            List<WorldstateSymbol> goalValues = new List<WorldstateSymbol>();
            List<WorldstateSymbol> currValues = new List<WorldstateSymbol>();

            // step 1 add unsatisfied goal values 
            goalValues.AddRange(parent.GetUnsatisfiedGoalValues());

            // step 2 find satisfying effects and add to current values
            currValues.AddRange(goalValues.Where(goalValue => action.Effects.Contains(goalValue)).ToList());

            // step 3 check if effects have satisfied goal values
            if (!(goalValues.Intersect(currValues).Count() > 0)) {
                throw new ArgumentException("an unproductive action was chosen for expanding a node");
            }

            // step 4
            List<WorldstateSymbol> additionalGoals = action.PreConditions.Where
                (precondition => !goalValues.Contains(precondition)).
                ToList();
            goalValues.AddRange(additionalGoals);

            // simple heuristik by counting not reached goal states
            int heuristic = goalValues.Except(currValues).Count();

            
            var node=    new Node(goalValues.ToList(), currValues.ToList(), heuristic);
            //Console.WriteLine("NODE CREATED: " + node);
            Log.Info("NODE CREATED: " + node);
            return node;
        }

        /// <summary>
        ///     check if the action changes the worldstate to the "opposite" of wanted value
        /// </summary>
        /// <param name="current"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private bool HasConverseEffect(IGoapNode current, AbstractGoapAction action) {
            foreach (WorldstateSymbol goalValue in current.GetUnsatisfiedGoalValues()) {
                if (action.Effects.Any
                    (effect => effect.EnumName.Equals(goalValue.EnumName)
                               && !effect.Value.Equals(goalValue.Value))) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     because of no inkremental ability in states there may be only one precondition
        ///     even if the inspected action needs the same
        ///     this is not allowed
        /// </summary>
        /// <param name="current"></param>
        /// <param name="goapAction"></param>
        /// <returns></returns>
        private bool IsCreatingDuplicatedGoalValueKeys(IGoapNode current, AbstractGoapAction goapAction) {
            List<Enum> nodeKeys =
                current.GetUnsatisfiedGoalValues().
                    Select(unsatisfiedGoalValue => unsatisfiedGoalValue.EnumName).
                    ToList();
            List<Enum> actionKeys =
                goapAction.PreConditions.Select(precondition => precondition.EnumName).ToList();

            return actionKeys.Any(actionKey => nodeKeys.Contains(actionKey));
        }

        /// <summary>
        ///     union of specialized test methods for actions to use on node
        /// </summary>
        /// <param name="applicableActions"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private List<AbstractGoapAction> FilterActions(List<AbstractGoapAction> applicableActions, IGoapNode parent) {
            List<AbstractGoapAction> filtered = new List<AbstractGoapAction>();
            foreach (AbstractGoapAction action in applicableActions) {
                if (HasConverseEffect(parent, action)) {
                    continue;
                }
                if (IsCreatingDuplicatedGoalValueKeys(parent, action)) {
                    continue;
                }
                filtered.Add(action);
            }
            return filtered;
        }

        /// <summary>
        ///     create the new edge (includes target node) resulting from node and action
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="actions"></param>
        /// <returns></returns>
        private List<IGoapEdge> GetResultingEdges(List<AbstractGoapAction> actions, IGoapNode parent) {
            List<IGoapEdge> edges = new List<IGoapEdge>();
            foreach (AbstractGoapAction action in actions) {
                IGoapNode newNode = GetChildNodeByActionAndParent(action, parent);
                Edge newEdge = new Edge(action, parent, newNode);
                edges.Add(newEdge);
            }
            return edges;
        }
    }

}
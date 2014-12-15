using System;
using System.Collections.Generic;
using System.Linq;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapCommon.Interfaces;
using GoapGraphConnector.SimpleGraph;

namespace GoapActionSystem.Implementation {

    /// <summary>
    ///     The goapplanner is responsible for the process of finding a valid plan from the actions, currentWorld and
    ///     targetWorld given to him. He uses the graph component for creating a new plan.
    ///     the planner saves the plan and check it for validity depending on the events and state of simulation.
    ///     The caller is responsible for giving well defined and corresponding actions and world states.
    ///     planner is responsible for the regressive search. the graph component knows nothing about that and the switched
    ///     root and target. planner is also responsible for the correct node creation.
    /// </summary>
    internal class GoapPlanner {
        private readonly int _maximuxSearchDepth;
        private readonly Dictionary<WorldstateSymbol, List<AbstractGoapAction>> _effectToAction;
        private readonly List<WorldstateSymbol> _startState;
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
                Dictionary<WorldstateSymbol, List<AbstractGoapAction>> effectToAction,
                List<WorldstateSymbol> startState) {
            if (availableActions.Count == 0) {
                throw new ArgumentException("Planner may not be instanciated with an empty list of actions");
            }
            _effectToAction = effectToAction;
            _startState = startState;
            _maximuxSearchDepth = maximuxSearchDepth;
        }

        /// <summary>
        ///     create the initial datastructures and values for search with a star.
        /// </summary>
        /// <param name="graphRoot"></param>
        /// <returns></returns>
        private IGoapGraphService InitializeGraphService(List<WorldstateSymbol> graphRoot) {
            GoapSimpleGraphService graphService = new GoapSimpleGraphService();
            IGoapNode root = new Node(graphRoot, new List<WorldstateSymbol>(), graphRoot.Count);
            //GoapComponent.Log.Debug("NODE CREATED: " + root);
            graphService.InitializeGoapGraph(root);
            return graphService;
        }

        /// <summary>
        ///     Main method of the planner. is called from goap manager.
        ///     create a list of actions the agent can follow. search direction is regressive.
        /// </summary>
        /// <param name="goal"></param>
        /// <returns></returns>
        public List<AbstractGoapAction> GetPlan(AbstractGoapGoal goal) {
            List<WorldstateSymbol> graphRoot = goal.TargetWorldState;
            IGoapGraphService graphService = InitializeGraphService(graphRoot);

            // Here the root node is chosen.
            IGoapNode currentNode = graphService.GetNextVertex();

            while (IsNotTargetNode(currentNode)) {
                // Expand node if search depth is smaller than maximum .
                if (IsSearchDepthSmallerAsMaximum(graphService)) {
                    List<AbstractGoapAction> satisfyingActions = GetActionsByUnsatisfiedProperties
                        (currentNode.GetUnsatisfiedGoalValues());
                    List<AbstractGoapAction> contextPreconditionsFulfilled = FilterActionsByContextPreconditions
                        (satisfyingActions);
                    List<AbstractGoapAction> applicableActions = FurtherActionFilter
                        (contextPreconditionsFulfilled, currentNode);
                    List<IGoapEdge> edges = GetResultingEdges(applicableActions, currentNode);

                    graphService.ExpandCurrentVertex(edges);
                }
                else {
                    GoapComponent.Log.Info("GoapPlanner: search depth maximum on branch of graph reached");
                }

                // Set node on closed list and calculate values for reachable adjacent neighbours.
                graphService.CalculateCurrentNode();

                if (graphService.HasNextVertexOnOpenList()) {
                    graphService.ChooseNextNodeFromOpenList();
                    currentNode = graphService.GetNextVertex();
                }
                else {
                    GoapComponent.Log.Info("GoapPlanner: no more nodes in graph for search");
                    return new List<AbstractGoapAction>();
                }
            }

            // case: target is found
            if (IsTargetNode(currentNode)) {
                List<IGoapEdge> planEdges = graphService.GetShortestPath();
                _currentPlan = planEdges.Select(goapEdge => goapEdge.GetAction()).ToList();
                _currentPlan.Reverse();
            }
            if (_currentPlan != null && _currentPlan.Count > 0) {
                return _currentPlan;
            }
            return new List<AbstractGoapAction>();
        }

        /// <summary>
        ///     Check if the current nodes depth in graph is smaller than maximum search depth.
        ///     Only if it is smaller the current nodes children can be inserted in graph.
        /// </summary>
        /// <param name="graphService"></param>
        /// <returns></returns>
        private bool IsSearchDepthSmallerAsMaximum(IGoapGraphService graphService) {
            return graphService.GetActualDepthFromRoot() < _maximuxSearchDepth;
        }

        /// <summary>
        ///     Inspect current node if its current values are equal to goal values or
        ///     if the unsatisfied goal values can be satisfied by the start worldstate.
        /// </summary>
        /// <param name="currentNode"></param>
        /// <returns></returns>
        private bool IsNotTargetNode(IGoapNode currentNode) {
            return (currentNode.HasUnsatisfiedProperties() && !currentNode.CanBeSatisfiedByStartState(_startState));
        }

        /// <summary>
        ///     inspect current node if its current values are NOT equal to goal values or
        ///     if the unsatisfied goal values can NOT be satisfied by the start worldstate.
        /// </summary>
        /// <param name="currentNode"></param>
        /// <returns></returns>
        private bool IsTargetNode(IGoapNode currentNode) {
            return !IsNotTargetNode(currentNode);
        }

        /// <summary>
        ///     Get the actions that could satisfy the current needed states by a copy of the actions
        ///     from config class. Search is done by inspecting the effectToAction map.
        /// </summary>
        /// <param name="unsatisfied"></param>
        /// <returns></returns>
        private List<AbstractGoapAction> GetActionsByUnsatisfiedProperties(List<WorldstateSymbol> unsatisfied) {
            HashSet<AbstractGoapAction> relevantActions = new HashSet<AbstractGoapAction>();
            foreach (WorldstateSymbol property in unsatisfied) {
                if (_effectToAction.ContainsKey(property)) {
                    _effectToAction[property].ForEach(action => relevantActions.Add(action.GetResetCopy()));
                }
            }
            return relevantActions.ToList();
        }

        /// <summary>
        ///     Get list of actions which are executable by context preconditions
        /// </summary>
        /// <param name="actionsToFilter"></param>
        /// <returns></returns>
        private List<AbstractGoapAction> FilterActionsByContextPreconditions(List<AbstractGoapAction> actionsToFilter) {
            List<AbstractGoapAction> passedPreconditions = new List<AbstractGoapAction>();
            foreach (AbstractGoapAction action in actionsToFilter) {
                if (action.ValidateContextPreconditions()) {
                    passedPreconditions.Add(action);
                }
            }
            return passedPreconditions;
        }

        /// <summary>
        ///     Node creation protocoll
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
            if (!(goalValues.Intersect(currValues).Any())) {
                throw new ArgumentException("an unproductive action was chosen for expanding a node");
            }

            // step 4 add preconditions of the chosen action to goal values
            List<WorldstateSymbol> additionalGoals = action.PreConditions.Where
                (precondition => !goalValues.Contains(precondition)).
                ToList();
            goalValues.AddRange(additionalGoals);

            // simple heuristik by counting not reached goal states
            int heuristic = goalValues.Except(currValues).Count();

            Node node = new Node(goalValues.ToList(), currValues.ToList(), heuristic);
            //GoapComponent.Log.Debug("NODE CREATED: " + node);
            return node;
        }

        /// <summary>
        ///     Check if the action changes the worldstate to the "opposite" of wanted value
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
        ///     Because of no increment ability for world states there may be only one precondition of one key type.
        ///     Even if the inspected action needs the same this is not allowed.
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
        ///     Union of specialized test methods for actions to use on node
        /// </summary>
        /// <param name="applicableActions"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private List<AbstractGoapAction> FurtherActionFilter
            (List<AbstractGoapAction> applicableActions, IGoapNode parent) {
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
        ///     Create the new edge (includes target node) resulting from node and the new created action.
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
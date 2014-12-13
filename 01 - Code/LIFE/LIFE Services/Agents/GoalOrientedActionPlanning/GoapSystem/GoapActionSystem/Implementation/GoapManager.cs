﻿using System.Collections.Generic;
using System.Linq;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapCommon.Interfaces;
using TypeSafeBlackboard;

namespace GoapActionSystem.Implementation {

    public class GoapManager : AbstractGoapSystem {
        private readonly List<AbstractGoapAction> _availableActions;
        private readonly List<AbstractGoapGoal> _availableGoals;
        private readonly Blackboard _internalBlackboard;
        private readonly int _maximumGraphSearchDepth;
        private readonly IGoapAgentConfig _configClass;
        private readonly bool _ignoreIfFinishedForTesting;

        /// <summary>
        ///     goap element: faster search for actions by needed worldstate symbols
        /// </summary>
        private Dictionary<WorldstateSymbol, List<AbstractGoapAction>> _effectToAction;
        private List<AbstractGoapAction> _currentPlan = new List<AbstractGoapAction>();
        private AbstractGoapGoal _currentGoal;

        /// <summary>
        ///     create the manager inklusive the current worldstates
        /// </summary>
        /// <param name="availableActions"></param>
        /// <param name="availableGoals"></param>
        /// <param name="startStates"></param>
        /// <param name="blackboard"></param>
        /// <param name="maximumGraphSearchDepth"></param>
        /// <param name="ignoreFinishedForTesting"></param>
        internal GoapManager
            (List<AbstractGoapAction> availableActions,
                List<AbstractGoapGoal> availableGoals,
                Blackboard blackboard,
                List<WorldstateSymbol> startStates,
                int maximumGraphSearchDepth,
                bool ignoreFinishedForTesting) {
            _availableActions = availableActions;
            _availableGoals = availableGoals;
            _internalBlackboard = blackboard;
            _internalBlackboard.Set(Worldstate, startStates);
            _maximumGraphSearchDepth = maximumGraphSearchDepth;
            _ignoreIfFinishedForTesting = ignoreFinishedForTesting;
            InitializationHelper();
        }

        /// <summary>
        ///     create the manager inklusive the current worldstates
        /// </summary>
        /// <param name="availableActions"></param>
        /// <param name="availableGoals"></param>
        /// <param name="startStates"></param>
        /// <param name="blackboard"></param>
        /// <param name="maximumGraphSearchDepth"></param>
        /// <param name="configClass"></param>
        internal GoapManager
            (List<AbstractGoapAction> availableActions,
                List<AbstractGoapGoal> availableGoals,
                Blackboard blackboard,
                List<WorldstateSymbol> startStates,
                int maximumGraphSearchDepth,
                IGoapAgentConfig configClass) {
            _availableActions = availableActions;
            _availableGoals = availableGoals;
            _internalBlackboard = blackboard;
            _internalBlackboard.Set(Worldstate, startStates);
            _maximumGraphSearchDepth = maximumGraphSearchDepth;
            _configClass = configClass;

            InitializationHelper();
        }

        /// <summary>
        ///     Initialize the first goal and plan. Does not prepare the first plan and action if the
        ///     manager has to respect the return value from IsFinished by actions.
        /// </summary>
        /// <returns></returns>
        private bool InitializationHelper() {
            CreateEffectActionHashTable();
            UpdateRelevancyOfGoals();
            
            if (!_ignoreIfFinishedForTesting) {
                if (!TryGetGoalAndPlan()) {
                    return false;
                }
                TakeActionFromPlan();
                return true;
            }
            return true;
        }

        /// <summary>
        ///     Call the current action on method is finished. This is only used if 
        ///     respecting finished action is configured.
        /// </summary>
        /// <returns></returns>
        private bool IsActionForExecutionFinished() {
            AbstractGoapAction action = _internalBlackboard.Get(ActionForExecution);
            return action.IsFinished();
        }

        /// <summary>
        ///     Entry point for user of goap services and main method.
        /// </summary>
        /// <returns></returns>
        public override AbstractGoapAction GetNextAction() {
            if (_ignoreIfFinishedForTesting) {
                return GetNextActionIgnoringIsFinishedAtAction();
            }
            return GetNextActionRespectingIsFinishedByActions();
        }

        /// <summary>
        ///     Entry point for complex use of goap, where actions must care for setting finished when n times 
        ///     executed. 
        /// </summary>
        /// <returns></returns>
        private AbstractGoapAction GetNextActionRespectingIsFinishedByActions() {
            if (IsWorldstateUpdateConfiguredForEveryCallOfGetNextAction()) {
                UpdateWorldstateByAgent();
            }

            /* current action can be given if: 
             *  + goal is still valid
             *  + the action is not finished
             *  + the action is executable
            */
            if (CurrentGoalIsValid() && !IsActionForExecutionFinished() && IsCurrentActionExecutable()) {
                return _internalBlackboard.Get(ActionForExecution);
            }

            /* next action can be given if
             *  + goal is still valid
             *  + current action is finished
             *  + plan is not empty
             *  + the action is executable            
             * */
            if (CurrentGoalIsValid() && IsActionForExecutionFinished() && IsCurrentPlanAvailable()
                && IsCurrentActionExecutable()) {
                ConvertWorldstateByActionForExecution();
                AbstractGoapAction nextAction = TakeActionFromPlan();
                return nextAction;
            }

            /* new plan and goal is needed if
             *      Goal = null
             * OR
             *      the current action is NOT executable
             * OR   
             *      current goal is NOT valid
             * OR     
             *      the current goal is REACHED 
             * OR 
             *      current action is finished and plan is empty
             */
            bool replanningTime = false;

            if (IsCurrentGoalReached()) {
                replanningTime = true;
                GoapComponent.Log.Info("GoapManager: current goal is successful reached");
            }
            else if (!CurrentGoalIsValid()) {
                replanningTime = true;
                GoapComponent.Log.Info("GoapManager: current goal is not valid or empty");
            }
            else if (!IsCurrentActionExecutable()) {
                replanningTime = true;
                GoapComponent.Log.Info("GoapManager: current action is not executable");
            }

            if (replanningTime) {
                _internalBlackboard.Set(ActionForExecution, null);
                _currentPlan = new List<AbstractGoapAction>();

                if (IsWorldstateUpdateConfiguredBeforeReplanning()) {
                    UpdateWorldstateByAgent();
                }

                if (TryGetGoalAndPlan()) {
                    AbstractGoapAction currentAction = TakeActionFromPlan();
                    return currentAction;
                }
            }
            GoapComponent.Log.Info("GoapManager: planning failed");
            return new SurrogateAction();
        }

        /// <summary>
        ///     Entry point for simple use of goap, where actions are finished when tey ar given back.
        /// </summary>
        /// <returns></returns>
        private AbstractGoapAction GetNextActionIgnoringIsFinishedAtAction() {
            if (IsWorldstateUpdateConfiguredForEveryCallOfGetNextAction()) {
                UpdateWorldstateByAgent();
            }

            /* new plan and goal is needed if:
             * 
             *      the current goal is REACHED  OR current goal is NOT valid  OR
             *      current plan is empty  OR  the NEXT action is not executable
             */
            bool replanningTime = false;
            bool replanningSuccessful = true;

            if (IsCurrentGoalReached()) {
                replanningTime = true;
                GoapComponent.Log.Info("GoapManager: current goal is successful reached");
            }
            else if (!CurrentGoalIsValid()) {
                replanningTime = true;
                GoapComponent.Log.Info("GoapManager: current goal is not valid or empty");
            }
            else if (!IsCurrentPlanAvailable()) {
                replanningTime = true;
                GoapComponent.Log.Info("GoapManager: current action is not executable");
            }
            else if (!IsNextActionExecutable()) {
                replanningTime = true;
                GoapComponent.Log.Info("GoapManager: next action is not executable");
            }

            if (replanningTime) {
                _internalBlackboard.Set(ActionForExecution, null);
                _currentPlan = new List<AbstractGoapAction>();

                if (IsWorldstateUpdateConfiguredBeforeReplanning()) {
                    UpdateWorldstateByAgent();
                }
                replanningSuccessful = TryGetGoalAndPlan();
            }

            // get the next action from plan. Instant manipulation of worldstate in goap.
            if (replanningSuccessful) {
                AbstractGoapAction currentAction = TakeActionFromPlan();
                List<WorldstateSymbol> newState =  currentAction.GetResultingWorldstate(_internalBlackboard.Get(AbstractGoapSystem.Worldstate));
                _internalBlackboard.Set(AbstractGoapSystem.Worldstate,newState);
                return currentAction;
            }
            GoapComponent.Log.Info("GoapManager: planning failed");
            return new SurrogateAction();
        }


        /// <summary>
        ///     Check if the current goal is not null and ...
        /// </summary>
        /// <returns></returns>
        private bool CurrentGoalIsValid() {
            if (_currentGoal != null) {
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Read from in the configuration class if available if the world state symbols
        ///     have to be updated before planning.
        /// </summary>
        /// <returns></returns>
        private bool IsWorldstateUpdateConfiguredBeforeReplanning() {
            return _configClass != null && _configClass.ForceSymbolsUpdateBeforePlanning();
        }

        private bool IsWorldstateUpdateConfiguredForEveryCallOfGetNextAction() {
            return _configClass != null && _configClass.ForceSymbolsUpdateEveryActionRequest();
        }

        /// <summary>
        ///     Use the connection between agent and goap to update the symbols in the goap system.
        /// </summary>
        private void UpdateWorldstateByAgent() {
            List<WorldstateSymbol> newState = _configClass.GetUpdatedSymbols();
            _internalBlackboard.Set(Worldstate, newState);
        }

        /// <summary>
        ///     Search goals by priority. Choose the first goal which is reachable with a plan.
        /// </summary>
        /// <returns></returns>
        private bool TryGetGoalAndPlan() {
            _currentGoal = null;
            _currentPlan = null;
            IOrderedEnumerable<AbstractGoapGoal> goals = GetUnsatisfiedGoalsSortedByRelevancy();

            // case all goals are satisfied
            if (!goals.Any()) {
                GoapComponent.Log.Info("TryGetGoalAndPlan: there were no unsatisfied goals found");
                return false;
            }

            foreach (AbstractGoapGoal goapGoal in goals)
            {
                List<AbstractGoapAction> tempPlan = CreateNewPlan(goapGoal);

                if (tempPlan.Count > 0) {
                    _currentGoal = goapGoal;
                    _currentPlan = tempPlan;
                    return true;
                }
            }

            GoapComponent.Log.Debug
                ("TryGetGoalAndPlan: there was no plan found for any unsatisfied goal at state: "
                 + GetCurrentStateValues());
            return false;
        }

        private string GetCurrentStateValues() {
            string symbolsAsString = "";

            List<WorldstateSymbol> symbols = _internalBlackboard.Get(Worldstate);
            foreach (WorldstateSymbol symbol in symbols) {
                symbolsAsString += "| " + symbol.ToString() + "| ";
            }
            return symbolsAsString;
        }

        /// <summary>
        ///     Get the first action of the plan and update the blackboard entry for the ActionForExecution.
        /// </summary>
        /// <returns></returns>
        private AbstractGoapAction TakeActionFromPlan() {
            AbstractGoapAction currentAction = _currentPlan.First();
            _internalBlackboard.Set(ActionForExecution, currentAction);
            _currentPlan.RemoveAt(0);
            return currentAction;
        }

        /// <summary>
        ///     create a new goap planner to create a new plan by the given goal
        /// </summary>
        private List<AbstractGoapAction> CreateNewPlan(AbstractGoapGoal goal)
        {
            GoapPlanner planner = new GoapPlanner
                (_maximumGraphSearchDepth, _availableActions, _effectToAction, _internalBlackboard.Get(Worldstate));
            return planner.GetPlan(goal);
        }

        /// <summary>
        ///     check if there is a current goal and if its reached by the current world state symbols
        /// </summary>
        /// <returns></returns>
        private bool IsCurrentGoalReached() {
            return _currentGoal != null && _currentGoal.IsSatisfied(_internalBlackboard.Get(Worldstate));
        }

        /// <summary>
        ///     create a key value structure for the search for satisfying actions
        /// </summary>
        private void CreateEffectActionHashTable() {
            _effectToAction = new Dictionary<WorldstateSymbol, List<AbstractGoapAction>>();

            foreach (AbstractGoapAction action in _availableActions) {
                foreach (WorldstateSymbol effect in action.Effects) {
                    if (_effectToAction.ContainsKey(effect)) {
                        _effectToAction[effect].Add(action);
                    }
                    else {
                        _effectToAction[effect] = new List<AbstractGoapAction> {action};
                    }
                }
            }
        }

        /// <summary>
        ///     get all goal sorted by relevancy which are currently not satisfied
        /// </summary>
        /// <returns></returns>
        private IOrderedEnumerable<AbstractGoapGoal> GetUnsatisfiedGoalsSortedByRelevancy()
        {
            List<AbstractGoapGoal> notSatisfied = _availableGoals.FindAll
                (g => !g.IsSatisfied(_internalBlackboard.Get(Worldstate)));
            return notSatisfied.OrderByDescending(x => x.GetRelevancy());
        }

        /// <summary>
        ///     validate next action of the plan
        /// </summary>
        /// <returns></returns>
        private bool IsNextActionExecutable() {
            if (IsCurrentPlanAvailable()) {
                return (_currentPlan.First().IsExecutable(_internalBlackboard.Get(Worldstate))
                        && _currentPlan.First().ValidateContextPreconditions());
            }
            return false;
        }

        /// <summary>
        ///     validate next action of the plan
        /// </summary>
        /// <returns></returns>
        private bool IsCurrentActionExecutable() {
            AbstractGoapAction currentaction = _internalBlackboard.Get(ActionForExecution);
            if (currentaction != null) {
                return (currentaction.IsExecutable(_internalBlackboard.Get(Worldstate))
                        && currentaction.ValidateContextPreconditions());
            }
            return false;
        }

        /// <summary>
        ///     Check if plan is not null or empty.
        /// </summary>
        /// <returns></returns>
        private bool IsCurrentPlanAvailable() {
            return (_currentPlan != null && _currentPlan.Count > 0);
        }

        /// <summary>
        ///     Call the update relavancy method on all known goals.
        /// </summary>
        private void UpdateRelevancyOfGoals() {
            _availableGoals.ForEach(x => x.UpdateRelevancy(_internalBlackboard.Get(Worldstate)));
        }

        /// <summary>
        ///     Change the goap internal worldstate by the effects of the action.
        /// </summary>
        /// <param name="action"></param>
        private void ConvertWorldstateByAction(AbstractGoapAction action) {
            List<WorldstateSymbol> newsWorldstate = action.GetResultingWorldstate
                (_internalBlackboard.Get(Worldstate));
            _internalBlackboard.Set(Worldstate, newsWorldstate);
        }

        /// <summary>
        ///     Adapter for ConvertWorldstateByAction.
        /// </summary>
        private void ConvertWorldstateByActionForExecution() {
            AbstractGoapAction currentAction = _internalBlackboard.Get(ActionForExecution);
            ConvertWorldstateByAction(currentAction);
        }
    }

}
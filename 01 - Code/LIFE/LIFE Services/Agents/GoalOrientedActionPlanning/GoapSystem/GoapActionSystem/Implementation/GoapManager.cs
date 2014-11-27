using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapCommon.Interfaces;
using TypeSafeBlackboard;

namespace GoapActionSystem.Implementation {

    public class GoapManager : AbstractGoapSystem {
        private readonly List<AbstractGoapAction> _availableActions;
        private readonly List<IGoapGoal> _availableGoals;
        private readonly Blackboard _internalBlackboard;
        private readonly int _maximumGraphSearchDepth;

        /// <summary>
        ///     goap element: faster search for actions be needed worldstates
        /// </summary>
        private Dictionary<WorldstateSymbol, List<AbstractGoapAction>> _effectToAction;

        private List<AbstractGoapAction> _currentPlan = new List<AbstractGoapAction>();
        private IGoapGoal _currentGoal;

        /// <summary>
        ///     create the manager inklusive the current worldstates
        /// </summary>
        /// <param name="availableActions"></param>
        /// <param name="availableGoals"></param>
        /// <param name="startStates"></param>
        /// <param name="blackboard"></param>
        /// <param name="maximumGraphSearchDepth"></param>
        internal GoapManager
            (List<AbstractGoapAction> availableActions,
                List<IGoapGoal> availableGoals,
                Blackboard blackboard,
                List<WorldstateSymbol> startStates,
                int maximumGraphSearchDepth) {
            
            _availableActions = availableActions;
            _availableGoals = availableGoals;
            _internalBlackboard = blackboard;
            _internalBlackboard.Set(Worldstate, startStates);
            _maximumGraphSearchDepth = maximumGraphSearchDepth;
            CreateEffectActionHashTable();
        }

        /// <summary>
        ///     entry point for user of goap services and main method
        /// </summary>
        /// <returns></returns>
        public override AbstractGoapAction GetNextAction() {
            UpdateRelevancyOfGoals();

            // case: the current action has not yet finished (actions may last more than one tick)
            AbstractGoapAction runningAction = _internalBlackboard.Get(ActionForExecution);
            if (runningAction != null && !runningAction.IsFinished()) {
                return runningAction;
            }

            // case: current goal is reached
            if (IsCurrentGoalReached()) {
                GoapComponent.Log.Info("Goal " + _currentGoal.GetType() + " is reached.");

                if (!TryGetGoalAndPlan()) {
                    return new SurrogateAction();
                }
            }
                // case: last tick no goal was found
            else if (_currentGoal == null) {
                if (!TryGetGoalAndPlan()) {
                    return new SurrogateAction();
                }
            }

            // case: a plan is given and the next action is executable 
            if (IsCurrentPlanAvailable() && IsNextActionExecutable()) {
                AbstractGoapAction currentAction = _currentPlan.First();
                _internalBlackboard.Set(ActionForExecution, currentAction);
                _currentPlan.RemoveAt(0);
                return currentAction;
            }
            return new SurrogateAction();
        }

        /// <summary>
        ///     search goals by priority. choose the first goal which is reachable with a plan
        /// </summary>
        /// <returns></returns>
        private bool TryGetGoalAndPlan() {
            _currentGoal = null;
            _currentPlan = null;
            IOrderedEnumerable<IGoapGoal> goals = GetUnsatisfiedGoalsSortedByRelevancy();

            // case all goals are satisfied
            if (!goals.Any()) {
                GoapComponent.Log.Info("TryGetGoalAndPlan: there were no unsatisfied goals found");
                return false;
            }

            foreach (IGoapGoal goapGoal in goals) {
                List<AbstractGoapAction> tempPlan = CreateNewPlan(goapGoal);

                if (tempPlan.Count > 0) {
                    _currentGoal = goapGoal;
                    _currentPlan = tempPlan;
                    return true;
                }
            }
            GoapComponent.Log.Info("TryGetGoalAndPlan: there was no plan found for any unsatisfied goal");
            return false;
        }


        /// <summary>
        ///     create a new goap planner to create a new plan by the given goal
        /// </summary>
        private List<AbstractGoapAction> CreateNewPlan(IGoapGoal goal) {
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
        private IOrderedEnumerable<IGoapGoal> GetUnsatisfiedGoalsSortedByRelevancy() {
            List<IGoapGoal> notSatisfied = _availableGoals.FindAll
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
        ///     check if plan is not null or empty
        /// </summary>
        /// <returns></returns>
        private bool IsCurrentPlanAvailable() {
            return (_currentPlan != null && _currentPlan.Count > 0);
        }

        /// <summary>
        ///     call the update relavancy method on all known goals
        /// </summary>
        private void UpdateRelevancyOfGoals() {
            _availableGoals.ForEach(x => x.UpdateRelevancy(_internalBlackboard.Get(Worldstate)));
        }
    }

}
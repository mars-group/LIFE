using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;
using TypeSafeBlackboard;

namespace GoapActionSystem.Implementation {
    public class GoapManager : AbstractGoapSystem {
        private readonly List<AbstractGoapAction> _availableActions;
        private readonly List<IGoapGoal> _availableGoals;
        private readonly Blackboard _internalBlackboard;

        /// <summary>
        ///     goap element: faster search for action satisfying needed worldstates
        /// </summary>
        private Dictionary<IGoapWorldProperty, List<AbstractGoapAction>> _effectToAction;

        private List<AbstractGoapAction> _currentPlan = new List<AbstractGoapAction>();
        private IGoapGoal _currentGoal;

        /// <summary>
        ///     create the manager and add worldstates later from sensors or other source
        /// </summary>
        /// <param name="availableActions"></param>
        /// <param name="availableGoals"></param>
        /// <param name="blackboard"></param>
        internal GoapManager
            (List<AbstractGoapAction> availableActions, List<IGoapGoal> availableGoals, Blackboard blackboard) {
            _availableActions = availableActions;
            _availableGoals = availableGoals;
            _internalBlackboard = blackboard;
            if (IsEmptyParam(availableActions) || IsEmptyParam(availableGoals))
                throw new ArgumentException("GoapManager: Goap manager cannot start with empty goal or action list");
            CreateWorldstatesByNeeds();
            CreateEffectActionHashTable();
            ChooseNewGoal();
        }

        /// <summary>
        ///     create the manager inklusive the current worldstates
        /// </summary>
        /// <param name="availableActions"></param>
        /// <param name="availableGoals"></param>
        /// <param name="startStates"></param>
        /// <param name="blackboard"></param>
        internal GoapManager
            (List<AbstractGoapAction> availableActions,
                List<IGoapGoal> availableGoals,
                Blackboard blackboard,
                List<IGoapWorldProperty> startStates) {
            if (IsEmptyParam(availableActions) || IsEmptyParam(availableGoals))
                throw new ArgumentException("GoapManager: Goap manager cannot start with empty goal or action list");
            _availableActions = availableActions;
            _availableGoals = availableGoals;
            _internalBlackboard = blackboard;
            _internalBlackboard.Set(Worldstate, startStates);
            CreateEffectActionHashTable();
            ChooseNewGoal();
        }

        private bool IsEmptyParam(IList paramList) {
            return paramList.Count == 0;
        }

        /// <summary>
        ///     entry point for user of goap services and main method
        /// </summary>
        /// <returns></returns>
        public override AbstractGoapAction GetNextAction() {
            if (GoalIsReached()) {
                Console.WriteLine("Goal " + _currentGoal.GetType() + " is reached.");
                ChooseNewGoal();
                CreateNewPlan();
            }
            else if (!IsPlanValid()) {
                ChooseNewGoal();
                CreateNewPlan();
            }

            if (HasPlan()) {
                AbstractGoapAction currentAction = _currentPlan.First();
                _internalBlackboard.Set(ActionForExecution, currentAction);
                _currentPlan.RemoveAt(0);
                return currentAction;
            }

            return new SurrogateAction();
        }

        private void CreateNewPlan() {
            GoapPlanner planner = new GoapPlanner(20, _availableActions, _effectToAction, _internalBlackboard.Get(Worldstate));
            _currentPlan = planner.GetPlan(_currentGoal);
        }

        private bool GoalIsReached() {
            if (HasGoal()) return _currentGoal.IsSatisfied(_internalBlackboard.Get(Worldstate));
            return false;
        }

        private void CreateEffectActionHashTable() {
            _effectToAction = new Dictionary<IGoapWorldProperty, List<AbstractGoapAction>>();
            foreach (AbstractGoapAction action in _availableActions) {
                foreach (IGoapWorldProperty effect in action.Effects) {
                    if (_effectToAction.ContainsKey(effect)) _effectToAction[effect].Add(action);
                    else _effectToAction[effect] = new List<AbstractGoapAction> {action};
                }
            }
        }

        private IGoapGoal ChooseNewGoal() {
            UpdateRelevancyOfGoals();
            IGoapGoal highestRelevancyGoal = _availableGoals.OrderByDescending(x => x.GetRelevancy()).ToList().First();
            if (_currentGoal != null && _currentGoal.Equals(highestRelevancyGoal)) return _currentGoal;

            return _currentGoal = highestRelevancyGoal;
        }

        private void CreateWorldstatesByNeeds() {
            IEnumerable<Type> allTypes = GetNeededWorldstates();
            List<IGoapWorldProperty> currentWorldstates = new List<IGoapWorldProperty>();
            foreach (Type type in allTypes) {
                object[] args = {false};
                IGoapWorldProperty instance = (IGoapWorldProperty) Activator.CreateInstance(type, args);
                currentWorldstates.Add(instance);
            }
            _internalBlackboard.Set(Worldstate, currentWorldstates);
        }

        /// <summary>
        ///     get the needed types of all worldstates by the used goals and actions - testing method
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Type> GetNeededWorldstates() {
            HashSet<Type> allTypes = new HashSet<Type>();
            foreach (AbstractGoapAction availableAction in _availableActions) {
                allTypes.UnionWith(availableAction.GetAffectingWorldstateTypes());
            }
            foreach (IGoapGoal availableGoal in _availableGoals) {
                allTypes.UnionWith(availableGoal.GetAffectingWorldstateTypes());
            }
            return allTypes;
        }

        private bool IsPlanValid() {
            return HasPlan() && IsPlanExecutable();
        }

        private bool IsPlanExecutable() {
            return true;
        }

        private bool HasPlan() {
            return (_currentPlan.Count > 0);
        }

        private bool IsGoalValid() {
            UpdateRelevancyOfGoals();
            if (HasGoal())
                if (_availableGoals.All(goal => goal.GetRelevancy() <= _currentGoal.GetRelevancy())) return true;
            return false;
        }

        private bool HasGoal() {
            return (_currentGoal != null);
        }

        private void UpdateRelevancyOfGoals() {
            _availableGoals.ForEach(x => x.UpdateRelevancy(_internalBlackboard.Get(Worldstate)));
        }
    }
}
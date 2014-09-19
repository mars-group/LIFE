using System;
using System.Collections.Generic;
using System.Linq;
using CommonTypes.Interfaces;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;

namespace GoapActionSystem.Implementation {
    /// <summary>
    ///     concrete class offering the neccessary methods
    /// </summary>
    public class GoapManager : IActionSystem {
        private readonly List<AbstractGoapAction> _availableActions;
        private readonly List<IGoapGoal> _availableGoals;
        private List<IGoapWorldstate> _neccessaryWorldstates;

        private readonly List<IGoapWorldstate> _currentWorldstates;
        private List<AbstractGoapAction> _currentPlan;
        private IGoapGoal _currentGoal;


        /// <summary>
        /// create the manager and add worldstates later from sensors or other source
        /// </summary>
        /// <param name="availableActions"></param>
        /// <param name="availableGoals"></param>
        public GoapManager(List<AbstractGoapAction> availableActions, List<IGoapGoal> availableGoals) {
            _availableActions = availableActions;
            _availableGoals = availableGoals;
            CreateWorldstatesByNeeds();
        }

        /// <summary>
        /// create  the manager inklusive the current worldstates
        /// </summary>
        /// <param name="availableActions"></param>
        /// <param name="availableGoals"></param>
        /// <param name="startStates"></param>
        public GoapManager(List<AbstractGoapAction> availableActions, List<IGoapGoal> availableGoals,
            List<IGoapWorldstate> startStates) {
            _availableActions = availableActions;
            _availableGoals = availableGoals;
            _currentWorldstates = startStates;
        }

        /// <summary>
        ///     entry point for user of goap services and main method
        /// </summary>
        /// <returns></returns>
        public IAction GetNextAction() {
            if (_currentPlan == null || _currentPlan.Count == 0) {
                var planner = new GoapPlanner(20, _availableActions);
                _currentPlan = planner.GetPlan(_currentWorldstates, GetGoal().GetTargetWorldstates());
            }

            return _currentPlan.First();
        }

        public bool PushIActionToBlackboard() {
            throw new NotImplementedException();
        }

        private IGoapGoal GetGoal() {
            if (_currentGoal == null) ChooseNewGoal();
            return _currentGoal;
        }

        /// <summary>
        ///     TODO dynamische GOAL Auswahl anhand von Prioritäten und Erreichbarkeit! Idee: sortierte Liste der Goals anhand von
        ///     Hirarchie
        /// </summary>
        /// <returns></returns>
        private IGoapGoal ChooseNewGoal() {
            try {
                return _currentGoal = _availableGoals[0];
            }
            catch (IndexOutOfRangeException) {
                throw new Exception("goap agent owns no goals. ");
            }
        }

        /// <summary>
        ///     get the needed types of all worldstates by the used goals and actions - testing method
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Type> GetNeededWorldstates() {
            var allTypes = new HashSet<Type>();

            foreach (var availableAction in _availableActions) {
                allTypes.UnionWith(availableAction.GetAffectingWorldstateTypes());
            }

            foreach (var availableGoal in _availableGoals) {
                allTypes.UnionWith(availableGoal.GetAffectingWorldstateTypes());
            }
            return allTypes;
        }

        private void CreateWorldstatesByNeeds() {
            IEnumerable<Type> allTypes = GetNeededWorldstates();

            _neccessaryWorldstates = new List<IGoapWorldstate>();

            foreach (var type in allTypes) {
                object[] args = {false};
                var instance = (IGoapWorldstate) Activator.CreateInstance(type, args);
                _neccessaryWorldstates.Add(instance);
            }
        }
    }
}
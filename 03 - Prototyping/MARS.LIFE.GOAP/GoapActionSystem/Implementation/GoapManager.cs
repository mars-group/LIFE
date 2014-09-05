using System;
using System.Collections.Generic;
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

        private List<IGoapWorldstate> _startWorldstates;


        public GoapManager() {}

        public GoapManager(List<AbstractGoapAction> availableActions, List<IGoapGoal> availableGoals) {
            _availableActions = availableActions;
            _availableGoals = availableGoals;
            CreateWorldstatesByNeeds();
        }

        public GoapManager(List<AbstractGoapAction> availableActions, List<IGoapGoal> availableGoals,
            List<IGoapWorldstate> startStates) {
            _availableActions = availableActions;
            _availableGoals = availableGoals;
            _startWorldstates = startStates;
            CreateWorldstatesByNeeds();
        }


        public IAction GetNextAction() {
            // 

            throw new NotImplementedException();
        }

        public bool PushIActionToBlackboard() {
            throw new NotImplementedException();
        }


        private void CreateWorldstatesByNeeds() {
            var allTypes = new HashSet<Type>();

            foreach (var availableAction in _availableActions) {
                allTypes.UnionWith(availableAction.GetAffectingWorldstateTypes());
            }

            foreach (var availableGoal in _availableGoals) {
                allTypes.UnionWith(availableGoal.GetAffectingWorldstateTypes());
            }

            _neccessaryWorldstates = new List<IGoapWorldstate>();

            foreach (var type in allTypes) {
                object[] args = {false};
                var instance = (IGoapWorldstate) Activator.CreateInstance(type, args);
                _neccessaryWorldstates.Add(instance);
            }
        }
    }
}
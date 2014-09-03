using System;
using System.Collections.Generic;
using CommonTypes.Interfaces;
using GoapCommon.Interfaces;


namespace GoapActionSystem.Implementation
{
    /// <summary>
    /// concrete class offering the neccessary methods
    /// </summary>
    public class GoapManager : IActionSystem {

        private List<IGoapAction> _availableActions;
        private List<IGoapGoal> _availableGoals;

        private List<IGoapWorldstate> _neccessaryWorldstates; 


        public GoapManager() {}

        public GoapManager(List<IGoapAction> availableActions, List<IGoapGoal> availableGoals) {
            _availableActions = availableActions;
            _availableGoals = availableGoals;
        }


        public IAction GetNextAction() {
            throw new NotImplementedException();
        }

        public bool PushIActionToBlackboard() {
            throw new NotImplementedException();
        }

        


    }
}

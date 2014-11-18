using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapCommon.Interfaces;

namespace SimPanInGoapModelDefinition {

    /// <summary>
    ///     determines the configuration of agent 1
    /// </summary>
    public class AgentConfig1 : IGoapAgentConfig {
        #region IGoapAgentConfig Members

        public List<WorldstateSymbol> GetStartWorldstate() {
            return new List<WorldstateSymbol> {};
        }

        public List<AbstractGoapAction> GetAllActions() {
            throw new NotImplementedException();
        }

        public List<IGoapGoal> GetAllGoals() {
            throw new NotImplementedException();
        }

        public int GetMaxGraphSearchDepth(){
            return 20;
        }

        #endregion
    }

}
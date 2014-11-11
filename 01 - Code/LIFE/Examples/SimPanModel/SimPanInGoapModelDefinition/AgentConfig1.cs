using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;
using SimPanInGoapModelDefinition.Actions;
using SimPanInGoapModelDefinition.Goals;

namespace SimPanInGoapModelDefinition {

    /// <summary>
    ///     determines the configuration of agent 1
    /// </summary>
    public class AgentConfig1 : IGoapAgentConfig {

        public List<IGoapWorldProperty> GetStartWorldstate() {
            return new List<IGoapWorldProperty> {};
        }

        public List<AbstractGoapAction> GetAllActions() {
            throw new NotImplementedException();
        }

        public List<IGoapGoal> GetAllGoals() {
            throw new NotImplementedException();
        }
    }
}
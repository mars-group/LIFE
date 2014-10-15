using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;
using GOAPModelDefinition.Actions;
using GOAPModelDefinition.Goals;
using GOAPModelDefinition.Worldstates;

namespace GOAPModelDefinition {

    /// <summary>
    ///     determines the configuration of agent 1
    /// </summary>
    public class AgentConfig1 : IAgentConfig {

        public List<IGoapWorldProperty> GetStartWorldstate() {
            return new List<IGoapWorldProperty> {new IsHappy(false), new HasMoney(true), new HasToy(false)};
        }

        public List<AbstractGoapAction> GetAllActions() {
            return new List<AbstractGoapAction> {new ActionClean(), new ActionGetToy(), new ActionPlay()};
        }

        public List<IGoapGoal> GetAllGoals() {
            return new List<IGoapGoal> {new GoalBeHappy()};
        }
    }
}
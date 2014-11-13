using System.Collections.Generic;
using GOAPBetaModelDefinition.Actions;
using GOAPBetaModelDefinition.Goals;
using GOAPBetaModelDefinition.Worldstates;
using GoapBetaCommon.Abstract;
using GoapBetaCommon.Interfaces;

namespace GOAPBetaModelDefinition {

    /// <summary>
    ///     determines the configuration of agent 1
    /// </summary>
    public class AgentConfig1 : IGoapAgentConfig {

        public List<IGoapWorldProperty> GetStartWorldstate() {
            return new List<IGoapWorldProperty> {new IsHappy(false), new HasMoney(true), new HasToy(false)};
        }

        public List<AbstractGoapAction> GetAllActions() {
            return new List<AbstractGoapAction> {new ActionClean(), new ActionGetToy(), new ActionPlay()};
        }

        public List<IGoapGoal> GetAllGoals() {
            return new List<IGoapGoal> {new GoalBeHappy(), new GoalGetRich()};
        }
    }
}
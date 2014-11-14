using System.Collections.Generic;
using GoapBetaCommon.Abstract;
using GoapBetaCommon.Interfaces;
using GoapModelTest.Actions;
using GoapModelTest.Goals;
using GoapModelTest.Worldstates;

namespace GoapModelTest {
    public class AgentTestConfig2 : IGoapAgentConfig
    {
        public List<IGoapWorldProperty> GetStartWorldstate() {
            return new List<IGoapWorldProperty> {new IsHappy(false), new HasMoney(false), new HasToy(true)};
        }

        public List<AbstractGoapAction> GetAllActions() {
            return new List<AbstractGoapAction> {new ActionClean(), new ActionGetToy(), new ActionPlay()};
        }

        public List<IGoapGoal> GetAllGoals() {
            return new List<IGoapGoal> {new GoalBeHappy()};
        }
    }
}
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;
using GoapModelTest.Actions;
using GoapModelTest.Goals;
using GoapModelTest.Worldstates;

namespace GoapModelTest {
    public class AgentTestConfig2 : IAgentConfig
    {
        public List<IGoapWorldProperty> GetStartWorldstate() {
            return new List<IGoapWorldProperty> {new Happy(false), new HasMoney(false), new HasToy(true)};
        }

        public List<AbstractGoapAction> GetAllActions() {
            return new List<AbstractGoapAction> {new ActionClean(), new ActionGetToy(), new ActionPlay()};
        }

        public List<IGoapGoal> GetAllGoals() {
            return new List<IGoapGoal> {new GoalBeHappy()};
        }
    }
}
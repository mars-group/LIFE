using System;
using System.Collections.Generic;
using GoapBetaCommon.Abstract;
using GoapBetaCommon.Implementation;
using GoapBetaCommon.Interfaces;
using GoapModelTest.Actions;
using GoapModelTest.Goals;
using GoapModelTest.Worldstates;

namespace GoapModelTest {

    /// <summary>
    ///     determines the configuration of agent 1
    /// </summary>
    public class AgentConfig1 : IGoapAgentConfig {

        public List<WorldstateSymbol> GetStartWorldstate(){
           
                List<WorldstateSymbol> symbols = new List<WorldstateSymbol> {
                    new WorldstateSymbol(WorldProperties.Happy, false, typeof (Boolean)),
                    new WorldstateSymbol(WorldProperties.HasMoney, true, typeof (Boolean)),
                    new WorldstateSymbol(WorldProperties.Happy, false, typeof (Boolean))
                };

            return symbols;
        }

        public List<AbstractGoapAction> GetAllActions() {
            return new List<AbstractGoapAction> {new ActionClean(), new ActionGetToy(), new ActionPlay()};
        }

        public List<IGoapGoal> GetAllGoals() {
            return new List<IGoapGoal> {new GoalBeHappy(), new GoalGetRich()};
        }
    }



   
}
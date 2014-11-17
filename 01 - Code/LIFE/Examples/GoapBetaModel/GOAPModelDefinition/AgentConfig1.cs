using System;
using System.Collections.Generic;
using GoapBetaCommon.Abstract;
using GoapBetaCommon.Implementation;
using GoapBetaCommon.Interfaces;
using GOAPBetaModelDefinition.Actions;
using GOAPBetaModelDefinition.Goals;
using GOAPBetaModelDefinition.Worldstates;

namespace GOAPBetaModelDefinition {

    /// <summary>
    ///     determines a configuration of an agent
    /// </summary>
    public class AgentConfig1 : IGoapAgentConfig {
        #region IGoapAgentConfig Members

        public List<WorldstateSymbol> GetStartWorldstate() {
            List<WorldstateSymbol> symbols = new List<WorldstateSymbol>();
            symbols.Add(new WorldstateSymbol(WorldProperties.Happy, false, typeof (Boolean)));
            symbols.Add(new WorldstateSymbol(WorldProperties.HasMoney, true, typeof (Boolean)));
            symbols.Add(new WorldstateSymbol(WorldProperties.Happy, false, typeof (Boolean)));

            return symbols;
            //return new Worldstate(typeof(WorldProperties), symbols);
        }

        public List<AbstractGoapAction> GetAllActions() {
            return new List<AbstractGoapAction> {
                new ActionClean(),
                new ActionBuyToy(),
                //new ActionStealToy(),
                new ActionBuyFood(),
                new ActionPlayWithToy(),
                new ActionEat()
            };
        }

        public List<IGoapGoal> GetAllGoals() {
            return new List<IGoapGoal> {new GoalBeHappy(), new GoalGetRich()};
        }

        #endregion
    }

}
using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapCommon.Interfaces;
using GOAPModelDefinition.Actions;
using GOAPModelDefinition.Goals;
using GOAPModelDefinition.Worldstates;

namespace GOAPModelDefinition {

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
        }

        public List<AbstractGoapAction> GetAllActions() {
            return new List<AbstractGoapAction> {
                new ActionClean(),
                new ActionBuyToy(),
                new ActionPlayWithToy(),
            };
        }

        public List<AbstractGoapGoal> GetAllGoals() {
            return new List<AbstractGoapGoal> {new GoalBeHappy(), new GoalGetRich()};
        }

        public int GetMaxGraphSearchDepth() {
            return 20;
        }

        public bool IgnoreActionsIsFinished() {
            return true;
        }

        public bool ForceSymbolsUpdateBeforePlanning() {
            return false;
        }

        public bool ForceSymbolsUpdateEveryActionRequest() {
            return false;
        }

        public bool ForceGoalRelevancyUpdateBeforePlanning() {
            return false;
        }

        public List<WorldstateSymbol> GetUpdatedSymbols() {
            throw new NotImplementedException();
        }

        #endregion
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapCommon.Interfaces;
using GoapModelTest.Actions;
using GoapModelTest.Goals;
using GoapModelTest.Worldstates;

namespace GoapModelTest
{
    class AgentTestConfigSwitchGoal : IGoapAgentConfig
    {
        #region IGoapAgentConfig Members

        public List<WorldstateSymbol> GetStartWorldstate()
        {
            List<WorldstateSymbol> symbols = new List<WorldstateSymbol> {
                new WorldstateSymbol(WorldProperties.Happy, false, typeof (Boolean)),
                new WorldstateSymbol(WorldProperties.HasMoney, true, typeof (Boolean)),
                new WorldstateSymbol(WorldProperties.HasToy, false, typeof (Boolean))
            };

            return symbols;
        }

        public List<AbstractGoapAction> GetAllActions()
        {
            return new List<AbstractGoapAction> { new ActionClean(), new ActionGetToy(), new ActionPlay() };
        }

        public List<AbstractGoapGoal> GetAllGoals()
        {
            return new List<AbstractGoapGoal> { new GoalBeHappy(), new GoalGetRich() };
        }

        public int GetMaxGraphSearchDepth()
        {
            return 20;
        }

        public bool ForceSymbolsUpdateBeforePlanning()
        {
            return false;
        }

        public bool ForceSymbolsUpdateEveryActionRequest()
        {
            return false;
        }

        public List<WorldstateSymbol> GetUpdatedSymbols()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
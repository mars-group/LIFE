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

namespace GoapModelTest
{
    class AgentTestConfigProvokeSurrogateAction :IGoapAgentConfig
    {
        public List<WorldstateSymbol> GetStartWorldstate() {
            return new List<WorldstateSymbol>();
        }

        public List<AbstractGoapAction> GetAllActions() {
            return  new List<AbstractGoapAction>{new AToB()};
        }

        public List<AbstractGoapGoal> GetAllGoals() {
            return new List<AbstractGoapGoal>{new GoalH()};
        }

        public int GetMaxGraphSearchDepth() {
            return 10;
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
            return GetStartWorldstate();
        }
    }
}

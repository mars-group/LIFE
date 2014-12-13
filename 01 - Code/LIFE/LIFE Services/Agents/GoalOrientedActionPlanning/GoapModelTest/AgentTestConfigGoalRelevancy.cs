using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapCommon.Interfaces;
using GoapModelTest.Actions;
using GoapModelTest.Goals;

namespace GoapModelTest {

    internal class AgentTestConfigGoalRelevancy : IGoapAgentConfig {
        #region IGoapAgentConfig Members

        public List<WorldstateSymbol> GetStartWorldstate() {
            return new List<WorldstateSymbol>();
        }

        public List<AbstractGoapAction> GetAllActions() {
            return new List<AbstractGoapAction> {
                new ToA(),
                new AToB(),
                new BToC(),

                new ToF(),
                new FToG(),
                new GToH(),
            };
        }

        public List<AbstractGoapGoal> GetAllGoals() {
            return new List<AbstractGoapGoal> {new GoalC(), new GoalH()};
        }

        public int GetMaxGraphSearchDepth() {
            return 10;
        }

        public bool ForceSymbolsUpdateBeforePlanning() {
            return true;
        }

        public bool ForceSymbolsUpdateEveryActionRequest() {
            return false;
        }

        public List<WorldstateSymbol> GetUpdatedSymbols() {
            return GetStartWorldstate();
        }

        #endregion
    }

}
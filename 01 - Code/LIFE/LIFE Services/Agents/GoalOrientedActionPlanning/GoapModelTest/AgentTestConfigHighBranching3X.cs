using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapCommon.Interfaces;
using GoapModelTest.Actions;
using GoapModelTest.Goals;
using GoapModelTest.Worldstates;

namespace GoapModelTest {


    /// <summary>
    ///     In this configuration there are produced 34 nodes for search in graph.
    /// </summary>
    internal class AgentTestConfigHighBranching3X : IGoapAgentConfig {
        #region IGoapAgentConfig Members

        public List<WorldstateSymbol> GetStartWorldstate() {
            return new List<WorldstateSymbol> {
                new WorldstateSymbol(WorldProperties.A, false, typeof (Boolean)),
                new WorldstateSymbol(WorldProperties.B, false, typeof (Boolean)),
                new WorldstateSymbol(WorldProperties.C, false, typeof (Boolean)),
            };
        }

        public List<AbstractGoapAction> GetAllActions() {
            return new List<AbstractGoapAction> {
                new HighBranchingA(),
                new HighBranchingB(),
                new HighBranchingC(),
            };
        }

        public List<AbstractGoapGoal> GetAllGoals() {
            return new List<AbstractGoapGoal> {new GoalHighBranching3X()};
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

        #endregion
    }

}
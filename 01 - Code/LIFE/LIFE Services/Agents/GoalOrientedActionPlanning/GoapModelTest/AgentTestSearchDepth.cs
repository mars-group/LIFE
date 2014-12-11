using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapCommon.Interfaces;
using GoapModelTest.Actions;
using GoapModelTest.Goals;
using GoapModelTest.Worldstates;

namespace GoapModelTest {

    internal class AgentTestSearchDepth : IGoapAgentConfig {
        #region IGoapAgentConfig Members

        public List<WorldstateSymbol> GetStartWorldstate() {
            return new List<WorldstateSymbol> {
                new WorldstateSymbol(WorldProperties.A, true, typeof (Boolean)),
                new WorldstateSymbol(WorldProperties.B, false, typeof (Boolean)),
                new WorldstateSymbol(WorldProperties.C, false, typeof (Boolean)),
                new WorldstateSymbol(WorldProperties.D, false, typeof (Boolean)),
                new WorldstateSymbol(WorldProperties.E, false, typeof (Boolean)),
                new WorldstateSymbol(WorldProperties.F, false, typeof (Boolean)),
                new WorldstateSymbol(WorldProperties.G, false, typeof (Boolean)),
                new WorldstateSymbol(WorldProperties.H, false, typeof (Boolean)),
                new WorldstateSymbol(WorldProperties.I, false, typeof (Boolean)),
                new WorldstateSymbol(WorldProperties.J, false, typeof (Boolean)),
            };
        }

        public List<AbstractGoapAction> GetAllActions() {
            return new List<AbstractGoapAction> {
                new AToB(),
                new BToC(),
                new CToD(),
                new DToE(),
                new EToF(),
                new FToG(),
                new GToH(),
                new HToI(),
                new IToJ()
            };
        }

        public List<IGoapGoal> GetAllGoals() {
            return new List<IGoapGoal> {new GoalSearchDeptLimitTest()};
        }

        public int GetMaxGraphSearchDepth() {
            return 5;
        }

        public bool ForceSymbolsUpdateBeforePlanning() {
            return false;
        }

        public bool ForceSymbolsUpdateEveryActionRequest() {
            return false;
        }

        public List<WorldstateSymbol> GetUpdatedSymbols() {
            throw new NotImplementedException();
        }

        #endregion
    }

}
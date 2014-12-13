using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapModelTest.Worldstates;

namespace GoapModelTest.Goals {

    internal class GoalHighBranching5X : AbstractGoapGoal {
        public GoalHighBranching5X()
            : base(new List<WorldstateSymbol> {
                new WorldstateSymbol(WorldProperties.A, true, typeof (Boolean)),
                new WorldstateSymbol(WorldProperties.B, true, typeof (Boolean)),
                new WorldstateSymbol(WorldProperties.C, true, typeof (Boolean)),
                new WorldstateSymbol(WorldProperties.D, true, typeof (Boolean)),
                new WorldstateSymbol(WorldProperties.E, true, typeof (Boolean)),
            },
                1) {}

        public override int UpdateRelevancy(List<WorldstateSymbol> actualWorldstate) {
            return 1;
        }
    }

}
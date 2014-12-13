using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapModelTest.Worldstates;

namespace GoapModelTest.Goals {

    internal class GoalC : AbstractGoapGoal {
        public GoalC()
            : base(new List<WorldstateSymbol> {
                new WorldstateSymbol(WorldProperties.C, true, typeof (Boolean))
            },6) {}

        public override void UpdateRelevancy(List<WorldstateSymbol> actualWorldstate) {

            if (actualWorldstate.Contains(new WorldstateSymbol(WorldProperties.H, true, typeof (Boolean)))) {
                Relevancy = 10;
            }
            else {
                Relevancy = 6;
            }
        }
    }

}
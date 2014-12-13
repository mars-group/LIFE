using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapModelTest.Worldstates;

namespace GoapModelTest.Goals {

    internal class GoalH : AbstractGoapGoal {
        public GoalH()
            : base(new List<WorldstateSymbol> {
                new WorldstateSymbol(WorldProperties.H, true, typeof (Boolean))
            }, 5) {}


        public override void UpdateRelevancy(List<WorldstateSymbol> actualWorldstate) {
            if (actualWorldstate.Contains(new WorldstateSymbol(WorldProperties.C, true, typeof (Boolean)))) {
                Relevancy = 10;
            }
            else {
                Relevancy = 5;
            }
        }
    }

}
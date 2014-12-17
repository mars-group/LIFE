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
            }, 6) {}


        public override void UpdateRelevancy(List<WorldstateSymbol> actualWorldstate) {
            switch (Relevancy) {
                case 6:
                    Relevancy = 1;
                    break;
                case 1:
                    Relevancy = 6;
                    break;
            }
        }
    }

}
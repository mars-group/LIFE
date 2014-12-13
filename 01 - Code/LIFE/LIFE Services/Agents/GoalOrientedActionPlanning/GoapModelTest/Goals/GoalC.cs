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
            },5) {}

        public override void UpdateRelevancy(List<WorldstateSymbol> actualWorldstate) {

            switch (Relevancy) {
                case 5:
                    Relevancy = 10;
                    break;
                case 10:
                    Relevancy = 5;
                    break;
            }
        }
    }

}
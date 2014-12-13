using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapModelTest.Worldstates;

namespace GoapModelTest.Goals {

    public class GoalBeHappy : AbstractGoapGoal {
        public GoalBeHappy()
            : base(new List<WorldstateSymbol> {
                new WorldstateSymbol(WorldProperties.Happy, true, typeof (Boolean))
            },5) {}

        public override int UpdateRelevancy(List<WorldstateSymbol> actualWorldstate) {
            return Relevancy;
        }
    }

}
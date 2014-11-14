using System;
using System.Collections.Generic;
using GoapBetaCommon.Abstract;
using GoapBetaCommon.Implementation;
using GOAPBetaModelDefinition.Worldstates;

namespace GOAPBetaModelDefinition.Goals {

    public class GoalBeHappy : AbstractGoapGoal {
        public GoalBeHappy()
            : base(new List<WorldstateSymbol> {
                new WorldstateSymbol(WorldProperties.Happy, true, typeof (Boolean))
            },
                2) {}

        public override int UpdateRelevancy(List<WorldstateSymbol> actualWorldstate) {
            if (IsSatisfied(actualWorldstate)) {
                return Relevancy = 0;
            }
            else {
                if (Relevancy < 10) {
                    return Relevancy += 1;
                }
                else {
                    return Relevancy;
                }
            }
        }
    }

}
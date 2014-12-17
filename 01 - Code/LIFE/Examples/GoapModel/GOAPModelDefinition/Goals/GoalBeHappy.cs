using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GOAPModelDefinition.Worldstates;

namespace GOAPModelDefinition.Goals {

    public class GoalBeHappy : AbstractGoapGoal {
        public GoalBeHappy()
            : base(new List<WorldstateSymbol> {
                new WorldstateSymbol(WorldProperties.Happy, true, typeof (Boolean))
            }, 2) {}

        public override void UpdateRelevancy(List<WorldstateSymbol> actualWorldstate) {
            if (IsSatisfied(actualWorldstate)) {
                Relevancy = 0;
            }
            else {
                if (Relevancy < 10) {
                    Relevancy += 1;
                }
            }
        }
    }

}
using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapModelTest.Worldstates;

namespace GoapModelTest.Goals {

    public class MisdesignedTargetstateGoal : AbstractGoapGoal {
        public MisdesignedTargetstateGoal()
            : base(new List<WorldstateSymbol> {
                new WorldstateSymbol(WorldProperties.HasMoney, true, typeof (Boolean)),
                new WorldstateSymbol(WorldProperties.HasMoney, true, typeof (Boolean))
            },
                1) {}

        public override void UpdateRelevancy(List<WorldstateSymbol> actualWorldstate) {}
    }

}
using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapModelTest.Worldstates;

namespace GoapModelTest.Goals {

    internal class GoalSearchDeptLimitTest : AbstractGoapGoal {
        public GoalSearchDeptLimitTest()
            : base(
                new List<WorldstateSymbol> {new WorldstateSymbol(WorldProperties.J, true, typeof (Boolean))},
                1) {}

    }

}
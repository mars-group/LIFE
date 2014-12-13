using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapModelTest.Worldstates;

namespace GoapModelTest.Goals
{
    class GoalHighBranching3X : AbstractGoapGoal {
        public GoalHighBranching3X()
            : base(new List<WorldstateSymbol> {
                new WorldstateSymbol(WorldProperties.A, true, typeof (Boolean)),
                new WorldstateSymbol(WorldProperties.B, true, typeof (Boolean)),
                new WorldstateSymbol(WorldProperties.C, true, typeof (Boolean)),
            },1) {}
    }
}
using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;

namespace SimPanInGoapModelDefinition.Goals {

    internal class CalmDown : AbstractGoapGoal {
        public CalmDown(List<WorldstateSymbol> targetWorldState, int startRelevancy)
            : base(targetWorldState, startRelevancy) {}


    }

}
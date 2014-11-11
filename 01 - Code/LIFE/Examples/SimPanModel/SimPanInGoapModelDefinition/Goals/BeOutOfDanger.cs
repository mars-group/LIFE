using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;

namespace SimPanInGoapModelDefinition.Goals {

    public class BeOutOfDanger : AbstractGoapGoal {
        public BeOutOfDanger(List<IGoapWorldProperty> targetWorldState, int startRelevancy) :
            base(targetWorldState, startRelevancy) {}

        public override int UpdateRelevancy(List<IGoapWorldProperty> actualWorldstate) {
            return 10;
        }
    }
}
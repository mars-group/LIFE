using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;

namespace SimPanInGoapModelDefinition.Goals {

    public class BeOutOfDanger : AbstractGoapGoal {
        public BeOutOfDanger(List<WorldstateSymbol> targetWorldState, int startRelevancy) :
            base(targetWorldState, startRelevancy) {}


        public override int UpdateRelevancy(List<WorldstateSymbol> actualWorldstate) {
            return 10;
        }
    }

}
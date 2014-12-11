using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using SimPanInGoapModelDefinition.Worldstates;

namespace SimPanInGoapModelDefinition.Goals {

    public class BeOutOfDanger : AbstractGoapGoal {
        public BeOutOfDanger() :
            base(
            new List<WorldstateSymbol> {new WorldstateSymbol(Properties.IsOutSide, true, typeof (Boolean))}
            ,
            1) {}
        
        public override int UpdateRelevancy(List<WorldstateSymbol> actualWorldstate) {
            return 10;
        }
    }

}
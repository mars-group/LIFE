using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;

namespace SimPanInGoapModelDefinition.Goals
{
    internal class CalmDown : AbstractGoapGoal{

        public CalmDown(List<IGoapWorldProperty> targetWorldState, int startRelevancy) : base(targetWorldState, startRelevancy) {}
        
        public override int UpdateRelevancy(List<IGoapWorldProperty> actualWorldstate) {
            throw new NotImplementedException();
        }
    }
}

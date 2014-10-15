using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;
using GOAPModelDefinition.Worldstates;

namespace GOAPModelDefinition.Goals
{
    public class GoalGetRich : AbstractGoapGoal
    {
        public GoalGetRich() 
            : base(new List<IGoapWorldProperty>{new HasMoney(true)}, 1) {}

        public override int UpdateRelevancy(List<IGoapWorldProperty> actualWorldstate) {
            throw new NotImplementedException();
        }
    }
}

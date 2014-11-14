using System;
using System.Collections.Generic;
using System.Linq;
using GoapBetaCommon.Abstract;
using GoapBetaCommon.Interfaces;
using GoapModelTest.Worldstates;

namespace GOAPModelDefinition.Goals
{
    public class GoalGetRich : AbstractGoapGoal
    {
        public GoalGetRich() 
            : base(new List<IGoapWorldProperty>{new HasMoney(true)}, 1) {}

        public override int UpdateRelevancy(List<IGoapWorldProperty> actualWorldstate) {
            if (IsSatisfied(actualWorldstate))
            {
                return Relevancy = 0;
            }
            else
            {
                if (Relevancy < 10)
                {
                    return Relevancy += 1;
                }
                else
                {
                    return Relevancy;
                }
            }
        }
    }
}

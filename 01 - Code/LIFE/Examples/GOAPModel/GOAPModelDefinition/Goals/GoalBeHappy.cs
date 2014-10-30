using System;
using System.Collections.Generic;
using System.Linq;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;
using GOAPModelDefinition.Worldstates;

namespace GOAPModelDefinition.Goals {
    public class GoalBeHappy : AbstractGoapGoal {

        public GoalBeHappy() 
            : base(new List<IGoapWorldProperty>{new IsHappy(true)}, 2) {}
        
        public override int UpdateRelevancy(List<IGoapWorldProperty> actualWorldstate) {
            if (IsSatisfied(actualWorldstate)) {
                return Relevancy = 0;
            }else {
                if (Relevancy < 10) {
                    return Relevancy += 1;
                }else {
                    return Relevancy;
                }
            }
        }

       

       
    }
}
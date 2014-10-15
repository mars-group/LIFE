using System;
using System.Collections.Generic;
using System.Linq;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;
using GOAPModelDefinition.Worldstates;

namespace GOAPModelDefinition.Goals {
    public class GoalBeHappy : AbstractGoapGoal {

        public GoalBeHappy() 
            : base(new List<IGoapWorldProperty>{new IsHappy(true)}, 1) {}
        
        public override int UpdateRelevancy(List<IGoapWorldProperty> actualWorldstate) {
            return Relevancy = 5;
        }

       

       
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoapCommon.Interfaces;

namespace GoapModelTest.Goals
{
    public class GoalBeHappy : IGoapGoal {

        private int _relevancy;

        public GoalBeHappy(List<IGoapWorldstate> startWorldstate) {
            _relevancy = UpdateRelevancy(startWorldstate);
        }

        public bool IsSatisfied(List<IGoapWorldstate> worldstate) {
            throw new NotImplementedException();
        }

        public int GetRelevancy() {
            return _relevancy;
        }

        public int UpdateRelevancy(List<IGoapWorldstate> actualWorldstate) {
            _relevancy = actualWorldstate.Count;
            return GetRelevancy();
        }
    }
}

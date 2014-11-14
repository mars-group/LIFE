using System;
using System.Collections.Generic;
using System.Linq;
using GoapBetaCommon.Interfaces;
using GoapModelTest.Worldstates;

namespace GoapModelTest.Goals {
    public class GoalBeHappy : IGoapGoal {
        /// <summary>
        /// value may be in range from 1 to 10
        /// </summary>
        private int _relevancy = 5;
        private readonly List<IGoapWorldProperty> _fullfilledBy = new List<IGoapWorldProperty> {new IsHappy(true)};


        public bool IsSatisfied(List<IGoapWorldProperty> worldstate) {
            return (_fullfilledBy.Where(x => worldstate.Contains(x)).Count() == _fullfilledBy.Count());
        }

        public int GetRelevancy() {
            return _relevancy;
        }

        public int UpdateRelevancy(List<IGoapWorldProperty> actualWorldstate) {
            return 5;
        }

        public ISet<Type> GetAffectingWorldstateTypes() {
            var types = new HashSet<Type>();

            foreach (var goapWorldstate in _fullfilledBy) {
                types.Add(goapWorldstate.GetType());
            }

            return types;
        }

        public List<IGoapWorldProperty> GetTargetWorldstates() {
            return _fullfilledBy;
        }
    }
}
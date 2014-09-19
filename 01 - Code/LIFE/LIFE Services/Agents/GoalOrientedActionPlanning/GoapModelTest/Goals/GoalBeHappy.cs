using System;
using System.Collections.Generic;
using System.Linq;
using GoapCommon.Interfaces;
using GoapModelTest.Worldstates;

namespace GoapModelTest.Goals {
    public class GoalBeHappy : IGoapGoal {
        /// <summary>
        /// value may be in range from 1 to 10
        /// </summary>
        private int _relevancy = 5;
        private readonly List<IGoapWorldstate> _fullfilledBy = new List<IGoapWorldstate> {new Happy(true)};


        public bool IsSatisfied(List<IGoapWorldstate> worldstate) {
            return (_fullfilledBy.Where(x => worldstate.Contains(x)).Count() == _fullfilledBy.Count());
        }

        public int GetRelevancy() {
            return _relevancy;
        }

        public int UpdateRelevancy(List<IGoapWorldstate> actualWorldstate) {
            _relevancy = actualWorldstate.Count;
            return GetRelevancy();
        }

        public ISet<Type> GetAffectingWorldstateTypes() {
            var types = new HashSet<Type>();

            foreach (var goapWorldstate in _fullfilledBy) {
                types.Add(goapWorldstate.GetType());
            }

            return types;
        }

        public List<IGoapWorldstate> GetTargetWorldstates() {
            return _fullfilledBy;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using GoapCommon.Implementation;
using GoapCommon.Interfaces;

namespace GoapCommon.Abstract {

    public abstract class AbstractGoapGoal : IGoapGoal {
        protected readonly List<WorldstateSymbol> TargetWorldState;
        protected int Relevancy;

        protected AbstractGoapGoal(List<WorldstateSymbol> targetWorldState, int startRelevancy) {
            TargetWorldState = targetWorldState;
            Relevancy = startRelevancy;
        }

        #region IGoapGoal Members

        public int GetRelevancy() {
            return Relevancy;
        }

        public List<WorldstateSymbol> GetTargetWorldstates() {
            return TargetWorldState;
        }

        public bool IsSatisfied(List<WorldstateSymbol> worldstate) {
            return (TargetWorldState.Where(x => worldstate.Contains(x)).Count() == TargetWorldState.Count());
        }

        public ISet<Type> GetAffectingWorldstateTypes() {
            HashSet<Type> types = new HashSet<Type>();

            foreach (WorldstateSymbol goapWorldstate in TargetWorldState) {
                types.Add(goapWorldstate.GetType());
            }
            return types;
        }

        public abstract int UpdateRelevancy(List<WorldstateSymbol> actualWorldstate);

        #endregion
    }

}
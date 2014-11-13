using System;
using System.Collections.Generic;
using System.Linq;
using GoapBetaCommon.Interfaces;

namespace GoapBetaCommon.Abstract
{
    public abstract class AbstractGoapGoal :IGoapGoal {

        protected readonly List<IGoapWorldProperty> TargetWorldState;
        protected int Relevancy;

        protected AbstractGoapGoal(List<IGoapWorldProperty> targetWorldState, int startRelevancy) {
            TargetWorldState = targetWorldState;
            Relevancy = startRelevancy;
        }

        public int GetRelevancy(){
            return Relevancy;
        }

        public List<IGoapWorldProperty> GetTargetWorldstates(){
            return TargetWorldState;
        }

        public bool IsSatisfied(List<IGoapWorldProperty> worldstate) {
            return (TargetWorldState.Where(x => worldstate.Contains(x)).Count() == TargetWorldState.Count());
        }
        
        public ISet<Type> GetAffectingWorldstateTypes() {
            var types = new HashSet<Type>();

            foreach (var goapWorldstate in TargetWorldState){
                types.Add(goapWorldstate.GetType());
            }
            return types;
        }

        public abstract int UpdateRelevancy(List<IGoapWorldProperty> actualWorldstate);

    }
}

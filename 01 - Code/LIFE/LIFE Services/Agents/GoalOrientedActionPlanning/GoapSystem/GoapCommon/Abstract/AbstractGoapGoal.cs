using System;
using System.Collections.Generic;
using System.Linq;
using GoapCommon.Implementation;
using GoapCommon.Interfaces;

namespace GoapCommon.Abstract {

    /// <summary>
    ///     offers some ready done methods for creation of goap goals
    /// </summary>
    public abstract class AbstractGoapGoal : IGoapGoal {
        protected readonly List<WorldstateSymbol> TargetWorldState;
        protected int Relevancy;

        protected AbstractGoapGoal(List<WorldstateSymbol> targetWorldState, int startRelevancy) {
            TargetWorldState = targetWorldState;
            Relevancy = startRelevancy;
        }

        #region IGoapGoal Members

        /// <summary>
        ///     get the weight of this goal.
        ///     is needed for dicision of goap manager when it chooses one of the goals.
        /// </summary>
        /// <returns></returns>
        public int GetRelevancy() {
            return Relevancy;
        }

        /// <summary>
        ///     get the world state symbols defining the reached goal.
        /// </summary>
        /// <returns></returns>
        public List<WorldstateSymbol> GetTargetWorldstates() {
            return TargetWorldState;
        }

        /// <summary>
        ///     check the target world state against the given world state.
        /// </summary>
        /// <param name="worldstate"></param>
        /// <returns></returns>
        public bool IsSatisfied(List<WorldstateSymbol> worldstate) {
            return (TargetWorldState.Where(x => worldstate.Contains(x)).Count() == TargetWorldState.Count());
        }

        /// <summary>
        ///     get all used world state symbols.
        /// </summary>
        /// <returns></returns>
        public ISet<Type> GetAffectingWorldstateTypes() {
            HashSet<Type> types = new HashSet<Type>();

            foreach (WorldstateSymbol goapWorldstate in TargetWorldState) {
                types.Add(goapWorldstate.GetType());
            }
            return types;
        }

        /// <summary>
        ///     create an individual relevancy depending on actual
        ///     world state or let it static.
        /// </summary>
        /// <param name="actualWorldstate"></param>
        /// <returns></returns>
        public abstract int UpdateRelevancy(List<WorldstateSymbol> actualWorldstate);

        #endregion
    }

}
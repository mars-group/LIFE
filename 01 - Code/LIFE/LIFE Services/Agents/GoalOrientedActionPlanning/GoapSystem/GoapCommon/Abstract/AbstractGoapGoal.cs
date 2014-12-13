using System;
using System.Collections.Generic;
using System.Linq;
using GoapCommon.Implementation;

namespace GoapCommon.Abstract {

    /// <summary>
    ///     offers some ready done methods for creation of goap goals
    /// </summary>
    public abstract class AbstractGoapGoal {
        /// <summary>
        ///     get the world state symbols defining the reached goal.
        /// </summary>
        /// <returns></returns>
        public List<WorldstateSymbol> TargetWorldState { get { return _targetWorldState; } }


        private readonly List<WorldstateSymbol> _targetWorldState;
        protected int Relevancy;

        protected AbstractGoapGoal(List<WorldstateSymbol> targetWorldState, int startRelevancy) {
            _targetWorldState = targetWorldState;
            Relevancy = startRelevancy;
        }

        /// <summary>
        ///     get the weight of this goal.
        ///     is needed for dicision of goap manager when it chooses one of the goals.
        /// </summary>
        /// <returns></returns>
        public int GetRelevancy() {
            return Relevancy;
        }

        /// <summary>
        ///     check the target world state against the given world state.
        /// </summary>
        /// <param name="worldstate"></param>
        /// <returns></returns>
        public bool IsSatisfied(List<WorldstateSymbol> worldstate) {
            return (_targetWorldState.Where(x => worldstate.Contains(x)).Count() == _targetWorldState.Count());
        }

        /// <summary>
        ///     get all used world state symbols.
        /// </summary>
        /// <returns></returns>
        public ISet<Type> GetAffectingWorldstateTypes() {
            HashSet<Type> types = new HashSet<Type>();

            foreach (WorldstateSymbol goapWorldstate in _targetWorldState) {
                types.Add(goapWorldstate.GetType());
            }
            return types;
        }

        /// <summary>
        ///     create an individual relevancy depending on actual
        ///     world state, other values or let it static.
        /// </summary>
        /// <param name="actualWorldstate"></param>
        /// <returns></returns>
        public virtual void UpdateRelevancy(List<WorldstateSymbol> actualWorldstate){}
    }

}
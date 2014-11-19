using System;
using System.Collections.Generic;
using GoapCommon.Implementation;

namespace GoapCommon.Interfaces {

    /// <summary>
    ///     defines the target world state for planning process.
    ///     world state is usual a partial description of available symbols
    /// </summary>
    public interface IGoapGoal {
        /// <summary>
        ///     get the weight of this goal.
        ///     is needed for dicision of goap manager when it chooses one of the goals.
        /// </summary>
        /// <returns></returns>
        int GetRelevancy();

        /// <summary>
        ///     get the world state symbols defining the reached goal.
        /// </summary>
        /// <returns></returns>
        List<WorldstateSymbol> GetTargetWorldstates();

        /// <summary>
        ///     check the target world state against the given world state.
        /// </summary>
        /// <param name="worldstate"></param>
        /// <returns></returns>
        bool IsSatisfied(List<WorldstateSymbol> worldstate);

        /// <summary>
        ///     create an individual relevancy depending on actual
        ///     world state or let it static.
        /// </summary>
        /// <param name="actualWorldstate"></param>
        /// <returns></returns>
        int UpdateRelevancy(List<WorldstateSymbol> actualWorldstate);

        /// <summary>
        ///     get all used world state symbols.
        /// </summary>
        /// <returns></returns>
        ISet<Type> GetAffectingWorldstateTypes();
    }

}
using System.Collections.Generic;
using GoapCommon.Implementation;

namespace GoapCommon.Interfaces {

    /// <summary>
    ///     node in search graph of planning process
    /// </summary>
    public interface IGoapNode {
        /// <summary>
        ///     get a list of goal value symbols that are not satisfied by current value symbols
        /// </summary>
        /// <returns></returns>
        List<WorldstateSymbol> GetUnsatisfiedGoalValues();

        /// <summary>
        ///     check if the given worldstate would satisfy all the unsatisfied symbols
        /// </summary>
        /// <param name="startState"></param>
        /// <returns></returns>
        bool CanBeSatisfiedByStartState(List<WorldstateSymbol> startState);

        /// <summary>
        ///     get the heuristic value
        /// </summary>
        /// <returns></returns>
        int GetHeuristic();

        /// <summary>
        ///     check if there are unsatisfied symbols
        /// </summary>
        /// <returns></returns>
        bool HasUnsatisfiedProperties();
    }

}
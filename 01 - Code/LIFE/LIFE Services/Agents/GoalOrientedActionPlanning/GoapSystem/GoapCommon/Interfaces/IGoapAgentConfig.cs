using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;

namespace GoapCommon.Interfaces {

    /// <summary>
    ///     determines the configuration of an agent
    ///     by convertion name of implementing classes must be "AgentConfig" + unique number per assembly
    /// </summary>
    public interface IGoapAgentConfig {
        /// <summary>
        ///     start assignment of world
        /// </summary>
        /// <returns></returns>
        List<WorldstateSymbol> GetStartWorldstate();

        /// <summary>
        ///     all action that the agent can use
        /// </summary>
        /// <returns></returns>
        List<AbstractGoapAction> GetAllActions();

        /// <summary>
        ///     all the goals the agent can follow
        /// </summary>
        /// <returns></returns>
        List<IGoapGoal> GetAllGoals();

        /// <summary>
        ///     define the maximum of search depth in the planner
        /// </summary>
        /// <returns></returns>
        int GetMaxGraphSearchDepth();
    }

}
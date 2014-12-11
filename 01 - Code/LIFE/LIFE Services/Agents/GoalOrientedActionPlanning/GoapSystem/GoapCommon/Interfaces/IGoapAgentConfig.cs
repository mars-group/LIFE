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
        ///     all action the agent may use for planning
        /// </summary>
        /// <returns></returns>
        List<AbstractGoapAction> GetAllActions();

        /// <summary>
        ///     all the goals the agent may follow
        /// </summary>
        /// <returns></returns>
        List<IGoapGoal> GetAllGoals();

        /// <summary>
        ///     define the maximum of search depth in the planner
        /// </summary>
        /// <returns></returns>
        int GetMaxGraphSearchDepth();


        /// <summary>
        ///     Instruction if the goap manager has to use the UpdateWorldstate method before planning.
        /// </summary>
        /// <returns></returns>
        bool ForceSymbolsUpdateBeforePlanning();

        /// <summary>
        ///     Instruction for the goap manger has to use the UpdateWorldstate method every time GetNextAction is called. 
        /// </summary>
        /// <returns></returns>
        bool ForceSymbolsUpdateEveryActionRequest();
        
        /// <summary>
        ///     Connector between the concrete agent variables and the worldstate of the goap system.
        ///     Here the relation between agent variables and concrete symbols for the planning.
        /// </summary>
        /// <returns></returns>
        List<WorldstateSymbol> GetUpdatedSymbols();

        
    }

}
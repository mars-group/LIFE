using System.Collections.Generic;
using GenericAgentArchitectureCommon.Interfaces;
using GoapCommon.Implementation;

namespace GoapCommon.Interfaces {

    /// <summary>
    ///     An action represents a step towards a goal of one agent. It's a reusable unit.
    ///     Every agent gets his own set of available IActions.
    /// </summary>
    public interface IGoapAction : IInteraction {
        /// <summary>
        ///     calcutate the resulting list of world states by source world
        /// </summary>
        /// <param name="sourceWorldState"></param>
        /// <returns></returns>
        List<WorldstateSymbol> GetResultingWorldstate(List<WorldstateSymbol> sourceWorldState);

        /// <summary>
        ///     check if the preconditions are subset of source world state
        /// </summary>
        /// <param name="sourceWorldState"></param>
        /// <returns></returns>
        bool IsExecutable(List<WorldstateSymbol> sourceWorldState);

        /// <summary>
        ///     check the context preconditions on
        /// </summary>
        /// <returns></returns>
        bool ValidateContextPreconditions();

        /// <summary>
        ///     execute the context effects on the current worlds
        /// </summary>
        /// <returns></returns>
        bool ExecuteContextEffects();

        /// <summary>
        ///     calculate / get the costs for execution
        /// </summary>
        /// <returns></returns>
        int GetExecutionCosts();

        /// <summary>
        ///     get / calculate the priority
        /// </summary>
        /// <returns></returns>
        int GetPriority();
    }

}
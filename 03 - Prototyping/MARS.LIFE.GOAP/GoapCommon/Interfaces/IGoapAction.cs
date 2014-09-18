using System.Collections.Generic;
using CommonTypes.Interfaces;

namespace GoapCommon.Interfaces {
    /// <summary>
    ///     minimus requirements in formulating an action
    /// </summary>
    public interface IGoapAction : IAction {
        /// <summary>
        ///     calcutate the resulting list of world states by source world
        /// </summary>
        /// <param name="sourceWorldState"></param>
        /// <returns></returns>
        List<IGoapWorldstate> GetResultingWorldstate(List<IGoapWorldstate> sourceWorldState);

        /// <summary>
        ///     check if the preconditions are subset of source world state
        /// </summary>
        /// <param name="sourceWorldState"></param>
        /// <returns></returns>
        bool IsExecutable(List<IGoapWorldstate> sourceWorldState);

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
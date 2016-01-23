using System;

namespace CommonTypes.Interfaces
{
    /// <summary>
    /// An action represents a step towards a goal of one agent. It's a reusable unit.
    /// Every agent gets his own set of available IActions. 
    /// </summary>
    public interface IAction {
        
        /// <summary>
        /// change the worldstates by the effects and apply the context effects of this action
        /// </summary>
        /// <returns>true if successful</returns>
        Boolean Execute();

    }
}

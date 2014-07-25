using System.Collections.Generic;
using CommonTypes.Interfaces;

namespace GoapActionSystem.Interfaces
{
    /// <summary>
    /// The planner is responsible for the whole process of finding a valid plan from the actions, currentWorld and targetWorld
    /// given to him. The caller is responsible for giving well defined and corresponding actions and world states. 
    /// </summary>
    interface IPlanner {

        /// <summary>
        /// the Planner tries to create a new plan (sorted list of actions)
        /// </summary>
        /// <param name="availableActions"></param>
        /// <param name="currentWorld"></param>
        /// <param name="targetWorld"></param>
        /// <returns>A sorted list of actions representing the plan</returns>
        List<IAction> GetPlan(List<IAction> availableActions, List<IGoapWorldstate> currentWorld, List<IGoapWorldstate> targetWorld);
    }
}

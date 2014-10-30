using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoapCommon.Abstract;

namespace GoapCommon.Interfaces
{
    /// <summary>
    /// determines the configuration of an agent
    /// by convertion name of implementing classes must be "AgentConfig" + unique number per assembly
    /// </summary>
    public interface IGoapAgentConfig {

        /// <summary>
        /// start assignment of world
        /// </summary>
        /// <returns></returns>
        List<IGoapWorldProperty> GetStartWorldstate();

        /// <summary>
        /// all action that the agent can use
        /// </summary>
        /// <returns></returns>
        List<AbstractGoapAction> GetAllActions();

        /// <summary>
        /// all the goals the agent can follow
        /// </summary>
        /// <returns></returns>
        List<IGoapGoal> GetAllGoals();

    }
}

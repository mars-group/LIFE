using System;
using LifeAPI.Agent;

namespace AgentShadowingService.Interface
{
    public interface IAgentShadowingService
    {
        /// <summary>
        /// Create a ShadowAgent for a real agent with id <param name="agentId"/> and Type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The ShadowAgent proxy object of type T</returns>
        T CreateShadowAgent<T>(Guid agentId) where T : class;

        /// <summary>
        /// Registers a real agent with the AgentShadowing System.
        /// </summary>
        /// <param name="agentToRegister"></param>
        void RegisterRealAgent(IAgent agentToRegister);
    }
}

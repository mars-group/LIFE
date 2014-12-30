using System;
using ASC.Communication.ScsServices.Service;
using LifeAPI.Agent;

namespace AgentShadowingService.Interface
{
    public interface IAgentShadowingService<TServiceInterface, TServiceClass> 
        where TServiceClass : AscService, TServiceInterface
            where TServiceInterface : class, IAgent
    {
        /// <summary> 
        /// Create a ShadowAgent for a real agent with id <param name="agentId"/> and Type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The ShadowAgent proxy object of type T</returns>
        TServiceInterface CreateShadowAgent(Guid agentId);

        /// <summary>
        /// Registers a real agent with the AgentShadowing System.
        /// </summary>
        /// <param name="agentToRegister"></param>
        void RegisterRealAgent(TServiceClass agentToRegister);
    }
}

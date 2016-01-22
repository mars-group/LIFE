using System;
using System.Collections.Generic;
using ASC.Communication.ScsServices.Service;

namespace AgentShadowingService.Interface
{
    public interface IAgentShadowingService<TServiceInterface, TServiceClass> 
        where TServiceClass : AscService, TServiceInterface
            where TServiceInterface : class {

        event EventHandler<LIFEAgentEventArgs<TServiceInterface>> AgentUpdates;

        /// <summary> 
        /// Resolves an agent by its agentId.
		/// Will create a ShadowAgent for a real agent with id <param name="agentId"/> and Type T,
		/// if the agent instance is not found to be running locally. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The reference to the local agent if found, the ShadowAgent proxy object of type T otherwise</returns>
        TServiceInterface ResolveAgent(Guid agentId);

        /// <summary>
		/// Resolves agents by their agentId.
		/// Will create one ShadowAgent for each real agent with id <param name="agentId"/> and Type T,
		/// if the agent instance is not found to be running locally. 
        /// Uses parallelism, so use this method, whenever you have more than a few ShadowAgents to create.
        /// e.g.: at first Initialization!
        /// </summary>
        /// <param name="agentIds"></param>
        /// <returns>List of TSerivceInterface</returns>
        List<TServiceInterface> ResolveAgents(Guid[] agentIds); 

        /// <summary>
        /// Removes the Shadow Agent with id agentId from the Service.
        /// Calls disconnect() prior to removal.
        /// </summary>
        /// <param name="agentId"></param>
        void RemoveShadowAgent(Guid agentId);

        /// <summary>
        /// Registers a real agent with the AgentShadowing System.
        /// </summary>
        /// <param name="agentToRegister"></param>
        void RegisterRealAgent(TServiceClass agentToRegister);

        /// <summary>
        /// Registers all agents from the provided array.
        /// </summary>
        /// <param name="agentsToRegister">all agents to register</param>
        void RegisterRealAgents(TServiceClass[] agentsToRegister);

        /// <summary>
        /// Removes the real agent agentToRemove from the AgentShadowing Service
        /// </summary>
        /// <param name="agentToRemove"></param>
        void RemoveRealAgent(TServiceClass agentToRemove);

        /// <summary>
        /// Returns the name of this LayerContainer
        /// </summary>
        /// <returns></returns>
        string GetLayerContainerName();
    }
}

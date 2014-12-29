using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AgentShadowingService.Interface;
using ASC.Communication.ScsServices.Client;
using ASC.Communication.ScsServices.Service;
using LifeAPI.Agent;
using LIFEUtilities.MulticastAddressGenerator;

namespace AgentShadowingService.Implementation
{
    internal class AgentShadowingServiceUseCase<T> : IAgentShadowingService<T> where T : class, IAgent
    {
        private IDictionary<IScsServiceClient<T>, byte> shadowAgentClients;
        private readonly string _mcastAddress;
        private readonly IScsServiceApplication _agentShadowingServer;
        private readonly MethodInfo _genericAddServiceMethod;

        public AgentShadowingServiceUseCase()
        {
            shadowAgentClients = new ConcurrentDictionary<IScsServiceClient<T>, byte>();
            _mcastAddress = "udp://" + MulticastAddressGenerator.GetIPv4MulticastAddressByType(typeof (T)) + ":6666";
            _agentShadowingServer = ScsServiceBuilder.CreateService(_mcastAddress);
            _agentShadowingServer.Start();

            // reflect type of Interface of T
            Type interfaceType = null;
            foreach (var @interface in from @interface in typeof(T).GetInterfaces() from customAttributeData in @interface.CustomAttributes where customAttributeData.AttributeType == typeof(ScsServiceAttribute) select @interface)
            {
                interfaceType = @interface;
            }
            var addServiceMethod = _agentShadowingServer.GetType().GetMethod("AddService");
            _genericAddServiceMethod = addServiceMethod.MakeGenericMethod(interfaceType, typeof(T));
        } 

        public T CreateShadowAgent(Guid agentId)
        {
            var shadowAgentClient = ScsServiceClientBuilder.CreateClient<T>(
                _mcastAddress,
                agentId
                );
            shadowAgentClients.Add(shadowAgentClient, new byte());
            return shadowAgentClient.ServiceProxy;
        }

        public void RegisterRealAgent(T agentToRegister)
        {
            _genericAddServiceMethod.Invoke(_agentShadowingServer, new object[]{agentToRegister});
        }
    }
}

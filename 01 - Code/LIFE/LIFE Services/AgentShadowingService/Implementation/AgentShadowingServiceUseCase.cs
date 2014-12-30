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
    internal class AgentShadowingServiceUseCase<TServiceInterface, TServiceClass> : IAgentShadowingService<TServiceInterface, TServiceClass>
        where TServiceClass : AscService, TServiceInterface
        where TServiceInterface : class, IAgent
    {
        private IDictionary<IScsServiceClient<TServiceClass>, byte> shadowAgentClients;
        private readonly string _mcastAddress;
        private readonly IScsServiceApplication _agentShadowingServer;
        //private readonly MethodInfo _genericAddServiceMethod;
        //private Type _interfaceType;

        public AgentShadowingServiceUseCase() {
            var typeOfTServiceClass = typeof (TServiceClass);
            shadowAgentClients = new ConcurrentDictionary<IScsServiceClient<TServiceClass>, byte>();
            _mcastAddress = "udp://" + MulticastAddressGenerator.GetIPv4MulticastAddressByType(typeOfTServiceClass) + ":6666";
            _agentShadowingServer = ScsServiceBuilder.CreateService(_mcastAddress);
            _agentShadowingServer.Start();

            // reflect type of Interface of T which has AscService Attribute
            /* 
            _interfaceType = null;
            foreach (var @interface in from @interface in typeOfT.GetInterfaces() from customAttributeData in @interface.CustomAttributes where customAttributeData.AttributeType == typeof(AscServiceAttribute) select @interface)
            {
                _interfaceType = @interface;
            }


            var addServiceMethod = _agentShadowingServer.GetType().GetMethod("AddService");
            _genericAddServiceMethod = addServiceMethod.MakeGenericMethod(_interfaceType, typeOfT);
            */
        }

        public TServiceInterface CreateShadowAgent(Guid agentId)
        {
            var shadowAgentClient = ScsServiceClientBuilder.CreateClient<TServiceClass>(
                _mcastAddress,
                agentId
                );
            shadowAgentClients.Add(shadowAgentClient, new byte());
            return shadowAgentClient.ServiceProxy;
        }

        public void RegisterRealAgent(TServiceClass agentToRegister)
        {
            _agentShadowingServer.AddService<TServiceInterface, TServiceClass>(agentToRegister);
        }
    }
}

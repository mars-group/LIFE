using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using AppSettingsManager.Implementation;
using AppSettingsManager.Interface;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using ConfigurationAdapter.Implementation;
using ConfigurationAdapter.Interface;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface;
using NodeRegistry.Implementation.Messages;
using NodeRegistry.Interface;
using ProtoBuf;

namespace NodeRegistry.Implementation
{
    public class NodeRegistryManager : INodeRegistry
    {
        //TODO clean
        #region Fields and Properties
        private Dictionary<String, NodeInformationType> activeNodeList;
        private readonly NodeInformationType localNodeInformation;
        private IMulticastReciever reciever;
        private IMulticastClientAdapter clientAdapter;
        // private readonly IConfigurationAdapter configurationAdapter;
        private Configuration<NodeRegistryConfig> config;
        private Thread listenThread;
        #endregion

        #region delegates

        public delegate void SubscribeForNodeTypeDelegate(NodeType nodeType);
        #endregion

        #region Constructors
        public NodeRegistryManager(NodeInformationType nodeInformation)
            : this()
        {
            this.localNodeInformation = nodeInformation;
        }

        public NodeRegistryManager()
        {
            var path = "./" + typeof(NodeRegistryManager).Name + ".config";

            this.config = new Configuration<NodeRegistryConfig>(path);

            activeNodeList = new Dictionary<string, NodeInformationType>();
            localNodeInformation = ParseNodeInformationTypeFromConfig();
            
            SetupNetworkAdapters();
            //TODO start dicovery 

        }
        #endregion

        #region private Methods
        private void SetupNetworkAdapters()
        {
            reciever = new UDPMulticastReceiver();
            clientAdapter = new UDPMulticastSender();
            listenThread = new Thread(Listen);

            listenThread.Start();
        }

        private NodeInformationType ParseNodeInformationTypeFromConfig()
        {
            return new NodeInformationType(
                ParseNodeTypeFromConfig(),
                config.Content.NodeIdentifier,
                ParseNodeEndpointFromConfig()
                );
        }

        private NodeEndpoint ParseNodeEndpointFromConfig()
        {
            return new NodeEndpoint(config.Content.NodeEndPointIP, config.Content.NodeEndPointPort);
        }

        private NodeType ParseNodeTypeFromConfig()
        {
            return config.Content.NodeType;
        }

     
        private void AnswerMessage(NodeRegistryMessage nodeRegestryMessage)
        {
            switch (nodeRegestryMessage.messageType)
            {
                case NodeRegistryMessageType.Answer:
                    if (!activeNodeList.ContainsKey(nodeRegestryMessage.nodeInformationType.NodeIdentifier))
                    {
                        activeNodeList.Add(nodeRegestryMessage.nodeInformationType.NodeIdentifier,
                            nodeRegestryMessage.nodeInformationType);
                    }
                    break;
                case NodeRegistryMessageType.Join:
                    //TODO dont add this node
                    clientAdapter.SendMessageToMulticastGroup(
                        NodeRegistryMessageFactory.GetAnswerMessage(localNodeInformation));
                    break;
                case NodeRegistryMessageType.Leave:
                    activeNodeList.Remove(nodeRegestryMessage.nodeInformationType.NodeIdentifier);
                    break;
                default:
                    break;
            }
        }

        private void JoinCluster()
        {
            clientAdapter.SendMessageToMulticastGroup(NodeRegistryMessageFactory.GetJoinMessage(localNodeInformation));
        }

        #endregion

        #region public Methods
        public void DropAllNodes()
        {
            activeNodeList = new Dictionary<string, NodeInformationType>();
        }

        public void Listen()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                byte[] msg = reciever.readMulticastGroupMessage();
                var stream = new MemoryStream(msg);
                var nodeRegestryMessage = Serializer.Deserialize<NodeRegistryMessage>(stream);
                AnswerMessage(nodeRegestryMessage);
            }
        }




        public void LeaveCluster()
        {
            clientAdapter.SendMessageToMulticastGroup(NodeRegistryMessageFactory.GetLeaveMessage(localNodeInformation));
        }



        public void ShutDownNodeRegistry()
        {
            LeaveCluster();
            reciever.CloseSocket();
            clientAdapter.CloseSocket();
            listenThread.Interrupt();
        }

        public void StartDiscovery()
        {
            JoinCluster();
        }

        public void RestartDiscovery()
        {
            DropAllNodes();
            LeaveCluster();
            JoinCluster();
        }


        //TODO testen  
        public void SubscribeForContainerType(SubscribeForNodeTypeDelegate subscribeForNodeTypeDelegate)
        {
            subscribeForNodeTypeDelegate.DynamicInvoke();
        }

        public List<NodeInformationType> GetAllNodes()
        {
            return activeNodeList.Values.Select(type => type).ToList();
        }

        public List<NodeInformationType> GetAllNodesByType(NodeType nodeType)
        {
            return GetAllNodes().Where(nodeInformationType => nodeInformationType.NodeType == nodeType).ToList();
        }
        #endregion
    }
}

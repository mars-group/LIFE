using AppSettingsManager.Implementation;
using AppSettingsManager.Interface;
using CommonTypes.DataTypes;
using CommonTypes.Types;

using NodeRegistry.Implementation.Messages;
using NodeRegistry.Interface;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;


namespace NodeRegistry.Implementation
{
    using MulticastAdapter.Implementation;
    using MulticastAdapter.Interface;

    public class NodeRegistryManager : INodeRegistry
    {
        private Dictionary<String, NodeInformationType> activeNodeList;
        private NodeInformationType localNodeInformation;
        private IMulticastReciever reciever;
        private IMulticastClientAdapter clientAdapter;
        private IConfigurationAdapter configurationAdapter; 

        private Thread listenThread;



        public NodeRegistryManager(NodeInformationType nodeInformation)
        {

            this.activeNodeList = new Dictionary<string, NodeInformationType>();
            this.localNodeInformation = nodeInformation;
            this.configurationAdapter = new NiniAdapterImpl("NodeRegistry");

           
            SetupNetworkAdapters();

        }

        public NodeRegistryManager()
        {
            this.configurationAdapter = new AppSettingAdapterImpl();
            this.activeNodeList = new Dictionary<string, NodeInformationType>();
            this.localNodeInformation = ParseNodeInformationTypeFromConfig();
          
            SetupNetworkAdapters();

        }

        private void SetupNetworkAdapters()
        {
            this.reciever = new UDPMulticastReceiver();
            this.clientAdapter = new UDPMulticastSender();
            this.listenThread = new Thread(new ThreadStart(this.Listen));

            listenThread.Start();
        }

        private NodeInformationType ParseNodeInformationTypeFromConfig()
        {
            return new NodeInformationType(
                ParseNodeTypeFromConfig(),
                configurationAdapter.GetValue("NodeIdentifier"),
                ParseNodeEndpointFromConfig()
                );
        }

        private NodeEndpoint ParseNodeEndpointFromConfig()
        {
            return new NodeEndpoint(
                configurationAdapter.GetValue("NodeEndPointIP"),
               configurationAdapter.GetInt32("NodeEndPointPort")
                );
        }

        private NodeType ParseNodeTypeFromConfig()
        {

            var argument = configurationAdapter.GetInt32("NodeType");
            return (NodeType) argument;

        }

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

        private void AnswerMessage(NodeRegistryMessage nodeRegestryMessage)
        {


            switch (nodeRegestryMessage.messageType)
            {
                case NodeRegistryMessageType.Answer:
                    if (!activeNodeList.ContainsKey(nodeRegestryMessage.nodeInformationType.NodeIdentifier))
                    {
                        activeNodeList.Add(nodeRegestryMessage.nodeInformationType.NodeIdentifier, nodeRegestryMessage.nodeInformationType);
                    }
                    break;
                case NodeRegistryMessageType.Join:
                    clientAdapter.SendMessageToMulticastGroup(NodeRegistryMessageFactory.GetAnswerMessage(localNodeInformation));
                    break;
                case NodeRegistryMessageType.Leave:
                    activeNodeList.Remove(nodeRegestryMessage.nodeInformationType.NodeIdentifier);
                    break;
                default:
                    break;

            }
        }

        public void LeaveCluster()
        {

            clientAdapter.SendMessageToMulticastGroup(NodeRegistryMessageFactory.GetLeaveMessage(localNodeInformation));


        }

        private void JoinCluster()
        {

            clientAdapter.SendMessageToMulticastGroup(NodeRegistryMessageFactory.GetJoinMessage(localNodeInformation));

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

        public List<NodeInformationType> GetAllNodes()
        {
            return activeNodeList.Values.Select(type => type).ToList();
        }

        public List<NodeInformationType> GetAllNodesByType(NodeType nodeType)
        {
            return GetAllNodes().Where(nodeInformationType => nodeInformationType.NodeType == nodeType).ToList();
        }
    }

}

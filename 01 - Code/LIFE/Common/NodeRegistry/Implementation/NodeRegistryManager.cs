using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using AppSettingsManager.Implementation;
using AppSettingsManager.Interface;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface;
using NodeRegistry.Implementation.Messages;
using NodeRegistry.Interface;
using ProtoBuf;

namespace NodeRegistry.Implementation {
    public class NodeRegistryManager : INodeRegistry {
        private Dictionary<String, NodeInformationType> activeNodeList;
        private readonly NodeInformationType localNodeInformation;
        private IMulticastReciever reciever;
        private IMulticastClientAdapter clientAdapter;
<<<<<<< HEAD
        private IConfigurationAdapter configurationAdapter;
=======
        private readonly IConfigurationAdapter configurationAdapter;
>>>>>>> 0897e73b2cf45bfadde3f1cf6df700ba2e4dafc1

        private Thread listenThread;


        public NodeRegistryManager(NodeInformationType nodeInformation) {
            activeNodeList = new Dictionary<string, NodeInformationType>();
            localNodeInformation = nodeInformation;
            configurationAdapter = new AppSettingAdapterImpl();

<<<<<<< HEAD
        public NodeRegistryManager(NodeInformationType nodeInformation)
        {
            this.activeNodeList = new Dictionary<string, NodeInformationType>();
            this.localNodeInformation = nodeInformation;
            this.configurationAdapter = new AppSettingAdapterImpl();
=======
>>>>>>> 0897e73b2cf45bfadde3f1cf6df700ba2e4dafc1

            SetupNetworkAdapters();
        }

<<<<<<< HEAD
        public NodeRegistryManager()
        {
            this.configurationAdapter = new AppSettingAdapterImpl();
            this.activeNodeList = new Dictionary<string, NodeInformationType>();
            this.localNodeInformation = ParseNodeInformationTypeFromConfig();
=======
        public NodeRegistryManager() {
            configurationAdapter = new AppSettingAdapterImpl();
            activeNodeList = new Dictionary<string, NodeInformationType>();
            localNodeInformation = ParseNodeInformationTypeFromConfig();
>>>>>>> 0897e73b2cf45bfadde3f1cf6df700ba2e4dafc1

            SetupNetworkAdapters();
        }

        private void SetupNetworkAdapters() {
            reciever = new UDPMulticastReceiver();
            clientAdapter = new UDPMulticastSender();
            listenThread = new Thread(Listen);

            listenThread.Start();
        }

        private NodeInformationType ParseNodeInformationTypeFromConfig() {
            return new NodeInformationType(
                ParseNodeTypeFromConfig(),
                configurationAdapter.GetValue("NodeIdentifier"),
                ParseNodeEndpointFromConfig()
                );
        }

        private NodeEndpoint ParseNodeEndpointFromConfig() {
            return new NodeEndpoint(
                configurationAdapter.GetValue("NodeEndPointIP"),
                configurationAdapter.GetInt32("NodeEndPointPort")
                );
        }

        private NodeType ParseNodeTypeFromConfig() {
            var argument = configurationAdapter.GetInt32("NodeType");
<<<<<<< HEAD
            return (NodeType)argument;

=======
            return (NodeType) argument;
>>>>>>> 0897e73b2cf45bfadde3f1cf6df700ba2e4dafc1
        }

        public void DropAllNodes() {
            activeNodeList = new Dictionary<string, NodeInformationType>();
        }

        public void Listen() {
            while (Thread.CurrentThread.IsAlive) {
                byte[] msg = reciever.readMulticastGroupMessage();
                var stream = new MemoryStream(msg);
                var nodeRegestryMessage = Serializer.Deserialize<NodeRegistryMessage>(stream);
                AnswerMessage(nodeRegestryMessage);
            }
        }

        private void AnswerMessage(NodeRegistryMessage nodeRegestryMessage) {
            switch (nodeRegestryMessage.messageType) {
                case NodeRegistryMessageType.Answer:
                    if (!activeNodeList.ContainsKey(nodeRegestryMessage.nodeInformationType.NodeIdentifier)) {
                        activeNodeList.Add(nodeRegestryMessage.nodeInformationType.NodeIdentifier,
                            nodeRegestryMessage.nodeInformationType);
                    }
                    break;
                case NodeRegistryMessageType.Join:
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

        public void LeaveCluster() {
            clientAdapter.SendMessageToMulticastGroup(NodeRegistryMessageFactory.GetLeaveMessage(localNodeInformation));
        }

        private void JoinCluster() {
            clientAdapter.SendMessageToMulticastGroup(NodeRegistryMessageFactory.GetJoinMessage(localNodeInformation));
        }

        public void ShutDownNodeRegistry() {
            LeaveCluster();
            reciever.CloseSocket();
            clientAdapter.CloseSocket();
            listenThread.Interrupt();
        }

        public void StartDiscovery() {
            JoinCluster();
        }

        public void RestartDiscovery() {
            DropAllNodes();
            LeaveCluster();
            JoinCluster();
        }

        public List<NodeInformationType> GetAllNodes() {
            return activeNodeList.Values.Select(type => type).ToList();
        }

        public List<NodeInformationType> GetAllNodesByType(NodeType nodeType) {
            return GetAllNodes().Where(nodeInformationType => nodeInformationType.NodeType == nodeType).ToList();
        }
    }
}

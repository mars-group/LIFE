using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface;
using Newtonsoft.Json;
using NodeRegistry.Implementation.Messages;
using NodeRegistry.Interface;
using ProtoBuf;


namespace NodeRegistry.Implementation
{
    public class NodeRegistryManager : INodeRegistry
    {
        private Dictionary<String, NodeInformationType> activeNodeList;
        private IMulticastReciever reciever;
        private IMulticastClientAdapter clientAdapter;
        private NodeInformationType nodeInformation;
        private Thread listenThread;


        public NodeRegistryManager(NodeInformationType nodeInformation)
        {
            
            SetupNetworkAdapters();

            this.activeNodeList = new Dictionary<string, NodeInformationType>();

            this.nodeInformation = nodeInformation;

            this.listenThread = new Thread(new ThreadStart(this.Listen));

            listenThread.Start();

        }

        public NodeRegistryManager(string nodeName, NodeType nodeType)
        {
            SetupNetworkAdapters();

            this.activeNodeList = new Dictionary<string, NodeInformationType>();

            this.nodeInformation = new NodeInformationType(nodeType, nodeName, ParseNodeEndpointFromConfig());

            this.listenThread = new Thread(new ThreadStart(this.Listen));

            listenThread.Start();
        }


        public NodeRegistryManager(string nodeName)
        {
            SetupNetworkAdapters();

            this.activeNodeList = new Dictionary<string, NodeInformationType>();
           
            this.nodeInformation = new NodeInformationType(ParseNodeTypeFromConfig(), nodeName, ParseNodeEndpointFromConfig());

            this.listenThread = new Thread(new ThreadStart(this.Listen));

            listenThread.Start();
        }
        

        public NodeRegistryManager()
        {
            SetupNetworkAdapters();

            this.activeNodeList = new Dictionary<string, NodeInformationType>();
           
            this.nodeInformation = ParseNodeInformationTypeFromConfig();

            this.listenThread = new Thread(new ThreadStart(this.Listen));

            listenThread.Start();
        }

        private void SetupNetworkAdapters()
        {
            this.reciever = new UDPMulticastReceiver();
            this.clientAdapter = new UDPMulticastSender();
        }

        private NodeEndpoint ParseNodeEndpointFromConfig()
        {
            return new NodeEndpoint(
                ConfigurationManager.AppSettings.Get("NodeEndPointIP"),
                Int32.Parse(ConfigurationManager.AppSettings.Get("NodeEndPointPort"))
                );
        }



        private NodeInformationType ParseNodeInformationTypeFromConfig()
        {
            ConfigurationManager.AppSettings.Get("");

            return new NodeInformationType(
                ParseNodeTypeFromConfig(),
                ConfigurationManager.AppSettings.Get("NodeIdentifier"),
                ParseNodeEndpointFromConfig()
                );
        }

        private NodeType ParseNodeTypeFromConfig()
        {
            var configEntry = ConfigurationManager.AppSettings.Get("NodeType");

            switch (Int32.Parse(configEntry))
            {
                case 0:
                    return NodeType.LayerContainer; 
                case 1:
                    return NodeType.SimulationManager;
                case 2: 
                    return NodeType.SimulationController;
            }
            throw new ArgumentException("Illigale Argument for NodeType. Valid Parameter are 0 for LayerContainer, 1 for SimulationManager , 2 for SimulationController");

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
                    clientAdapter.SendMessageToMulticastGroup(NodeRegistryMessageFactory.GetAnswerMessage(nodeInformation));
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
            clientAdapter.SendMessageToMulticastGroup(NodeRegistryMessageFactory.GetLeaveMessage(nodeInformation));
            clientAdapter.CloseSocket();
            reciever.CloseSocket();
            listenThread.Interrupt();
        }

        public void JoinCluster()
        {
            clientAdapter.SendMessageToMulticastGroup(NodeRegistryMessageFactory.GetJoinMessage(nodeInformation));
        }


        public void startDiscovery()
        {
            JoinCluster();
        }

        public void restartDiscovery()
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

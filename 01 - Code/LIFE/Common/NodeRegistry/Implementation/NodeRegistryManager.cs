using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CommonTypes.DataTypes;
using CommonTypes.Types;
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
        //TODO clean & comment
        #region Fields and Properties
        private Dictionary<String, NodeInformationType> _activeNodeList;
        private readonly NodeInformationType _localNodeInformation;
        private IMulticastAdapter _multicastAdapter;
        private readonly Configuration<NodeRegistryConfig> _config;
        private Thread _listenThread;
        private NewNodeConnected _newNodeConnectedHandler;
        private NewNodeConnected _newNodeTypeConnectedHandler;

        #endregion


        //TODO dependency inject MulticastAdapter
        #region Constructors
        public NodeRegistryManager(NodeInformationType nodeInformation, IMulticastAdapter multicastAdapter)
        {
            this._localNodeInformation = nodeInformation;
            var path = "./" + typeof(NodeRegistryManager).Name + ".config";

            this._config = new Configuration<NodeRegistryConfig>(path);

            _activeNodeList = new Dictionary<string, NodeInformationType>();

            SetupNetworkAdapters(multicastAdapter);
            JoinCluster();
        }

        public NodeRegistryManager(IMulticastAdapter multicastAdapter)
        {
            var path = "./" + typeof(NodeRegistryManager).Name + ".config";

            this._config = new Configuration<NodeRegistryConfig>(path);

            _activeNodeList = new Dictionary<string, NodeInformationType>();
            _localNodeInformation = ParseNodeInformationTypeFromConfig();

            SetupNetworkAdapters(multicastAdapter);
            JoinCluster();
        }
        #endregion

        #region public Methods

        public void JoinCluster()
        {
            _multicastAdapter.SendMessageToMulticastGroup(NodeRegistryMessageFactory.GetJoinMessage(_localNodeInformation));
        }

        public void LeaveCluster()
        {
            _multicastAdapter.SendMessageToMulticastGroup(NodeRegistryMessageFactory.GetLeaveMessage(_localNodeInformation));
        }

        public void ShutDownNodeRegistry()
        {
            LeaveCluster();
            _listenThread.Interrupt();
            _multicastAdapter.CloseSocket();

        }

        public Configuration<NodeRegistryConfig> GetConfig()
        {
            return _config;
        }

        public List<NodeInformationType> GetAllNodes(bool includeMySelf = false)
        {
            if (includeMySelf)
            {
                return _activeNodeList.Values
                    .Select(type => type)
                    .Where(type => type.Equals(ParseNodeInformationTypeFromConfig()))
                    .ToList();
            }
            return _activeNodeList.Values.Select(type => type).ToList();

        }

        public List<NodeInformationType> GetAllNodesByType(NodeType nodeType)
        {
            return GetAllNodes().Where(nodeInformationType => nodeInformationType.NodeType == nodeType).ToList();
        }

        public void SubscribeForNewNodeConnected(NewNodeConnected newNodeConnectedHandler)
        {

            _newNodeConnectedHandler += newNodeConnectedHandler;

        }

        public void SubscribeForNewNodeConnectedByType(NewNodeConnected newNodeConnectedHandler, NodeType nodeType)
        {
            _newNodeTypeConnectedHandler += newNodeConnectedHandler;
        }

        #endregion

        #region private Methods
        private void SetupNetworkAdapters(IMulticastAdapter multicastAdapter)
        {

            _multicastAdapter = multicastAdapter;
            _listenThread = new Thread(Listen);
            _listenThread.Start();

        }

        private NodeInformationType ParseNodeInformationTypeFromConfig()
        {
            return new NodeInformationType(
                ParseNodeTypeFromConfig(),
                _config.Content.NodeIdentifier,
                ParseNodeEndpointFromConfig()
                );
        }

        private NodeEndpoint ParseNodeEndpointFromConfig()
        {
            return new NodeEndpoint(_config.Content.NodeEndPointIP, _config.Content.NodeEndPointPort);
        }

        private NodeType ParseNodeTypeFromConfig()
        {
            return _config.Content.NodeType;
        }

        private void AnswerMessage(NodeRegistryMessage nodeRegistryMessage)
        {
            switch (nodeRegistryMessage.messageType)
            {
                case NodeRegistryMessageType.Answer:
                    if (!_activeNodeList.ContainsKey(nodeRegistryMessage.nodeInformationType.NodeIdentifier))
                    {
                        _activeNodeList.Add(nodeRegistryMessage.nodeInformationType.NodeIdentifier,
                            nodeRegistryMessage.nodeInformationType);
                    }
                    break;
                case NodeRegistryMessageType.Join:
                    OnJoinMessage(nodeRegistryMessage);
                    break;
                case NodeRegistryMessageType.Leave:
                    _activeNodeList.Remove(nodeRegistryMessage.nodeInformationType.NodeIdentifier);
                    break;
                default:
                    break;
            }
        }

        private void OnJoinMessage(NodeRegistryMessage nodeRegistryMessage)
        {

            // chekcs if the new node is the local node 
            if (nodeRegistryMessage.nodeInformationType.NodeIdentifier.Equals(
                _localNodeInformation.NodeIdentifier))
            {
                //check configured behavouir if true add local node information to list
                if (_config.Content.AddMySelfToActiveNodeList)
                {
                    _multicastAdapter.SendMessageToMulticastGroup(
                        NodeRegistryMessageFactory.GetAnswerMessage(_localNodeInformation));
                }
            }
            //other node has joined the cluster
            else
            {
                //add new node to list
                _activeNodeList.Add(nodeRegistryMessage.nodeInformationType.NodeIdentifier,
                    nodeRegistryMessage.nodeInformationType);

                //notify all subsribers
                NotifyOnNodeJoinSubsribers(nodeRegistryMessage.nodeInformationType);
                NotifyOnNodeTypeJoinSubsribers(nodeRegistryMessage.nodeInformationType);

                // send my information to the new node
                _multicastAdapter.SendMessageToMulticastGroup(
                    NodeRegistryMessageFactory.GetAnswerMessage(_localNodeInformation));
            }


        }

        private void NotifyOnNodeJoinSubsribers(NodeInformationType nodeInformation)
        {
            if (_newNodeConnectedHandler != null)
            {
                _newNodeConnectedHandler.Invoke(nodeInformation);
            }

        }

        private void NotifyOnNodeTypeJoinSubsribers(NodeInformationType nodeInformationType)
        {
            if (_newNodeTypeConnectedHandler != null)
            {
                _newNodeTypeConnectedHandler.Invoke(nodeInformationType);
            }

        }


        private void DropAllNodes()
        {
            _activeNodeList = new Dictionary<string, NodeInformationType>();
        }

        private void Listen()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                byte[] msg = _multicastAdapter.readMulticastGroupMessage();
                var stream = new MemoryStream(msg);
                var nodeRegistryMessage = Serializer.Deserialize<NodeRegistryMessage>(stream);
                AnswerMessage(nodeRegistryMessage);
            }
        }


        #endregion

    }
}

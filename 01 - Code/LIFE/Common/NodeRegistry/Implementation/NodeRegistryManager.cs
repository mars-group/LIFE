using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using AppSettingsManager.Interface;
using CommonTypes.DataTypes;
using CommonTypes.Types;
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
        private Dictionary<String, NodeInformationType> _activeNodeList;
        private readonly NodeInformationType _localNodeInformation;
        private IMulticastReciever _reciever;
        private IMulticastClientAdapter _clientAdapter;
        private readonly Configuration<NodeRegistryConfig> _config;
        private Thread _listenThread;
        private NewNodeConnected _newNodeConnectedHandler;
        private NewNodeConnected _newNodeTypeConnectedHandler;
        #endregion

        #region Constructors
        public NodeRegistryManager(NodeInformationType nodeInformation)
        {
            this._localNodeInformation = nodeInformation;
            var path = "./" + typeof(NodeRegistryManager).Name + ".config";

            this._config = new Configuration<NodeRegistryConfig>(path);

            _activeNodeList = new Dictionary<string, NodeInformationType>();

            SetupNetworkAdapters();
            JoinCluster();
        }

        public NodeRegistryManager()
        {
            var path = "./" + typeof(NodeRegistryManager).Name + ".config";

            this._config = new Configuration<NodeRegistryConfig>(path);

            _activeNodeList = new Dictionary<string, NodeInformationType>();
            _localNodeInformation = ParseNodeInformationTypeFromConfig();

            SetupNetworkAdapters();
            JoinCluster();
        }
        #endregion

       #region public Methods

        public void JoinCluster()
        {
            _clientAdapter.SendMessageToMulticastGroup(NodeRegistryMessageFactory.GetJoinMessage(_localNodeInformation));
        }

        public void LeaveCluster()
        {
            _clientAdapter.SendMessageToMulticastGroup(NodeRegistryMessageFactory.GetLeaveMessage(_localNodeInformation));
        }

        public void ShutDownNodeRegistry()
        {
            LeaveCluster();
            _reciever.CloseSocket();
            _clientAdapter.CloseSocket();
            _listenThread.Interrupt();
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

        public void SubscribeForNewNodeConnected(NewNodeConnected newNodeConnectedHandler) {
            
            _newNodeConnectedHandler += newNodeConnectedHandler;
            
        }

        public void SubscribeForNewNodeConnectedByType(NewNodeConnected newNodeConnectedHandler, NodeType nodeType) {
            _newNodeTypeConnectedHandler += newNodeConnectedHandler;
        }

        #endregion

        #region private Methods
        private void SetupNetworkAdapters()
        {
            _reciever = new UDPMulticastReceiver();
            _clientAdapter = new UDPMulticastSender();
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
                    _clientAdapter.SendMessageToMulticastGroup(
                        NodeRegistryMessageFactory.GetAnswerMessage(_localNodeInformation));
                    NotifyOnNodeJoinSubsribers(nodeRegistryMessage.nodeInformationType);
                    NotifyOnNodeTypeJoinSubsribers(nodeRegistryMessage.nodeInformationType);
                    break;
                case NodeRegistryMessageType.Leave:
                    _activeNodeList.Remove(nodeRegistryMessage.nodeInformationType.NodeIdentifier);
                    break;
                default:
                    break;
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
                byte[] msg = _reciever.readMulticastGroupMessage();
                var stream = new MemoryStream(msg);
                var nodeRegistryMessage = Serializer.Deserialize<NodeRegistryMessage>(stream);
                AnswerMessage(nodeRegistryMessage);
            }
        }


        #endregion

    }
}

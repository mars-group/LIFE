using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using ConfigurationAdapter.Interface;
using MulticastAdapter.Interface;
using NodeRegistry.Implementation.Messages;
using NodeRegistry.Interface;
using ProtoBuf;


namespace NodeRegistry.Implementation
{
    public class NodeRegistryUseCase : INodeRegistry
    {
        #region Fields and Properties
        /// <summary>
        /// A dictonary of all active Node in the cluster. The Key is the NodeIdentifier from the NodeInformationobject(Value) and the Value is the NodeInformation object
        /// </summary>
        private Dictionary<String, NodeInformationType> _activeNodeList;
        /// <summary>
        ///Object that holds all relevant information about the NodeRegistry on this calculation unit.
        /// </summary>
        private readonly NodeInformationType _localNodeInformation;

        /// <summary>
        /// Adapter that handels the network comunication.
        /// </summary>
        private IMulticastAdapter _multicastAdapter;
        /// <summary>
        /// Configuration Object that changes the behaviour of this node Registry.
        /// </summary>
        private readonly Configuration<NodeRegistryConfig> _config;

        /// <summary>
        /// Thread that waits for multicast messages.
        /// </summary>
        private Thread _listenThread;

        /// <summary>
        /// delegates for different subscriber types.
        /// </summary>
        private NewNodeConnected _newNodeConnectedHandler;
        private NewNodeConnected _newLayerContainerConnectedHandler;
        private NewNodeConnected _newSimulationManagerConnectedHandler;
        private NewNodeConnected _newSimulationControllerConnectedHandler;

        #endregion


        #region Constructors
        public NodeRegistryUseCase(NodeInformationType nodeInformation, IMulticastAdapter multicastAdapter)
        {
            this._localNodeInformation = nodeInformation;
          

            this._config = new Configuration<NodeRegistryConfig>();

            _activeNodeList = new Dictionary<string, NodeInformationType>();

            SetupNetworkAdapters(multicastAdapter);
            JoinCluster();
        }

        public NodeRegistryUseCase(IMulticastAdapter multicastAdapter)
        {
      
            this._config = new Configuration<NodeRegistryConfig>();

            _activeNodeList = new Dictionary<string, NodeInformationType>();
            _localNodeInformation = ParseNodeInformationTypeFromConfig();

            SetupNetworkAdapters(multicastAdapter);
            JoinCluster();
        }
        #endregion

        #region public Methods

        /// <summary>
        /// Send a "join message" to all nodes in the multicastgroupe/cluster
        /// </summary>
        public void JoinCluster()
        {
            _multicastAdapter.SendMessageToMulticastGroup(NodeRegistryMessageFactory.GetJoinMessage(_localNodeInformation));
        }


        //sends a "leave message" to all nodes in the multicastgroupe/cluster
        public void LeaveCluster()
        {
            _multicastAdapter.SendMessageToMulticastGroup(NodeRegistryMessageFactory.GetLeaveMessage(_localNodeInformation));
        }

        /// <summary>
        /// Leaves the cluster by sending a "leave message" to the cluster. Also stop the listenThrad and shutsdown the multicast adapter.
        /// </summary>
        public void ShutDownNodeRegistry()
        {
            LeaveCluster();
            _listenThread.Interrupt();
            _multicastAdapter.CloseSocket();

        }

        /// <summary>
        /// Returns the Configuration wrapper for this NodeRegistry
        /// </summary>
        /// <returns>Configuration<T> T should be a custom config object</returns>
        public Configuration<NodeRegistryConfig> GetConfig()
        {
            return _config;
        }

        /// <summary>
        /// Returns a List of all known Nodes in the Cluster
        /// </summary>
        /// <returns>List of all known nodes. If Config.Instance.myselfToActiveNodeList is true the return values contains the localNodeInformation as well</returns>
        public List<NodeInformationType> GetAllNodes()
        {
            return _activeNodeList.Values.Select(type => type).ToList();
        }

        /// <summary>
        /// Returns a List of all known nodes from the given type
        /// </summary>
        /// <param name="nodeType">not null</param>
        /// <returns></returns>
        public List<NodeInformationType> GetAllNodesByType(NodeType nodeType)
        {
            return GetAllNodes().Where(nodeInformationType => nodeInformationType.NodeType == nodeType).ToList();
        }

        /// <summary>
        /// The given delegate get invovked as soon as a new nodes joins the cluster that is not equal the NodeInformationtype of this instance.
        /// </summary>
        /// <param name="newNodeConnectedHandler">not null. Delegate with NewNodeConnected as parameter </param>
        public void SubscribeForNewNodeConnected(NewNodeConnected newNodeConnectedHandler)
        {

            _newNodeConnectedHandler += newNodeConnectedHandler;

        }

        /// <summary>
        /// The given delegate get invovked as soon as a new nodes joins the cluster that is not equal the NodeInformationtype of this instance and the NodeType matches the given NodeTyoe.
        /// </summary>
        /// <param name="newNodeConnectedHandler">not null. Delegate with NewNodeConnected as parameter </param>
        /// <param name="nodeType">not null. The Type of the new node.</param>
        public void SubscribeForNewNodeConnectedByType(NewNodeConnected newNodeConnectedHandler, NodeType nodeType)
        {
            switch (nodeType)
            {
                case NodeType.LayerContainer:
                    _newLayerContainerConnectedHandler += newNodeConnectedHandler;
                    break;
                case NodeType.SimulationController:
                    _newSimulationControllerConnectedHandler += newNodeConnectedHandler;
                    break;
                case NodeType.SimulationManager:
                    _newSimulationManagerConnectedHandler += newNodeConnectedHandler;
                    break;
            }

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
                _config.Instance.NodeIdentifier,
                ParseNodeEndpointFromConfig()
                );
        }

        private NodeEndpoint ParseNodeEndpointFromConfig()
        {
            return new NodeEndpoint(_config.Instance.NodeEndPointIP, _config.Instance.NodeEndPointPort);
        }

        private NodeType ParseNodeTypeFromConfig()
        {
            return _config.Instance.NodeType;
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

            //check if the new node is this instance.
            if (nodeRegistryMessage.nodeInformationType.Equals(_localNodeInformation))
            {
                if (_config.Instance.AddMySelfToActiveNodeList)
                {
                    //add self to list
                    _activeNodeList[nodeRegistryMessage.nodeInformationType.NodeIdentifier] = nodeRegistryMessage.nodeInformationType;
                }
            }
            else
            {
                //add new node to list
                _activeNodeList[nodeRegistryMessage.nodeInformationType.NodeIdentifier] = nodeRegistryMessage.nodeInformationType;
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

        private void NotifyOnNodeTypeJoinSubsribers(NodeInformationType nodeInformation)
        {
            switch (nodeInformation.NodeType)
            {
                case NodeType.LayerContainer:
                    if (_newLayerContainerConnectedHandler != null)
                    {
                        _newLayerContainerConnectedHandler.Invoke(nodeInformation);
                    }
                    break;
                case NodeType.SimulationController:
                    if (_newSimulationControllerConnectedHandler != null)
                    {
                        _newSimulationControllerConnectedHandler.Invoke(nodeInformation);
                    }
                    break;
                case NodeType.SimulationManager:
                    if (_newSimulationManagerConnectedHandler != null)
                    {
                        _newSimulationManagerConnectedHandler.Invoke(nodeInformation);
                    }
                    break;
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

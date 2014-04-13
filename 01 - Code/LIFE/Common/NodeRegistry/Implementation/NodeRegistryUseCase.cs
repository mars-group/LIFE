using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using MulticastAdapter.Interface;
using NodeRegistry.Implementation.Messages;
using NodeRegistry.Interface;
using ProtoBuf;
using Timer = System.Timers.Timer;

namespace NodeRegistry.Implementation {
    using System.Collections.Concurrent;

    using Hik.Collections;

    public class NodeRegistryUseCase : INodeRegistry {
        #region Fields and Properties

        /// <summary>
        ///     A dictonary of all active Node in the cluster. The Key is the NodeIdentifier from the NodeInformationobject(Value)
        ///     and the Value is the NodeInformation object
        /// </summary>
        private ThreadSafeSortedList<String, NodeInformationType> _activeNodeList;
        //TODO use list
        /// <summary>
        /// This dictionary maps Timers NodeInformationTypes to Timers. If no HeartBeat msg from a NodeInformationType is recied the timeEvent will fire
        ///  and delte this NodeInformationType from the active NodeList
        /// </summary>
        private Dictionary<NodeInformationType, Timer> _heartBeatTimers;

        /// <summary>
        /// Heartbeat send interval duration, in ms;
        /// </summary>
        private int _heartBeatInterval;

        /// <summary>
        ///     Object that holds all relevant information about the NodeRegistry on this calculation unit.
        /// </summary>
        private readonly NodeInformationType _localNodeInformation;

        /// <summary>
        ///     Adapter that handels the network comunication.
        /// </summary>
        private IMulticastAdapter _multicastAdapter;

        /// <summary>
        ///     Configuration Object that changes the behaviour of this node Registry.
        /// </summary>
        private NodeRegistryConfig _config;

        /// <summary>
        ///     Thread that waits for multicast messages.
        /// </summary>
        private Thread _listenThread;

        /// <summary>
        /// Thread that is sending periodically HeartBeatMessages to the cluster.
        /// </summary>
        private Thread _heartBeatSenderThread;

        /// <summary>
        ///     delegates for different subscriber types.
        /// </summary>
        private NewNodeConnected _newNodeConnectedHandler;

        private NewNodeConnected _newLayerContainerConnectedHandler;
        private NewNodeConnected _newSimulationManagerConnectedHandler;
        private NewNodeConnected _newSimulationControllerConnectedHandler;

        #endregion

        #region Constructors

        public NodeRegistryUseCase(NodeInformationType nodeInformation, IMulticastAdapter multicastAdapter,
            NodeRegistryConfig nodeRegistryConfig) {
            InitNodeRegistry(nodeRegistryConfig);

            _localNodeInformation = nodeInformation;

            SetupNetworkAdapters(multicastAdapter);
            JoinCluster();
        }

        public NodeRegistryUseCase(IMulticastAdapter multicastAdapter, NodeRegistryConfig nodeRegistryConfig) {
            InitNodeRegistry(nodeRegistryConfig);

<<<<<<< .mine
            _localNodeInformation = ParseNodeInformationTypeFromConfig();
=======
            _activeNodeList = new ThreadSafeSortedList<string, NodeInformationType>();
>>>>>>> .theirs

            SetupNetworkAdapters(multicastAdapter);
            JoinCluster();

            _heartBeatSenderThread = new Thread(this.HeartBeat);
            _heartBeatSenderThread.Start();
        }


        private void InitNodeRegistry(NodeRegistryConfig nodeRegistryConfig) {
            _config = nodeRegistryConfig;

            _heartBeatTimers = new Dictionary<NodeInformationType, Timer>();
            _activeNodeList = new ThreadSafeSortedList<string, NodeInformationType>();
            _heartBeatInterval = _config.HeartBeatInterval;
        }


        private void SetupNetworkAdapters(IMulticastAdapter multicastAdapter) {
            _multicastAdapter = multicastAdapter;
            _listenThread = new Thread(Listen);
            _listenThread.Start();
        }

        #endregion

        #region public Methods

        /// <summary>
        ///     Send a "join message" to all nodes in the multicastgroupe/cluster
        /// </summary>
        public void JoinCluster() {
            _multicastAdapter.SendMessageToMulticastGroup(
                NodeRegistryMessageFactory.GetJoinMessage(_localNodeInformation));

            _heartBeatSenderThread = new Thread(this.HeartBeat);
            _heartBeatSenderThread.Start();
        }


        //sends a "leave message" to all nodes in the multicastgroupe/cluster
        public void LeaveCluster() {
            _multicastAdapter.SendMessageToMulticastGroup(
                NodeRegistryMessageFactory.GetLeaveMessage(_localNodeInformation));
        }

        /// <summary>
        ///     Leaves the cluster by sending a "leave message" to the cluster. Also stop the listenThrad and shutsdown the
        ///     multicast adapter.
        /// </summary>
        public void ShutDownNodeRegistry() {
            LeaveCluster();
            _listenThread.Interrupt();
            _heartBeatSenderThread.Interrupt();
            _multicastAdapter.CloseSocket();
        }

        /// <summary>
        ///     Returns the Configuration wrapper for this NodeRegistry
        /// </summary>
        /// <returns>Configuration<T> T should be a custom config object</returns>
        public NodeRegistryConfig GetConfig() {
            return _config;
        }

        /// <summary>
        ///     Returns a List of all known Nodes in the Cluster
        /// </summary>
        /// <returns>
        ///     List of all known nodes. If Config.Instance.myselfToActiveNodeList is true the return values contains the
        ///     localNodeInformation as well
        /// </returns>
        public List<NodeInformationType> GetAllNodes() {
            return _activeNodeList.GetAllItems();
        }

        /// <summary>
        ///     Returns a List of all known nodes from the given type
        /// </summary>
        /// <param name="nodeType">not null</param>
        /// <returns></returns>
        public List<NodeInformationType> GetAllNodesByType(NodeType nodeType) {
            return GetAllNodes().Where(nodeInformationType => nodeInformationType.NodeType == nodeType).ToList();
        }

        /// <summary>
        ///     The given delegate get invovked as soon as a new nodes joins the cluster that is not equal the NodeInformationtype
        ///     of this instance.
        /// </summary>
        /// <param name="newNodeConnectedHandler">not null. Delegate with NewNodeConnected as parameter </param>
        public void SubscribeForNewNodeConnected(NewNodeConnected newNodeConnectedHandler) {
            _newNodeConnectedHandler += newNodeConnectedHandler;
        }

        /// <summary>
        ///     The given delegate get invovked as soon as a new nodes joins the cluster that is not equal the NodeInformationtype
        ///     of this instance and the NodeType matches the given NodeTyoe.
        /// </summary>
        /// <param name="newNodeConnectedHandler">not null. Delegate with NewNodeConnected as parameter </param>
        /// <param name="nodeType">not null. The Type of the new node.</param>
        public void SubscribeForNewNodeConnectedByType(NewNodeConnected newNodeConnectedHandler, NodeType nodeType) {
            switch (nodeType) {
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

        private NodeInformationType ParseNodeInformationTypeFromConfig() {
            return new NodeInformationType(
                ParseNodeTypeFromConfig(),
                _config.NodeIdentifier,
                ParseNodeEndpointFromConfig()
                );
        }

        private NodeEndpoint ParseNodeEndpointFromConfig() {
            return new NodeEndpoint(_config.NodeEndPointIP, _config.NodeEndPointPort);
        }

        private NodeType ParseNodeTypeFromConfig() {
            return _config.NodeType;
        }


        private void _resetTimer(NodeInformationType nodeInformationType) {}

        private void NotifyOnNodeJoinSubsribers(NodeInformationType nodeInformation) {
            if (_newNodeConnectedHandler != null) _newNodeConnectedHandler.Invoke(nodeInformation);
        }

        private void NotifyOnNodeTypeJoinSubsribers(NodeInformationType nodeInformation) {
            switch (nodeInformation.NodeType) {
                case NodeType.LayerContainer:
                    if (_newLayerContainerConnectedHandler != null)
                        _newLayerContainerConnectedHandler.Invoke(nodeInformation);
                    break;
                case NodeType.SimulationController:
                    if (_newSimulationControllerConnectedHandler != null)
                        _newSimulationControllerConnectedHandler.Invoke(nodeInformation);
                    break;
                case NodeType.SimulationManager:
                    if (_newSimulationManagerConnectedHandler != null)
                        _newSimulationManagerConnectedHandler.Invoke(nodeInformation);
                    break;
            }
        }


        private void DropAllNodes() {
            _activeNodeList = new Dictionary<string, NodeInformationType>();
        }

        private void Listen() {
            while (Thread.CurrentThread.IsAlive) {
                byte[] msg = _multicastAdapter.readMulticastGroupMessage();
                var stream = new MemoryStream(msg);
                var nodeRegistryMessage = Serializer.Deserialize<NodeRegistryMessage>(stream);
                AnswerMessage(nodeRegistryMessage);
            }
        }


        private void AnswerMessage(NodeRegistryMessage nodeRegistryMessage) {
            switch (nodeRegistryMessage.messageType) {
                case NodeRegistryMessageType.Answer:
                OnAnswerMessage(nodeRegistryMessage);
                    break;
                case NodeRegistryMessageType.Join:
                    OnJoinMessage(nodeRegistryMessage);
                    break;
                case NodeRegistryMessageType.Leave:
                    _activeNodeList.Remove(nodeRegistryMessage.nodeInformationType.NodeIdentifier);
                    break;
                case NodeRegistryMessageType.HeartBeat:
                    _resetTimer(nodeRegistryMessage.nodeInformationType);
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
                if (_config.AddMySelfToActiveNodeList)
                {
                    //add self to list
                    _activeNodeList[nodeRegistryMessage.nodeInformationType.NodeIdentifier] =
                        nodeRegistryMessage.nodeInformationType;
                }
            }
            else
            {
                //add new node to list
                _activeNodeList[nodeRegistryMessage.nodeInformationType.NodeIdentifier] =
                    nodeRegistryMessage.nodeInformationType;
                //notify all subsribers
                NotifyOnNodeJoinSubsribers(nodeRegistryMessage.nodeInformationType);
                NotifyOnNodeTypeJoinSubsribers(nodeRegistryMessage.nodeInformationType);

                // send my information to the new node
                _multicastAdapter.SendMessageToMulticastGroup(
                    NodeRegistryMessageFactory.GetAnswerMessage(_localNodeInformation));
            }
        }


        private void OnAnswerMessage(NodeRegistryMessage nodeRegistryMessage) {
             //add answer node to list
            _activeNodeList[nodeRegistryMessage.nodeInformationType.NodeIdentifier] =
                nodeRegistryMessage.nodeInformationType;
            //start HeartbeatTimer for Node.
        }

        /// <summary>
        /// check if all active nodes are still up. Hopefully that HartBeat wont bleed like Open SSL.
        /// </summary>
        private void HeartBeat() {
            while (Thread.CurrentThread.IsAlive) {
                Thread.Sleep(_heartBeatInterval);
                _multicastAdapter.SendMessageToMulticastGroup(
                    NodeRegistryMessageFactory.GetHeartBeatMessage(_localNodeInformation));
            }
        }

        #endregion
    }
}
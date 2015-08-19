using System;
using System.IO;
using System.Threading;
using CommonTypes.DataTypes;
using log4net;
using MulticastAdapter.Interface;
using NodeRegistry.Implementation.Messages;
using NodeRegistry.Implementation.Messages.Factory;
using ProtoBuf;

namespace NodeRegistry.Implementation.UseCases {

    internal class NodeRegistryNetworkUseCase {
        private readonly TNodeInformation _localNodeInformation;
        private readonly NodeRegistryNodeManagerUseCase _nodeRegistryNodeManagerUse;
        private readonly NodeRegistryHeartBeatUseCase _nodeRegistryHeartBeatUseCase;

        private readonly IMulticastAdapter _multicastAdapter;
        private readonly Boolean _addMySelfToActiveNodeList;

        private readonly Thread _listenThread;
        private ILog Logger;

        public NodeRegistryNetworkUseCase
            (NodeRegistryNodeManagerUseCase nodeManagerUseCase,
                NodeRegistryHeartBeatUseCase heartBeatUseCase,
                TNodeInformation localNodeInformation,
                bool addMySelfToActiveNodeList,
                IMulticastAdapter multicastAdapter) {
            _nodeRegistryNodeManagerUse = nodeManagerUseCase;
            _multicastAdapter = multicastAdapter;
            _nodeRegistryHeartBeatUseCase = heartBeatUseCase;
            _localNodeInformation = localNodeInformation;
            _addMySelfToActiveNodeList = addMySelfToActiveNodeList;
            _nodeRegistryHeartBeatUseCase = heartBeatUseCase;

            _listenThread = new Thread(Listen);
            _listenThread.Start();

            JoinCluster();
        }

        public void JoinCluster() {
            _multicastAdapter.SendMessageToMulticastGroup
                (
                    NodeRegistryMessageFactory.GetJoinMessage
                        (_localNodeInformation, _localNodeInformation.NodeEndpoint.IpAddress));
        }

        public void LeaveCluster() {
            _multicastAdapter.SendMessageToMulticastGroup
                (
                    NodeRegistryMessageFactory.GetLeaveMessage
                        (_localNodeInformation, _localNodeInformation.NodeEndpoint.IpAddress));
        }

        public void Shutdown() {
            _listenThread.Interrupt();
        }

        private void Listen() {
            try {
                while (Thread.CurrentThread.IsAlive) {
                    byte[] msg = _multicastAdapter.readMulticastGroupMessage();
                    MemoryStream stream = new MemoryStream(msg);
                    stream.Position = 0;

                    if (stream.Length > 0) {
                        AbstractNodeRegistryMessage nodeRegistryMessage =
                            Serializer.Deserialize<AbstractNodeRegistryMessage>(stream);
                        ComputeMessage(nodeRegistryMessage);
                    }
                }
            }
            catch (ThreadInterruptedException ex) {
                Logger.Debug("Message lost in local NodeRegistry. Reason was: \n" + ex);
            }
            catch (ProtoException ex) {
                Logger.Debug("Message lost in local NodeRegistry. Reason was: \n" + ex);
            }
			catch (ObjectDisposedException ex){
				Logger.Debug("Call to disposed multicast adapter happenend. Usually not problematic, since it only happens when shutting down.");
			}
        }

        private void ComputeMessage(AbstractNodeRegistryMessage nodeRegistryConnectionInfoMessage) {
            if (nodeRegistryConnectionInfoMessage == null) {
                return;
            }
            // check for Reasonableness of incoming message
            if (!CheckReasonableness(nodeRegistryConnectionInfoMessage)) {
                return;
            }

            switch (nodeRegistryConnectionInfoMessage.MessageType) {
                case NodeRegistryMessageType.Answer:
                    OnAnswerMessage(nodeRegistryConnectionInfoMessage as NodeRegistryConnectionInfoMessage);
                    break;
                case NodeRegistryMessageType.Join:
                    OnJoinMessage(nodeRegistryConnectionInfoMessage as NodeRegistryConnectionInfoMessage);
                    break;
                case NodeRegistryMessageType.Leave:
                    OnLeaveMessage(nodeRegistryConnectionInfoMessage as NodeRegistryConnectionInfoMessage);
                    break;
                case NodeRegistryMessageType.HeartBeat:
                    OnHeartBeatMessage(nodeRegistryConnectionInfoMessage as NodeRegistryHeartBeatMessage);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        ///     Checks if the incoming message is reasonable to compute.
        /// </summary>
        /// <param name="nodeRegistryConnectionInfoMessage"></param>
        /// <returns>
        ///     Returns false if and only if nodeRegistryConnectionInfoMessage does not originate from this host
        ///     and the nodeInformation Field contains a localhost Endpoint information.
        /// </returns>
        private bool CheckReasonableness(AbstractNodeRegistryMessage nodeRegistryConnectionInfoMessage) {
            if (nodeRegistryConnectionInfoMessage == null) {
                return false;
            }

            // Heartbeat and Leave Messages are always ok
            if (nodeRegistryConnectionInfoMessage.MessageType == NodeRegistryMessageType.Leave
                || nodeRegistryConnectionInfoMessage.MessageType == NodeRegistryMessageType.HeartBeat) {
                return true;
            }

            // if message cannot be cast to NodeRegistry ConnectionMessage, something's gone wrong
            NodeRegistryConnectionInfoMessage connectionInfoMessage =
                nodeRegistryConnectionInfoMessage as NodeRegistryConnectionInfoMessage;
            if (connectionInfoMessage == null) {
                return false;
            }

            return connectionInfoMessage.OriginAddress == _localNodeInformation.NodeEndpoint.IpAddress
                   || connectionInfoMessage.NodeInformation.NodeEndpoint.IpAddress != "127.0.0.1";
        }

        private void OnJoinMessage(NodeRegistryConnectionInfoMessage nodeRegistryConnectionInfoMessage) {
            //check if the new node is this instance.
            if (nodeRegistryConnectionInfoMessage.NodeInformation.Equals(_localNodeInformation)) {
                if (_addMySelfToActiveNodeList) {
                    //add self to list
                    _nodeRegistryNodeManagerUse.AddNode(nodeRegistryConnectionInfoMessage.NodeInformation);
                }
            }
            else {
                //add new node to list
                _nodeRegistryNodeManagerUse.AddNode(nodeRegistryConnectionInfoMessage.NodeInformation);

                // send my information to the new node
                _multicastAdapter.SendMessageToMulticastGroup
                    (
                        NodeRegistryMessageFactory.GetAnswerMessage
                            (_localNodeInformation, _localNodeInformation.NodeEndpoint.IpAddress));
            }
        }

        private void OnLeaveMessage(NodeRegistryConnectionInfoMessage nodeRegistryConnectionInfoMessage) {
            _nodeRegistryNodeManagerUse.RemoveNode(nodeRegistryConnectionInfoMessage.NodeInformation);
        }

        private void OnAnswerMessage(NodeRegistryConnectionInfoMessage nodeRegistryConnectionInfoMessage) {
            //add answer node to list
            _nodeRegistryNodeManagerUse.AddNode(nodeRegistryConnectionInfoMessage.NodeInformation);

            _nodeRegistryHeartBeatUseCase.CreateAndStartTimerForNodeEntry
                (nodeRegistryConnectionInfoMessage.NodeInformation);
        }

        private void OnHeartBeatMessage(NodeRegistryHeartBeatMessage heartBeatMessage) {
            _nodeRegistryHeartBeatUseCase.ResetTimer(heartBeatMessage.NodeIdentifier, heartBeatMessage.NodeType);
        }
    }

}
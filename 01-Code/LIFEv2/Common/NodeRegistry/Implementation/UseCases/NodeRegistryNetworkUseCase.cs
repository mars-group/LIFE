//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 25.01.2016
//  *******************************************************/
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommonTypes.DataTypes;
using MulticastAdapter.Interface;
using Newtonsoft.Json;
using NodeRegistry.Implementation.Messages;
using NodeRegistry.Implementation.Messages.Factory;


namespace NodeRegistry.Implementation.UseCases {

    internal class NodeRegistryNetworkUseCase {
        private readonly TNodeInformation _localNodeInformation;
        private readonly NodeRegistryNodeManagerUseCase _nodeRegistryNodeManagerUse;
        private readonly NodeRegistryHeartBeatUseCase _nodeRegistryHeartBeatUseCase;

        private readonly IMulticastAdapter _multicastAdapter;
        private readonly bool _addMySelfToActiveNodeList;

        //private readonly Thread _listenThread;
        private Task _listenTask;

        private CancellationTokenSource _tokenSource;
        private readonly string _clusterName;

        public NodeRegistryNetworkUseCase
            (NodeRegistryNodeManagerUseCase nodeManagerUseCase,
                NodeRegistryHeartBeatUseCase heartBeatUseCase,
                TNodeInformation localNodeInformation,
                bool addMySelfToActiveNodeList,
                IMulticastAdapter multicastAdapter,
                string clusterName) {
            _clusterName = clusterName;
            _nodeRegistryNodeManagerUse = nodeManagerUseCase;
            _multicastAdapter = multicastAdapter;
            _nodeRegistryHeartBeatUseCase = heartBeatUseCase;
            _localNodeInformation = localNodeInformation;
            _addMySelfToActiveNodeList = addMySelfToActiveNodeList;
            _nodeRegistryHeartBeatUseCase = heartBeatUseCase;

            _tokenSource = new CancellationTokenSource();
            var ct = _tokenSource.Token;

            Task.Factory.StartNew(() =>
            {
                // Were we already canceled?
                //ct.ThrowIfCancellationRequested();

                while (!ct.IsCancellationRequested)
                {
                    byte[] msg = _multicastAdapter.readMulticastGroupMessage();


                    if (msg.Length <= 0) continue;

                    var nodeRegistryMessage =
                        JsonConvert.DeserializeObject<AbstractNodeRegistryMessage>(Encoding.UTF8.GetString(msg));

                    // check whether the message belongs to our cluster, if not throw away
                    if (nodeRegistryMessage.ClusterName != null &&
                        nodeRegistryMessage.ClusterName != _clusterName)
                    {
                        continue;
                    }

                    // message is ok, so compute
                    ComputeMessage(nodeRegistryMessage);
                }
            }, _tokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            JoinCluster();
        }

        public void JoinCluster() {
            _multicastAdapter.SendMessageToMulticastGroup
                (
                    NodeRegistryMessageFactory.GetJoinMessage
                        (_localNodeInformation, _localNodeInformation.NodeEndpoint.IpAddress, _clusterName));
        }

        public void LeaveCluster() {
            _multicastAdapter.SendMessageToMulticastGroup
                (
                    NodeRegistryMessageFactory.GetLeaveMessage
                        (_localNodeInformation, _localNodeInformation.NodeEndpoint.IpAddress, _clusterName));
        }

        public void Shutdown() {
            _tokenSource.Cancel();
        }

        private void Listen() {


			
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
                                     (_localNodeInformation, _localNodeInformation.NodeEndpoint.IpAddress, _clusterName));
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
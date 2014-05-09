using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommonTypes.DataTypes;
using log4net;
using MulticastAdapter.Interface;
using NodeRegistry.Implementation.Messages;
using NodeRegistry.Implementation.Messages.Factory;
using ProtoBuf;

namespace NodeRegistry.Implementation.UseCases
{
    class NodeRegistryNetworkUseCase
    {

        private ILog Logger;

        private TNodeInformation _localNodeInformation;
        private NodeRegistryNodeManagerUseCase _nodeRegistryNodeManagerUse;
        private NodeRegistryHeartBeatUseCase _nodeRegistryHeartBeatUseCase;

        private IMulticastAdapter _multicastAdapter;
        private Boolean _addMySelfToActiveNodeList;

        private Thread _listenThread;

        public NodeRegistryNetworkUseCase(NodeRegistryNodeManagerUseCase nodeManagerUseCase, NodeRegistryHeartBeatUseCase heartBeatUseCase,
            TNodeInformation localNodeInformation, bool addMySelfToActiveNodeList, IMulticastAdapter multicastAdapter)
        {
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

        public void JoinCluster()
        {
            _multicastAdapter.SendMessageToMulticastGroup(
                NodeRegistryMessageFactory.GetJoinMessage(_localNodeInformation));
        }

        public void LeaveCluster()
        {
            _multicastAdapter.SendMessageToMulticastGroup(
                NodeRegistryMessageFactory.GetLeaveMessage(_localNodeInformation));
        }

        public void Shutdown()
        {
            _listenThread.Interrupt();
        }

        private void Listen()
        {
            try
            {
                while (Thread.CurrentThread.IsAlive)
                {
                    byte[] msg = _multicastAdapter.readMulticastGroupMessage();
                    var stream = new MemoryStream(msg);
                    stream.Position = 0;

                    if (stream.Length > 0)
                    {
                        var nodeRegistryMessage = Serializer.Deserialize<AbstractNodeRegistryMessage>(stream);
                        ComputeMessage(nodeRegistryMessage);

                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is ThreadInterruptedException || ex is ProtoException)
                {
                    Logger.Debug("Message lost in local NodeRegistry. Reason was: \n" + ex );
                }
                else
                {
                    throw;
                }




            }
        }

        private void ComputeMessage(AbstractNodeRegistryMessage nodeRegistryConnectionInfoMessage)
        {
            switch (nodeRegistryConnectionInfoMessage.MessageType)
            {
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

        private void OnJoinMessage(NodeRegistryConnectionInfoMessage nodeRegistryConnectionInfoMessage)
        {
            //check if the new node is this instance.
            if (nodeRegistryConnectionInfoMessage.NodeInformation.Equals(_localNodeInformation))
            {
                if (_addMySelfToActiveNodeList)
                {
                    //add self to list
                    _nodeRegistryNodeManagerUse.AddNode(nodeRegistryConnectionInfoMessage.NodeInformation);
                }
            }
            else
            {
                //add new node to list
                _nodeRegistryNodeManagerUse.AddNode(nodeRegistryConnectionInfoMessage.NodeInformation);

                // send my information to the new node
                _multicastAdapter.SendMessageToMulticastGroup(
                    NodeRegistryMessageFactory.GetAnswerMessage(_localNodeInformation));
            }
        }

        private void OnLeaveMessage(NodeRegistryConnectionInfoMessage nodeRegistryConnectionInfoMessage)
        {
            _nodeRegistryNodeManagerUse.RemoveNode(nodeRegistryConnectionInfoMessage.NodeInformation);

        }

        private void OnAnswerMessage(NodeRegistryConnectionInfoMessage nodeRegistryConnectionInfoMessage)
        {
            //add answer node to list
            _nodeRegistryNodeManagerUse.AddNode(nodeRegistryConnectionInfoMessage.NodeInformation);

            _nodeRegistryHeartBeatUseCase.CreateAndStartTimerForNodeEntry(nodeRegistryConnectionInfoMessage.NodeInformation);

        }

        private void OnHeartBeatMessage(NodeRegistryHeartBeatMessage heartBeatMessage)
        {
            _nodeRegistryHeartBeatUseCase.ResetTimer(heartBeatMessage.NodeIdentifier, heartBeatMessage.NodeType);

        }


    }
}

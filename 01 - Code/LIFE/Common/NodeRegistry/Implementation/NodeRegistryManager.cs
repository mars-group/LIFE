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
        private List<NodeInformationType> activeNodeList;
        private IMulticastReciever reciever;
        private IMulticastClientAdapter clientAdapter;
        private NodeInformationType nodeInformation;
        private Thread listenThread;


        public NodeRegistryManager(NodeInformationType nodeInformation)
        {
            activeNodeList = new List<NodeInformationType>();
            reciever = new UDPMulticastReceiver();
            clientAdapter = new UDPMulticastClient();
            this.nodeInformation = nodeInformation;
            this.listenThread = new Thread(new ThreadStart(this.Listen));
            listenThread.Start();

        }

    

        public void DropAllNodes()
        {
            activeNodeList = new List<NodeInformationType>();
        }

        public  void Listen()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                byte[] msg = reciever.readMulticastGroupMessage();
                var stream = new MemoryStream(msg);
                var nodeRegestryMessage = Serializer.Deserialize<NodeRegistryMessage>(stream);
                AnswerMessage(nodeRegestryMessage); 
            }
            
        }

        private  void AnswerMessage(NodeRegistryMessage nodeRegestryMessage)
        {

          
             switch (nodeRegestryMessage.messageType)
            {
                case NodeRegistryMessageType.Answer:
                    if (! activeNodeList.Contains(nodeRegestryMessage.nodeInformationType))
                    {
                        activeNodeList.Add(nodeRegestryMessage.nodeInformationType);
                    }
                    
                    break;
                case NodeRegistryMessageType.Join:
                    clientAdapter.SendMessageToMulticastGroup(NodeRegistryMessageFactory.GetAnswerMessage(nodeInformation));
                    break;
                case NodeRegistryMessageType.Leave:
                    activeNodeList.Remove(nodeRegestryMessage.nodeInformationType);
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
            return activeNodeList.Select(type => type).ToList();
        }

        public List<NodeInformationType> GetAllNodesByType(NodeType nodeType)
        {
            return GetAllNodes().Where(nodeInformationType => nodeInformationType.NodeType == nodeType).ToList();
        }
    }

}

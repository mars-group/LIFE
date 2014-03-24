using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface;
using NodeRegistry.Implementation.Messages;
using NodeRegistry.Interface;

namespace NodeRegistry.Implementation
{
    class NodeRegistryImpl : INodeRegistry
    {
        private NodeRegistryManager nodeRegistryManager;
        private IMulticastClientAdapter clientAdapter;
        private NodeInformationType nodeInformation;

        public NodeRegistryImpl(NodeInformationType nodeInformation)
        {
            nodeRegistryManager = new NodeRegistryManager();
            clientAdapter = new UDPMulticastClient();
            this.nodeInformation = nodeInformation;
        }


        public void startDiscovery()
        {
           JoinGroupe();
        }

        public void restartDiscovery()
        {
            
            
            
        }

        public List<NodeInformationType> GetAllNodes()
        {
            return nodeRegistryManager.GetActiveNodes();
        }

        public List<NodeInformationType> GetAllNodesByType(NodeType nodeType)
        {
            List<NodeInformationType> resultList = new List<NodeInformationType>();

            foreach (var nodeInformationType in GetAllNodes())
            {
                if (nodeInformationType.NodeType == nodeType)
                {
                    resultList.Add(nodeInformationType);
                }
            }

            return resultList;
        }

        private void JoinGroupe()
        {
            clientAdapter.SendMessageToMulticastGroup(MulticastMessageFactory.GetJoinMessage(nodeInformation));
        }

        private void LeaveGroup()
        {
            
        }

    }
}

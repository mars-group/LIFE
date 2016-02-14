//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using NodeRegistry.Interface;
using NUnit.Framework;

namespace RTEManagerBlackBoxTest.Mocks
{
    public class NodeRegistryMock : INodeRegistry
    {
        public event EventHandler<TNodeInformation> SimulationManagerConnected;


        private List<TNodeInformation> _nodeInformations;

        public NodeRegistryMock()
        {
            this._nodeInformations = new List<TNodeInformation>() { new TNodeInformation(NodeType.LayerContainer, "RTEMockTest", new NodeEndpoint("127.0.0.1", 7900)) }; 
        }


        public List<TNodeInformation> GetAllNodes()
        {
            return _nodeInformations;
        }

        public List<TNodeInformation> GetAllNodesByType(NodeType nodeType)
        {
            throw new NotImplementedException();
        }

        public void SubscribeForNewNodeConnected(NewNodeConnected newNodeConnectedHandler)
        {
            throw new NotImplementedException();
        }

        public void SubscribeForNewNodeConnectedByType(NewNodeConnected newNodeConnectedHandler, NodeType nodeType)
        {
            throw new NotImplementedException();
        }

        public void SubscribeForNodeDisconnected(NodeDisconnected nodeDisconnectedHandler, TNodeInformation node)
        {
            throw new NotImplementedException();
        }

        public void LeaveCluster()
        {
            throw new NotImplementedException();
        }

        public void JoinCluster()
        {
            throw new NotImplementedException();
        }

        public void ShutDownNodeRegistry()
        {
            throw new NotImplementedException();
        }
    }
}

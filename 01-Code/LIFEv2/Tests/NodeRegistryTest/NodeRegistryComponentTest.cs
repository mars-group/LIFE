//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 25.01.2016
//  *******************************************************/

using System;
using System.Collections.Generic;
using System.Threading;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using ConfigurationAdapter;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface.Config;
using NodeRegistry.Implementation;
using NodeRegistry.Interface;
using NUnit.Framework;

namespace NodeRegistryTest {

    /// <summary>
    ///     Summary description for NodeRegistryComponentTest
    /// </summary>
    [TestFixture]
    public class NodeRegistryComponentTest {
        #region Setup/Teardown

        [SetUp]
        public void Setup() {
            _information = new TNodeInformation
                (
                NodeType.SimulationManager,
                "UnitTestNode0",
                new NodeEndpoint("127.0.0.1", 55500)
                );
        }

        #endregion

        private TNodeInformation _information;
        private int _listenStartPortSeed = 50000;
        private int _sendingStartPortSeed = 52500;

        [Test]
        public void TestInitialization() {
            int localListenPort = _listenStartPortSeed;
            _listenStartPortSeed += 1;
            int localSendingPort = _sendingStartPortSeed;
            _sendingStartPortSeed += 1;

            GlobalConfig globalConfig = new GlobalConfig("239.0.0.1", localListenPort, localSendingPort, 4);

            MulticastAdapterComponent multicastAdapter = new MulticastAdapterComponent
                (globalConfig, new MulticastSenderConfig());

            //test if the NodeRegistryUseCase can be bootstrapped from a config entry
            NodeRegistryComponent nr = new NodeRegistryComponent
                (
                multicastAdapter,
                new NodeRegistryConfig
                    (
                    _information.NodeType,
                    _information.NodeIdentifier,
                    _information.NodeEndpoint.IpAddress,
                    _information.NodeEndpoint.Port,
                    true), null);
            Assert.True(nr != null);

            nr.ShutDownNodeRegistry();
        }


        [Test]
        public void TestJoinAndLeaveClusterLocal() {
            string localMulticastGrp = "239.0.0.2";

            int localListenPort = _listenStartPortSeed;
            _listenStartPortSeed += 1;
            int localSendingPort = _sendingStartPortSeed;
            _sendingStartPortSeed += 1;


            MulticastAdapterComponent multicastAdapter =
                new MulticastAdapterComponent
                    (
                    new GlobalConfig(localMulticastGrp, localListenPort, localSendingPort, 4),
                    new MulticastSenderConfig());

            TNodeInformation localNodeInfo = _information;
            NodeRegistryComponent localNodeRegistry = new NodeRegistryComponent
                (
                multicastAdapter,
                new NodeRegistryConfig
                    (
                    localNodeInfo.NodeType,
                    localNodeInfo.NodeIdentifier,
                    localNodeInfo.NodeEndpoint.IpAddress,
                    localNodeInfo.NodeEndpoint.Port,
                    true), null);

            //Just to make sure 
            localNodeRegistry.JoinCluster();

            Thread.Sleep(300);
            //check if this node has joined the cluster

            Assert.True(localNodeRegistry.GetAllNodes().Contains(localNodeInfo));

            localNodeRegistry.LeaveCluster();

            Thread.Sleep(200);

            Assert.True(!localNodeRegistry.GetAllNodes().Contains(localNodeInfo));

            localNodeRegistry.ShutDownNodeRegistry();
        }

        [Test]
        public void TestNewNodeSubscrition() {
            var localMulticastGrp = "239.0.0.3";

            var localNodeInfo = _information;

            var nodeType = localNodeInfo.NodeType;

            var otherNodeinfo = new TNodeInformation
                (
                localNodeInfo.NodeType,
					"UnitTestNode",
                new NodeEndpoint("127.0.0.1", 40862));

            var newNodeSubscriberFired = false;
            var newNodeOftypeSubscriberFired = false;

            var localListenPort = _listenStartPortSeed;
            _listenStartPortSeed += 1;
            var localSendingPort = _sendingStartPortSeed;
            _sendingStartPortSeed += 1;

            var multicastAdapter =
                new MulticastAdapterComponent
                    (
                    new GlobalConfig(localMulticastGrp, localListenPort, localSendingPort, 4),
                    new MulticastSenderConfig());

			var otherMulticastAdapter =
				new MulticastAdapterComponent
				(
					new GlobalConfig(localMulticastGrp, localListenPort, localSendingPort++, 4),
					new MulticastSenderConfig());
			_sendingStartPortSeed += 1;

            var localNodeRegistry = new NodeRegistryComponent
                (
                multicastAdapter,
                new NodeRegistryConfig
                    (
                    localNodeInfo.NodeType,
                    localNodeInfo.NodeIdentifier,
                    localNodeInfo.NodeEndpoint.IpAddress,
                    localNodeInfo.NodeEndpoint.Port,
                    true), null);

            //subscribe for events
            localNodeRegistry.SubscribeForNewNodeConnected
                (
                    delegate(TNodeInformation nodeInformation) {
                        if (nodeInformation.Equals(otherNodeinfo)) {
                            newNodeSubscriberFired = true;
                        }
                    });

            localNodeRegistry.SubscribeForNewNodeConnectedByType
                (
                    delegate(TNodeInformation nodeInformation) {
                        if (nodeInformation.Equals(otherNodeinfo)) {
                            newNodeOftypeSubscriberFired = true;
                        }
                    },
                    nodeType);

            var otherNodeRegistry = new NodeRegistryComponent
                (
                otherMulticastAdapter,
                new NodeRegistryConfig
                    (
                    otherNodeinfo.NodeType,
                    otherNodeinfo.NodeIdentifier,
                    otherNodeinfo.NodeEndpoint.IpAddress,
                    otherNodeinfo.NodeEndpoint.Port,
                    true), null);

            Thread.Sleep(1000);

            if (!newNodeSubscriberFired || !newNodeOftypeSubscriberFired) {
                Thread.Sleep(500);
            }

            Assert.True(newNodeSubscriberFired);
            Assert.True(newNodeOftypeSubscriberFired);


            localNodeRegistry.ShutDownNodeRegistry();
        }

        [Test]
        public void TestClusterDivision()
        {
            var localMulticastGrp = "239.0.0.3";

            var localNodeInfo = _information;

            var nodeType = localNodeInfo.NodeType;

            var clusterNode1Info = new TNodeInformation
                (
                localNodeInfo.NodeType,
                    "UnitTestNode1",
                new NodeEndpoint("127.0.0.1", 40862));

            var clusterNode2Info = new TNodeInformation
                (
                localNodeInfo.NodeType,
                    "UnitTestNode2",
                new NodeEndpoint("127.0.0.1", 40863));

            var newNodeSubscriberFiredNonClusterNode = false;
            var newNodeOftypeSubscriberFiredNonClusterNode = false;
            var newNodeSubscriberFiredClusterNode1 = false;
            var newNodeOftypeSubscriberFiredClusterNode1 = false;
            var newNodeSubscriberFiredClusterNode2 = false;
            var newNodeOftypeSubscriberFiredClusterNode2 = false;

            var localListenPort = _listenStartPortSeed;
            _listenStartPortSeed += 1;
            var localSendingPort = _sendingStartPortSeed;
            _sendingStartPortSeed += 1;

            var multicastAdapter =
                new MulticastAdapterComponent
                    (
                    new GlobalConfig(localMulticastGrp, localListenPort, localSendingPort, 4),
                    new MulticastSenderConfig());

            var clusterMulticastAdapter1 =
                new MulticastAdapterComponent
                (
                    new GlobalConfig(localMulticastGrp, localListenPort, localSendingPort++, 4),
                    new MulticastSenderConfig());

            var clusterMulticastAdapter2 =
                new MulticastAdapterComponent
                (
                    new GlobalConfig(localMulticastGrp, localListenPort, localSendingPort++, 4),
                    new MulticastSenderConfig());

            var localNodeRegistry = new NodeRegistryComponent
                (
                multicastAdapter,
                new NodeRegistryConfig
                    (
                    localNodeInfo.NodeType,
                    localNodeInfo.NodeIdentifier,
                    localNodeInfo.NodeEndpoint.IpAddress,
                    localNodeInfo.NodeEndpoint.Port,
                    true), null);

            //subscribe for events
            localNodeRegistry.SubscribeForNewNodeConnected
                (
                    delegate (TNodeInformation nodeInformation) {
                        if (nodeInformation.Equals(clusterNode1Info) || nodeInformation.Equals(clusterNode2Info))
                        {
                            newNodeSubscriberFiredNonClusterNode = true;
                        }
                    });

            localNodeRegistry.SubscribeForNewNodeConnectedByType
                (
                    delegate (TNodeInformation nodeInformation) {
                        if (nodeInformation.Equals(clusterNode1Info) || nodeInformation.Equals(clusterNode2Info))
                        {
                            newNodeOftypeSubscriberFiredNonClusterNode = true;
                        }
                    },
                    nodeType);

            var clusterNodeRegistry1 = new NodeRegistryComponent
                (
                clusterMulticastAdapter1,
                new NodeRegistryConfig
                    (
                    clusterNode1Info.NodeType,
                    clusterNode1Info.NodeIdentifier,
                    clusterNode1Info.NodeEndpoint.IpAddress,
                    clusterNode1Info.NodeEndpoint.Port,
                    true), "cluster1");

            //subscribe for events
            clusterNodeRegistry1.SubscribeForNewNodeConnected
                (
                    nodeInformation =>
                    {
                        if (nodeInformation.Equals(clusterNode2Info))
                        {
                            newNodeSubscriberFiredClusterNode1 = true;
                        }
                    });

            clusterNodeRegistry1.SubscribeForNewNodeConnectedByType
                (
                    nodeInformation =>
                    {
                        if (nodeInformation.Equals(clusterNode2Info))
                        {
                            newNodeOftypeSubscriberFiredClusterNode1 = true;
                        }
                    },
                    nodeType);

            var clusterNodeRegistry2 = new NodeRegistryComponent
                (
                clusterMulticastAdapter2,
                new NodeRegistryConfig
                    (
                    clusterNode2Info.NodeType,
                    clusterNode2Info.NodeIdentifier,
                    clusterNode2Info.NodeEndpoint.IpAddress,
                    clusterNode2Info.NodeEndpoint.Port,
                    true), "cluster1");

            //subscribe for events
            clusterNodeRegistry2.SubscribeForNewNodeConnected
                (
                    nodeInformation =>
                    {
                        if (nodeInformation.Equals(clusterNode1Info))
                        {
                            newNodeSubscriberFiredClusterNode2 = true;
                        }
                    });

            clusterNodeRegistry2.SubscribeForNewNodeConnectedByType
                (
                    nodeInformation =>
                    {
                        if (nodeInformation.Equals(clusterNode1Info))
                        {
                            newNodeOftypeSubscriberFiredClusterNode2 = true;
                        }
                    },
                    nodeType);

            Thread.Sleep(2500);

            Assert.False(newNodeSubscriberFiredNonClusterNode);
            Assert.False(newNodeOftypeSubscriberFiredNonClusterNode);

            Assert.True(newNodeOftypeSubscriberFiredClusterNode2);
            Assert.True(newNodeSubscriberFiredClusterNode2);
      
						Assert.True(newNodeOftypeSubscriberFiredClusterNode1);
            Assert.True(newNodeSubscriberFiredClusterNode1);



            localNodeRegistry.ShutDownNodeRegistry();
            clusterNodeRegistry2.ShutDownNodeRegistry();
            clusterNodeRegistry1.ShutDownNodeRegistry();
        }


        [Test]
        public void GetNodesListFromNodeRegistry() {
            var localNodeInformation = _information;

            var localListenPort = _listenStartPortSeed;
            _listenStartPortSeed += 1;

            var localSendingPort = _sendingStartPortSeed;
            _sendingStartPortSeed += 1;

            var localMulticastAdapter =
                new MulticastAdapterComponent
                    (
                    new GlobalConfig("239.0.0.4", localListenPort, localSendingPort, 4),
                    new MulticastSenderConfig());

            var localNodeRegistry = new NodeRegistryComponent
                (
                localMulticastAdapter,
                new NodeRegistryConfig
                    (
                    localNodeInformation.NodeType,
                    localNodeInformation.NodeIdentifier,
                    localNodeInformation.NodeEndpoint.IpAddress,
                    localNodeInformation.NodeEndpoint.Port,
                    true), null);


            Thread.Sleep(300);

            var nodeList = localNodeRegistry.GetAllNodes();

            Assert.True(nodeList.Contains(localNodeInformation));

            localNodeRegistry.ShutDownNodeRegistry();
        }


        //test if a node does not time out while sending heartbeats
        [Test]
        public void HeartBeatTest() {
            var localNodeInformation = new TNodeInformation
                (
                NodeType.LayerContainer,
                "localNode",
                new NodeEndpoint("127.0.0.1", 41000));


            var otherNodeinfo = new TNodeInformation
                (
                NodeType.LayerContainer,
                "otherNode",
                new NodeEndpoint("127.0.0.1", 40999));

            var localListenPort = _listenStartPortSeed;
            _listenStartPortSeed += 1;

            var localSendingPort = _sendingStartPortSeed;
            _sendingStartPortSeed += 1;

            var mcastGrp = "239.0.0.5";

            var localMulticastAdapter =
                new MulticastAdapterComponent
                    (
                    new GlobalConfig(mcastGrp, localListenPort, localSendingPort, 4),
                    new MulticastSenderConfig());

            var otherMulticastAdapter =
                new MulticastAdapterComponent
                    (
                    new GlobalConfig(mcastGrp, localListenPort, localSendingPort, 4),
                    new MulticastSenderConfig());

            var timeout = 500;

            var localNodeRegistry = new NodeRegistryComponent
                (
                localMulticastAdapter,
                new NodeRegistryConfig(localNodeInformation, false, timeout), null);

            var otherNodeRegistry = new NodeRegistryComponent
                (
                otherMulticastAdapter,
                new NodeRegistryConfig(otherNodeinfo, false, timeout), null);

            Thread.Sleep(150);
            //Check if Nodes have found eachother

            Assert.True(localNodeRegistry.GetAllNodes().Contains(otherNodeinfo));
            Assert.True(otherNodeRegistry.GetAllNodes().Contains(localNodeInformation));

            //wait for timeout to expire if it was not reset.
            Thread.Sleep(timeout*10);

            //check if node is still there.
            Assert.True(localNodeRegistry.GetAllNodes().Contains(otherNodeinfo));
            Assert.True(otherNodeRegistry.GetAllNodes().Contains(localNodeInformation));


            otherNodeRegistry.ShutDownNodeRegistry();

            Thread.Sleep(timeout*4);

            Assert.True(! localNodeRegistry.GetAllNodes().Contains(otherNodeinfo));


            otherNodeRegistry.ShutDownNodeRegistry();
            localNodeRegistry.ShutDownNodeRegistry();
        }

    }

}
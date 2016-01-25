using System;
using System.Collections.Generic;
using System.Threading;
using AppSettingsManager;
using CommonTypes.DataTypes;
using CommonTypes.Types;
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
                NodeType.LayerContainer,
                "UnitTestNode",
                new NodeEndpoint("127.0.0.1", 55500)
                );
        }

        #endregion

        private TNodeInformation _information;
        private int _listenStartPortSeed = 50000;
        private int _sendingStartPortSeed = 52500;

        public void FireLeaveEventTest() {
            int localListenPort = _listenStartPortSeed;
            _listenStartPortSeed++;

            int localSendPortStartSeed = _sendingStartPortSeed;
            _sendingStartPortSeed++;

            TNodeInformation otherNodeInfo = new TNodeInformation
                (
                NodeType.LayerContainer,
                "AllOfMyHate",
                new NodeEndpoint("127.0.0.1", 34567));

            MulticastAdapterComponent localMulticastAdapter =
                new MulticastAdapterComponent
                    (
                    new GlobalConfig("239.0.0.6", localListenPort, _sendingStartPortSeed, 4),
                    new MulticastSenderConfig());

            NodeRegistryComponent localNodeRegistry = new NodeRegistryComponent
                (
                localMulticastAdapter,
                new NodeRegistryConfig
                    (
                    new TNodeInformation
                        (
                        NodeType.LayerContainer,
                        "motivationBurst",
                        new NodeEndpoint("127.0.0.1", 44567)),
                    false,
                    400));

            bool leaveEventFired = false;

            localNodeRegistry.SubscribeForNodeDisconnected
                (
                    delegate(TNodeInformation nodeInformation) {
                        leaveEventFired = true;
                        Console.WriteLine("leave event was fired");
                    },
                    otherNodeInfo);

            NodeRegistryComponent otherNodeReg = new NodeRegistryComponent
                (
                localMulticastAdapter,
                new NodeRegistryConfig(otherNodeInfo, false, 300));

            Thread.Sleep(250);

            otherNodeReg.LeaveCluster();


            Thread.Sleep(1000);

            Assert.True(leaveEventFired);

            localNodeRegistry.ShutDownNodeRegistry();
            otherNodeReg.ShutDownNodeRegistry();
        }


        /*
        [Test]
        public void NodeShutDownAndTimeOutMessage() {
            var localListenPort = _listenStartPortSeed;
            _listenStartPortSeed++;

            var localSendingPort = _sendingStartPortSeed;
            _sendingStartPortSeed++;

            var localMulticastAdapter =
                new MulticastAdapterComponent(
                    new GlobalConfig("224.55.55.77", localListenPort, localSendingPort, 4),
                    new MulticastSenderConfig());

            var localNodeReg = new NodeRegistryComponent(
                localMulticastAdapter,
                new NodeRegistryConfig(
                    new TNodeInformation(
                        NodeType.LayerContainer,
                        "allofmywhatever",
                        new NodeEndpoint("127.0.0.1", 47477)),
                    false,
                    333));

            var otherHeartBeatMock =
                new NodeRegistryHeartBeatUseCase(
                    new NodeRegistryNodeManagerUseCase(new NodeRegistryEventHandlerUseCase()),
                    new TNodeInformation(NodeType.LayerContainer, "mOck", new NodeEndpoint("127.0.0.1", 47878)),
                    300,
                    localMulticastAdapter);
        }
        */

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
                    true));
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
                    true));

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
            string localMulticastGrp = "239.0.0.3";

            TNodeInformation localNodeInfo = _information;

            NodeType nodeType = localNodeInfo.NodeType;

            TNodeInformation otherNodeinfo = new TNodeInformation
                (
                localNodeInfo.NodeType,
					"UnitTestNode",
                new NodeEndpoint("127.0.0.1", 90010));

            bool newNodeSubscriberFired = false;
            bool newNodeOftypeSubscriberFired = false;

            int localListenPort = _listenStartPortSeed;
            _listenStartPortSeed += 1;
            int localSendingPort = _sendingStartPortSeed;
            _sendingStartPortSeed += 1;

            MulticastAdapterComponent multicastAdapter =
                new MulticastAdapterComponent
                    (
                    new GlobalConfig(localMulticastGrp, localListenPort, localSendingPort, 4),
                    new MulticastSenderConfig());

			MulticastAdapterComponent otherMulticastAdapter =
				new MulticastAdapterComponent
				(
					new GlobalConfig(localMulticastGrp, localListenPort, localSendingPort++, 4),
					new MulticastSenderConfig());
			_sendingStartPortSeed += 1;

            NodeRegistryComponent localNodeRegistry = new NodeRegistryComponent
                (
                multicastAdapter,
                new NodeRegistryConfig
                    (
                    localNodeInfo.NodeType,
                    localNodeInfo.NodeIdentifier,
                    localNodeInfo.NodeEndpoint.IpAddress,
                    localNodeInfo.NodeEndpoint.Port,
                    true));

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

            NodeRegistryComponent otherNodeRegistry = new NodeRegistryComponent
                (
                otherMulticastAdapter,
                new NodeRegistryConfig
                    (
                    otherNodeinfo.NodeType,
                    otherNodeinfo.NodeIdentifier,
                    otherNodeinfo.NodeEndpoint.IpAddress,
                    otherNodeinfo.NodeEndpoint.Port,
                    true));

            Thread.Sleep(1000);

            if (!newNodeSubscriberFired || !newNodeOftypeSubscriberFired) {
                Thread.Sleep(500);
            }

            Assert.True(newNodeSubscriberFired);
            Assert.True(newNodeOftypeSubscriberFired);


            localNodeRegistry.ShutDownNodeRegistry();
        }

        [Test]
        public void GetNodesListFromNodeRegistry() {
            TNodeInformation localNodeInformation = _information;

            int localListenPort = _listenStartPortSeed;
            _listenStartPortSeed += 1;

            int localSendingPort = _sendingStartPortSeed;
            _sendingStartPortSeed += 1;

            MulticastAdapterComponent localMulticastAdapter =
                new MulticastAdapterComponent
                    (
                    new GlobalConfig("239.0.0.4", localListenPort, localSendingPort, 4),
                    new MulticastSenderConfig());

            NodeRegistryComponent localNodeRegistry = new NodeRegistryComponent
                (
                localMulticastAdapter,
                new NodeRegistryConfig
                    (
                    localNodeInformation.NodeType,
                    localNodeInformation.NodeIdentifier,
                    localNodeInformation.NodeEndpoint.IpAddress,
                    localNodeInformation.NodeEndpoint.Port,
                    true));


            Thread.Sleep(300);

            List<TNodeInformation> nodeList = localNodeRegistry.GetAllNodes();

            Assert.True(nodeList.Contains(localNodeInformation));

            localNodeRegistry.ShutDownNodeRegistry();


            localNodeRegistry = new NodeRegistryComponent
                (
                localMulticastAdapter,
                new NodeRegistryConfig
                    (
                    localNodeInformation.NodeType,
                    localNodeInformation.NodeIdentifier,
                    localNodeInformation.NodeEndpoint.IpAddress,
                    localNodeInformation.NodeEndpoint.Port,
                    false));

            Thread.Sleep(300);

            nodeList = localNodeRegistry.GetAllNodes();

            Assert.True(!nodeList.Contains(localNodeInformation));

           // localNodeRegistry.ShutDownNodeRegistry();
        }


        //test if a node does not time out while sending heartbeats
        [Test]
        public void HeartBeatTest() {
            TNodeInformation localNodeInformation = new TNodeInformation
                (
                NodeType.LayerContainer,
                "localNode",
                new NodeEndpoint("127.0.0.1", 41000));


            TNodeInformation otherNodeinfo = new TNodeInformation
                (
                NodeType.LayerContainer,
                "otherNodeInfo",
                new NodeEndpoint("127.0.0.1", 40999));

            int localListenPort = _listenStartPortSeed;
            _listenStartPortSeed += 1;

            int localSendingPort = _sendingStartPortSeed;
            _sendingStartPortSeed += 1;

            string mcastGrp = "239.0.0.5";

            MulticastAdapterComponent localMulticastAdapter =
                new MulticastAdapterComponent
                    (
                    new GlobalConfig(mcastGrp, localListenPort, localSendingPort, 4),
                    new MulticastSenderConfig());
            MulticastAdapterComponent otherMulticastAdapter =
                new MulticastAdapterComponent
                    (
                    new GlobalConfig(mcastGrp, localListenPort, localSendingPort, 4),
                    new MulticastSenderConfig());

            int timeout = 500;

            NodeRegistryComponent localNodeRegistry = new NodeRegistryComponent
                (
                localMulticastAdapter,
                new NodeRegistryConfig(localNodeInformation, false, timeout));
            NodeRegistryComponent otherNodeRegistry = new NodeRegistryComponent
                (
                otherMulticastAdapter,
                new NodeRegistryConfig(otherNodeinfo, false, timeout));

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


        //[Test]
    }

}
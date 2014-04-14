

using System;
using System.CodeDom;
using System.Runtime.InteropServices;
using System.Threading;

using CommonTypes.DataTypes;
using CommonTypes.Types;
using ConfigurationAdapter.Interface;
using ConfigurationAdapter.Interface.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface.Config;
using MulticastAdapter.Interface.Config.Types;
using NodeRegistry.Implementation;
using NodeRegistry.Interface;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using TestContext = NUnit.Framework.TestContext;

namespace NodeRegistryTest
{
    using AppSettingsManager;

    /// <summary>
    /// Summary description for NodeRegistryComponentTest
    /// </summary>
    [TestFixture]
    public class NodeRegistryComponentTest
    {

        private NodeInformationType _informationType;
        private int _listenStartPortSeed = 50000;
        private int _sendingStartPortSeed = 52500;


        public NodeRegistryComponentTest()
        {



        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [SetUp]
        public void Setup()
        {
            _informationType = new NodeInformationType(NodeType.LayerContainer, "UnitTestNode", new NodeEndpoint("127.0.0.1", 55500));
        }



        [Test]
        public void TestInitialization()
        {
            var localListenPort = _listenStartPortSeed;
            _listenStartPortSeed += 1;
            var localSendingPort = _sendingStartPortSeed;
            _sendingStartPortSeed += 1;


            var globalConfig = new GlobalConfig("224.111.11.1", localListenPort, localSendingPort, 4);


            var multicastAdapter = new MulticastAdapterComponent(globalConfig, new MulticastSenderConfig());

            //test if the NodeRegistryUseCase can be bootstrapped from a config entry
            var nr = new NodeRegistryComponent(multicastAdapter, new NodeRegistryConfig());
            Assert.True(nr != null);

            nr.ShutDownNodeRegistry();

        }


        [Test]
        public void TestJoinAndLeaveClusterLocal()
        {

            var localMulticastGrp = "224.1.11.111";

            var localListenPort = _listenStartPortSeed;
            _listenStartPortSeed += 1;
            var localSendingPort = _sendingStartPortSeed;
            _sendingStartPortSeed += 1;



            var multicastAdapter = new MulticastAdapterComponent(new GlobalConfig(localMulticastGrp, localListenPort, localSendingPort, 4), new MulticastSenderConfig());

            var localNodeInfo = _informationType;
            var localNodeRegistry = new NodeRegistryComponent(multicastAdapter, new NodeRegistryConfig(localNodeInfo.NodeType, localNodeInfo.NodeIdentifier, localNodeInfo.NodeEndpoint.IpAddress, localNodeInfo.NodeEndpoint.Port, true));
          
            //Just to make sure 
            localNodeRegistry.JoinCluster();

            Thread.Sleep(300);
            //check if this node has joined the cluster
            Assert.True(localNodeRegistry.GetAllNodes().Contains(localNodeInfo));

            localNodeRegistry.LeaveCluster();

            Assert.True(!localNodeRegistry.GetAllNodes().Contains(localNodeInfo));

            localNodeRegistry.ShutDownNodeRegistry();

        }

        [Test]
        public void TestNewNodeSubscrition()
        {

            var localMulticastGrp = "224.1.11.112";

            var localNodeInfo = _informationType;

            var nodeType = localNodeInfo.NodeType;

            var otherNodeinfo = new NodeInformationType(localNodeInfo.NodeType, "otherNodeInfo",
                new NodeEndpoint("127.0.0.1", 90010));

            var newNodeSubscriberFired = false;
            var newNodeOftypeSubscriberFired = false;

            var localListenPort = _listenStartPortSeed;
            _listenStartPortSeed += 1;
            var localSendingPort = _sendingStartPortSeed;
            _sendingStartPortSeed += 1;

            var multicastAdapter = new MulticastAdapterComponent(new GlobalConfig(localMulticastGrp, localListenPort, localSendingPort, 4), new MulticastSenderConfig());

            var localNodeRegistry = new NodeRegistryComponent(multicastAdapter, new NodeRegistryConfig(localNodeInfo.NodeType, localNodeInfo.NodeIdentifier, localNodeInfo.NodeEndpoint.IpAddress, localNodeInfo.NodeEndpoint.Port, true));

            //subscribe for events
            localNodeRegistry.SubscribeForNewNodeConnected(delegate(NodeInformationType nodeInformation)
            {
                if (nodeInformation.Equals(otherNodeinfo))
                {
                    newNodeSubscriberFired = true;
                }
            });

            localNodeRegistry.SubscribeForNewNodeConnectedByType(delegate(NodeInformationType nodeInformation)
            {
                if (nodeInformation.Equals(otherNodeinfo))
                {
                    newNodeOftypeSubscriberFired = true;
                }
            }, nodeType);

            var otherNodeRegistry = new NodeRegistryComponent(multicastAdapter, new NodeRegistryConfig(otherNodeinfo.NodeType, otherNodeinfo.NodeIdentifier, otherNodeinfo.NodeEndpoint.IpAddress, otherNodeinfo.NodeEndpoint.Port, true));

            Thread.Sleep(1000);


            Assert.True(newNodeSubscriberFired);
            Assert.True(newNodeOftypeSubscriberFired);


            localNodeRegistry.ShutDownNodeRegistry();
        }

        [Test]
        public void GetNodesListFromNodeRegistry()
        {
            var localNodeInformation = _informationType;
         
            var localListenPort = _listenStartPortSeed;
            _listenStartPortSeed += 1;

            var localSendingPort = _sendingStartPortSeed;
            _sendingStartPortSeed += 1;

            var localMulticastAdapter = new MulticastAdapterComponent(new GlobalConfig("224.2.22.222", localListenPort, localSendingPort, 4), new MulticastSenderConfig());

            var localNodeRegistry = new NodeRegistryComponent(localMulticastAdapter, new NodeRegistryConfig(localNodeInformation.NodeType, localNodeInformation.NodeIdentifier, localNodeInformation.NodeEndpoint.IpAddress, localNodeInformation.NodeEndpoint.Port, true));

            localNodeRegistry.JoinCluster();

            Thread.Sleep(300);

            var nodeList = localNodeRegistry.GetAllNodes();

            Assert.True(nodeList.Contains(localNodeInformation));

            localNodeRegistry.ShutDownNodeRegistry();

            localNodeRegistry = new NodeRegistryComponent(localMulticastAdapter, new NodeRegistryConfig(localNodeInformation.NodeType, localNodeInformation.NodeIdentifier, localNodeInformation.NodeEndpoint.IpAddress, localNodeInformation.NodeEndpoint.Port, false));
           localNodeRegistry.JoinCluster();

            Thread.Sleep(300);

            nodeList = localNodeRegistry.GetAllNodes();

            Assert.True(!nodeList.Contains(localNodeInformation));

            localNodeRegistry.ShutDownNodeRegistry();

        }


        //test if a node does not time out while sending heartbeats
        [Test]
        public void KeepNodeAliveTest() {
            
             var localNodeInformation = new NodeInformationType(NodeType.LayerContainer, "localNode", 
                    new NodeEndpoint("127.0.0.1", 41000));


            var otherNodeinfo = new NodeInformationType(NodeType.LayerContainer, "otherNodeInfo",
                new NodeEndpoint("127.0.0.1", 40999));

            var localListenPort = _listenStartPortSeed;
            _listenStartPortSeed += 1;

            var localSendingPort = _sendingStartPortSeed;
            _sendingStartPortSeed += 1;
            
            var mcastGrp = "224.3.33.3";
            
            var localMulticastAdapter = new MulticastAdapterComponent(new GlobalConfig(mcastGrp, localListenPort, localSendingPort, 4), new MulticastSenderConfig());
            var otherMulticastAdapter = new MulticastAdapterComponent(new GlobalConfig(mcastGrp, localListenPort,  localSendingPort, 4), new MulticastSenderConfig() );

            var timeout = 500;

            var localNodeRegistry = new NodeRegistryComponent(localMulticastAdapter, new NodeRegistryConfig(localNodeInformation, false, timeout));
            var otherNodeRegistry = new NodeRegistryComponent(otherMulticastAdapter, new NodeRegistryConfig(otherNodeinfo, false, timeout));

            Thread.Sleep(150);
            //Check if Nodes have found eachother
            Assert.True(localNodeRegistry.GetAllNodes().Contains(otherNodeinfo));
            Assert.True(otherNodeRegistry.GetAllNodes().Contains(localNodeInformation));

            //wait for timeout to expire if it was not reset.
            Thread.Sleep(timeout*10);

            //check if node is still there.
            Assert.True(localNodeRegistry.GetAllNodes().Contains(otherNodeinfo));
            Assert.True(otherNodeRegistry.GetAllNodes().Contains(localNodeInformation));
            
        }




    }
}

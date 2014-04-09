

using System;
using System.CodeDom;
using System.Runtime.InteropServices;
using System.Threading;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using ConfigurationAdapter.Interface.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface.Config;
using MulticastAdapter.Interface.Config.Types;
using NodeRegistry.Implementation;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using TestContext = NUnit.Framework.TestContext;

namespace NodeRegistryTest
{
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
            _informationType = new NodeInformationType(NodeType.LayerContainer, "localNodeInfo", new NodeEndpoint("127.0.0.1", 55500));
        }



        [Test]
        public void TestInitialization()
        {
            var localListenPort = _listenStartPortSeed;
            _listenStartPortSeed += 1;
            var localSendingPort = _sendingStartPortSeed;
            _sendingStartPortSeed += 1;

            var multicastAdapter = new MulticastAdapterComponent(new GeneralMulticastAdapterConfig("224.111.11.1", localListenPort, localSendingPort, IPVersionType.IPv4), new MulticastSenderConfig());

            //test if the NodeRegistryManager can be bootstrapped from a config entry
            var nr = new NodeRegistryManager(multicastAdapter);
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

            var multicastAdapter = new MulticastAdapterComponent(new GeneralMulticastAdapterConfig(localMulticastGrp, localListenPort, localSendingPort, IPVersionType.IPv4), new MulticastSenderConfig());

            var localNodeInfo = _informationType;
            var localNodeRegistry = new NodeRegistryManager(localNodeInfo, multicastAdapter);
            localNodeRegistry.GetConfig().Content.AddMySelfToActiveNodeList = true;

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

            var multicastAdapter = new MulticastAdapterComponent(new GeneralMulticastAdapterConfig(localMulticastGrp, localListenPort, localSendingPort, IPVersionType.IPv4), new MulticastSenderConfig());

            var localNodeRegistry = new NodeRegistryManager(localNodeInfo, multicastAdapter);
            localNodeRegistry.GetConfig().Content.AddMySelfToActiveNodeList = true;

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

            var otherNodeRegistry = new NodeRegistryManager(otherNodeinfo, multicastAdapter);

            Thread.Sleep(1000);


            Assert.True(newNodeSubscriberFired);
            Assert.True(newNodeOftypeSubscriberFired);


            localNodeRegistry.ShutDownNodeRegistry();
        }

        [Test]
        public void GetNodesListFromNodeRegistry() {
            var localNodeInformation = _informationType;

            var localListenPort = _listenStartPortSeed;
            _listenStartPortSeed += 1;

            var localSendingPort = _sendingStartPortSeed;
            _sendingStartPortSeed += 1;

            var localMulticastAdapter = new MulticastAdapterComponent(new GeneralMulticastAdapterConfig("224.2.22.222", localListenPort, localSendingPort, IPVersionType.IPv4), new MulticastSenderConfig());

            var localNodeRegistry = new NodeRegistryManager(localNodeInformation, localMulticastAdapter);
            localNodeRegistry.GetConfig().Content.AddMySelfToActiveNodeList = true;
            localNodeRegistry.JoinCluster();

            Thread.Sleep(300);

            var nodeList = localNodeRegistry.GetAllNodes();

            Assert.True(nodeList.Contains(localNodeInformation));

            localNodeRegistry.ShutDownNodeRegistry();
            
            localNodeRegistry = new NodeRegistryManager(localNodeInformation, localMulticastAdapter);
            localNodeRegistry.GetConfig().Content.AddMySelfToActiveNodeList = false;
            localNodeRegistry.JoinCluster();

            Thread.Sleep(300);

            nodeList = localNodeRegistry.GetAllNodes();

            Assert.True(! nodeList.Contains(localNodeInformation));

            localNodeRegistry.ShutDownNodeRegistry();
            
        }


    }
}

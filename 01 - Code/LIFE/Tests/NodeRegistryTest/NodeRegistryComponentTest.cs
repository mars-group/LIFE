

using System.Threading;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using ConfigurationAdapter.Interface.Exceptions;
using NodeRegistry.Implementation;
using NUnit.Framework;

namespace NodeRegistryTest
{
    /// <summary>
    /// Summary description for NodeRegistryComponentTest
    /// </summary>
    [TestFixture]
    public class NodeRegistryComponentTest
    {

        private NodeInformationType informationType;

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
            informationType = new NodeInformationType(NodeType.LayerContainer, "IamSoMAAAAADRightNow^2", new NodeEndpoint("127.0.0.1", 55500));
        }

        [Test]
        public void TestInitialization()
        {
            //test if the NodeRegistryManager can be bootstrapped from a config entry
            var nr = new NodeRegistryManager();
            Assert.True(nr != null);
        }

        [Test]
        public void TestJoinClusterLocal()
        {
            var localNodeInfo = informationType;
            var localNodeRegistry = new NodeRegistryManager(localNodeInfo);
            localNodeRegistry.GetConfig().Content.AddMySelfToActiveNodeList = true;
            
            //Just to make sure 
            localNodeRegistry.JoinCluster();

            Thread.Sleep(300);

            Assert.True(localNodeRegistry.GetAllNodes().Contains(localNodeInfo));
        }

        [Test]
        public void TestLeaveClusterLocal()
        {
            var localNodeInfo = informationType;
            var localNodeRegistry = new NodeRegistryManager(localNodeInfo);
            localNodeRegistry.GetConfig().Content.AddMySelfToActiveNodeList = true;

            //Just to make sure 
            localNodeRegistry.JoinCluster();
            
            Thread.Sleep(300);
            //check if this node has joined the cluster
            Assert.True(localNodeRegistry.GetAllNodes().Contains(localNodeInfo));
            
            localNodeRegistry.LeaveCluster();
            
            Assert.True(! localNodeRegistry.GetAllNodes().Contains(localNodeInfo));

        }



    }
}

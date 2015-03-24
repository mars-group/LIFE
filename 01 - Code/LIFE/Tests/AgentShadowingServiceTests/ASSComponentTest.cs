using System;
using System.Collections.Generic;
using System.Linq;
using AgentShadowingService.Implementation;
using AgentShadowingService.Interface;
using ASC.Communication.ScsServices.Service;
using NUnit.Framework;

namespace AgentShadowingServiceTests
{
    [TestFixture]
    public class ASSComponentTest {

        private IAgentShadowingService<IMockAgent,MockAgent> _serviceA;
        private IAgentShadowingService<IMockAgent, MockAgent> _serviceB;

        [SetUp]
        public void SetupTest() {
            _serviceA = new AgentShadowingServiceComponent<IMockAgent, MockAgent>(6666);
            _serviceB = new AgentShadowingServiceComponent<IMockAgent, MockAgent>(6666);
        }

        [Test]
        public void TestCommunication() {
            // create and register RealAgents in serviceA
            var agentsA = new List<MockAgent>();
            for (int i = 0; i < 1; i++) {
                agentsA.Add(new MockAgent());
            }


            _serviceA.RegisterRealAgents(agentsA.ToArray());

            // create and register RealAgents in serviceB
            var agentsB = new List<MockAgent>();
            for (int i = 0; i < 1; i++) {
                agentsB.Add(new MockAgent());
            }


            _serviceB.RegisterRealAgents(agentsB.ToArray());

            // create Shadowagents
            var shadowsOfB = _serviceA.CreateShadowAgents(agentsB.Select(a => a.ID).ToArray());
            var shadowsOfA = _serviceB.CreateShadowAgents(agentsA.Select(a => a.ID).ToArray());

            foreach (var mockAgent in shadowsOfA) {
                mockAgent.DoCrazyShit();
            }
            //shadowsOfA.ToArray()[0].DoCrazyShit();
            //Assert.Contains(shadowsOfA.ToArray()[0].DoCrazyShit(), agentsA.Select(a => a.ID).ToArray());
            //Assert.AreEqual(agentsA.ToArray()[0].ID, shadowsOfA.ToArray()[0].DoCrazyShit());

        }

    }

    internal class MockAgent : AscService, IMockAgent {

        public MockAgent(){
            ID = Guid.NewGuid();
            ServiceID = ID;
        }

        public Guid ID { get; set; }

        public Guid DoCrazyShit() {
            return ID;
        }
    }

    [AscService(Version = "0.1")]
    internal interface IMockAgent {
        Guid DoCrazyShit();
    }
}

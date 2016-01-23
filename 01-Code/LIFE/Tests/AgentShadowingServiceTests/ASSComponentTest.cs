using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AgentShadowingService.Implementation;
using AgentShadowingService.Interface;
using ASC.Communication.ScsServices.Service;
using NUnit.Framework;

namespace AgentShadowingServiceTests
{
    [TestFixture]
    public class ASSComponentTest {

        private IAgentShadowingService<IMockAgent, MockAgent> _serviceA;
        private IAgentShadowingService<IMockAgent, MockAgent> _serviceB;

        private const int AgentsPerNode = 20000;

        [SetUp]
        public void SetupTest() {
            _serviceA = new AgentShadowingServiceComponent<IMockAgent, MockAgent>();
            _serviceB = new AgentShadowingServiceComponent<IMockAgent, MockAgent>();
        }

        [Test]
        public void TestCommunication() {
            // create and register RealAgents in serviceA
            var agentsA = new List<MockAgent>();
            for (var i = 0; i < AgentsPerNode; i++) {
                agentsA.Add(new MockAgent());
            }


            _serviceA.RegisterRealAgents(agentsA.ToArray());

            // create and register RealAgents in serviceB
            var agentsB = new List<MockAgent>();
            for (var i = 0; i < AgentsPerNode; i++)
            {
                agentsB.Add(new MockAgent());
            }


            _serviceB.RegisterRealAgents(agentsB.ToArray());

            // create Shadowagents
            var shadows = new List<IMockAgent>();
            shadows.AddRange(_serviceA.ResolveAgents(agentsB.Select(a => a.ID).ToArray()));
            shadows.AddRange(_serviceB.ResolveAgents(agentsA.Select(a => a.ID).ToArray()));
            var sw = Stopwatch.StartNew();

            Parallel.ForEach(shadows, mockAgent => Assert.AreEqual(42, mockAgent.DoCrazyShit()));

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }


    }

    internal class MockAgent : AscService, IMockAgent {

        public MockAgent(){
            ID = Guid.NewGuid();
            ServiceID = ID;
        }

        public Guid ID { get; }

        public int DoCrazyShit() {
            return 42;
        }
    }

    [AscService(Version = "0.1")]
    internal interface IMockAgent {
        int DoCrazyShit();
    }
}

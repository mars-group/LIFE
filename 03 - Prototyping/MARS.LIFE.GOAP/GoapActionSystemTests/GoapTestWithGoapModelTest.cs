using CommonTypes.Interfaces;
using GoapActionSystemFactory.Implementation;
using GoapModelTest.Actions;
using NUnit.Framework;

namespace GoapActionSystemTests {
    [TestFixture]
    public class GoapTestWithGoapModelTest {
        private IActionSystem _goapActionSystem1;
        private IActionSystem _goapActionSystem2;

        private readonly IAction _actionClean = new ActionClean();
        private readonly IAction _actionGetToy = new ActionGetToy();
        private readonly IAction _actionPlay = new ActionPlay();


        private void CreateGoapActionSystems() {
            _goapActionSystem1 = GoapComponent.LoadAgentConfiguration("AgentTestConfig1", "GoapModelTest");
            _goapActionSystem2 = GoapComponent.LoadAgentConfiguration("AgentTestConfig2", "GoapModelTest");
        }

        [SetUp]
        protected void SetUp() {
            CreateGoapActionSystems();
        }

        [Test]
        public void FindAssemblyAndClassTest() {
            CreateGoapActionSystems();
        }

        [Test]
        public void FactoryTest() {
            Assert.NotNull(_goapActionSystem1);
            Assert.NotNull(_goapActionSystem2);
        }

        [Test]
        public void ReturnActionCorrectTest() {
            Assert.True((_goapActionSystem1.GetNextAction()).Equals(_actionGetToy));
            Assert.True((_goapActionSystem2.GetNextAction()).Equals(_actionPlay));
        }

        [Test]
        public void NotReturnedActionTest() {
            Assert.False((_goapActionSystem1.GetNextAction()).Equals(_actionPlay));
            Assert.False((_goapActionSystem1.GetNextAction()).Equals(_actionClean));

            Assert.False((_goapActionSystem2.GetNextAction()).Equals(_actionGetToy));
            Assert.False((_goapActionSystem2.GetNextAction()).Equals(_actionClean));
        }
    }
}
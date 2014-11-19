using System;
using System.Runtime.Serialization;
using GoapActionSystem.Implementation;
using GoapCommon.Abstract;
using GoapModelTest.Actions;
using NUnit.Framework;
using TypeSafeBlackboard;

namespace GoapTests {

    [TestFixture]
    public class GoapActionSystemComponentTest {
        #region Setup/Teardown

        [SetUp]
        protected void SetUp() {
            CreateGoapActionSystems();
        }

        #endregion

        private AbstractGoapSystem _goapActionSystem1;
        private AbstractGoapSystem _goapActionSystem2;
        private AbstractGoapSystem _goapActionSystemSearchDepth;

        private readonly AbstractGoapAction _actionClean = new ActionClean();
        private readonly AbstractGoapAction _actionClean2 = new ActionClean();
        private readonly AbstractGoapAction _actionGetToy = new ActionGetToy();
        private readonly AbstractGoapAction _actionPlay = new ActionPlay();


        private void CreateGoapActionSystems() {
            _goapActionSystem1 = GoapComponent.LoadGoapConfiguration
                ("AgentTestConfig1", "GoapModelTest", new Blackboard());
            _goapActionSystem2 = GoapComponent.LoadGoapConfiguration
                ("AgentTestConfig2", "GoapModelTest", new Blackboard());
            _goapActionSystemSearchDepth = GoapComponent.LoadGoapConfiguration
                ("AgentTestSearchDepth", "GoapModelTest", new Blackboard());
        }

        [Test]
        public void FindAssemblyAndClassTest() {
            CreateGoapActionSystems();
        }

        [Test]
        public void ComponentCreationTest() {
            Assert.NotNull(_goapActionSystem1);
            Assert.NotNull(_goapActionSystem2);
        }

        [Test]
        public void ActionEqualityTest() {
            Assert.True(_actionClean.Equals(_actionClean));
            Assert.True(_actionClean.Equals(_actionClean2));
        }

        [Test]
        public void ReturnActionCorrectTest() {

            //var nextAction1 = _goapActionSystem1.GetNextAction();
            var nextAction2 = _goapActionSystem2.GetNextAction();
            //Assert.True(nextAction1.Equals(_actionGetToy));
            Assert.True(nextAction2.Equals(_actionPlay));
            
        }
        
        [Test]
        public void NotReturnedActionTest() {
            Assert.False((_goapActionSystem1.GetNextAction()).Equals(_actionPlay));
            Assert.False((_goapActionSystem1.GetNextAction()).Equals(_actionClean));

            Assert.False((_goapActionSystem2.GetNextAction()).Equals(_actionGetToy));
            Assert.False((_goapActionSystem2.GetNextAction()).Equals(_actionClean));
        }

        [Test]
        public void RunOutOfSearchDepthLimitTest() {
            Assert.True(new SurrogateAction().Equals(_goapActionSystemSearchDepth.GetNextAction()));
        }
    }

}
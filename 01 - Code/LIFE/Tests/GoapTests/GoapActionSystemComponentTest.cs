using System;
using System.Collections.Generic;
using System.Linq;
using GoapActionSystem.Implementation;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapModelTest.Actions;
using GoapModelTest.Worldstates;
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

        private static bool IsSubset(List<WorldstateSymbol> potentiallySubSet, List<WorldstateSymbol> enclosingSet) {
            return (potentiallySubSet.Where(x => enclosingSet.Contains(x)).Count() ==
                    potentiallySubSet.Count());
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
            AbstractGoapAction nextAction1 = _goapActionSystem1.GetNextAction();
            AbstractGoapAction nextAction2 = _goapActionSystem2.GetNextAction();
            Assert.True(nextAction1.Equals(_actionGetToy));
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

        [Test]
        public void ResultingWorldstateSymbolsFromActionTest() {
            List<WorldstateSymbol> startState = new List<WorldstateSymbol> {
                new WorldstateSymbol(WorldProperties.Happy, true, typeof (Boolean))
            };

            List<WorldstateSymbol> secondState = new List<WorldstateSymbol> {
                new WorldstateSymbol(WorldProperties.Happy, false, typeof (Boolean)),
                new WorldstateSymbol(WorldProperties.HasMoney, true, typeof (Boolean))
            };

            List<WorldstateSymbol> thirdState = new List<WorldstateSymbol> {
                new WorldstateSymbol(WorldProperties.HasMoney, false, typeof (Boolean)),
                new WorldstateSymbol(WorldProperties.HasToy, true, typeof (Boolean))
            };

            Assert.True(IsSubset(_actionClean.GetResultingWorldstate(startState), secondState));
            Assert.True(IsSubset(secondState, _actionClean.GetResultingWorldstate(startState)));

            Assert.True(IsSubset(_actionGetToy.GetResultingWorldstate(secondState), thirdState));
            Assert.True(IsSubset(thirdState, _actionGetToy.GetResultingWorldstate(secondState)));
        }
    }

}
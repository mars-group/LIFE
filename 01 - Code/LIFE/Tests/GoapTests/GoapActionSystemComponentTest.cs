using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        private AbstractGoapSystem _goapActionSystemSwitchGoal;

        private AbstractGoapSystem _goapActionSystemSwitchGoalRelevany;

        private AbstractGoapSystem _goapActionSystemHighBranching3X;
        private AbstractGoapSystem _goapActionSystemHighBranching4X;
        private AbstractGoapSystem _goapActionSystemHighBranching5X;
        private AbstractGoapSystem _goapActionSystemHighBranching6X;

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

            _goapActionSystemSwitchGoal = GoapComponent.LoadGoapConfiguration
                ("AgentTestConfigSwitchGoal", "GoapModelTest", new Blackboard());
            
            _goapActionSystemSwitchGoalRelevany = GoapComponent.LoadGoapConfiguration
                ("AgentTestConfigGoalRelevancy", "GoapModelTest", new Blackboard());
            
            _goapActionSystemHighBranching3X = GoapComponent.LoadGoapConfiguration
                ("AgentTestConfigHighBranching3X", "GoapModelTest", new Blackboard());

            _goapActionSystemHighBranching4X = GoapComponent.LoadGoapConfiguration
                ("AgentTestConfigHighBranching4X", "GoapModelTest", new Blackboard());

            _goapActionSystemHighBranching5X = GoapComponent.LoadGoapConfiguration
                ("AgentTestConfigHighBranching5X", "GoapModelTest", new Blackboard());

            _goapActionSystemHighBranching6X = GoapComponent.LoadGoapConfiguration
                ("AgentTestConfigHighBranching6X", "GoapModelTest", new Blackboard());
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
            Assert.False(_actionClean.Equals(_actionGetToy));
        }

        [Test]
        public void ReturnActionCorrectTest() {
            AbstractGoapAction nextAction1A = _goapActionSystem1.GetNextAction();
            AbstractGoapAction nextAction1B = _goapActionSystem1.GetNextAction();

            Assert.True(nextAction1A.Equals(_actionGetToy));
            Assert.True(nextAction1B.Equals(_actionPlay));
            
            AbstractGoapAction nextAction2A = _goapActionSystem2.GetNextAction();
            Assert.True(nextAction2A.Equals(_actionPlay));
        }

        [Test]
        public void NotReturnedActionTest() {
            AbstractGoapAction nextAction1 = _goapActionSystem1.GetNextAction();
            AbstractGoapAction nextAction2 = _goapActionSystem1.GetNextAction();

            AbstractGoapAction nextAction3 = _goapActionSystem2.GetNextAction();
            AbstractGoapAction nextAction4 = _goapActionSystem2.GetNextAction();

            Assert.False(nextAction1.Equals(_actionPlay));
            Assert.False(nextAction2.Equals(_actionClean));

            Assert.False(nextAction3.Equals(_actionGetToy));
            Assert.False(nextAction4.Equals(_actionClean));
        }

        [Test]
        public void SwitchGoalIfFinishedTest() {

            // First goal is to get happy
            AbstractGoapAction nextAction1 = _goapActionSystemSwitchGoal.GetNextAction();
            Assert.True(nextAction1.Equals(_actionGetToy));

            AbstractGoapAction nextAction2 = _goapActionSystemSwitchGoal.GetNextAction();
            Assert.True(nextAction2.Equals(_actionPlay));

            // Happy is satisfied and new goal is get rich
            AbstractGoapAction nextAction3 = _goapActionSystemSwitchGoal.GetNextAction();
            Assert.True(nextAction3.Equals(_actionClean));

            // Get rich is satisfied and new goal is get happy
            AbstractGoapAction nextAction4 = _goapActionSystemSwitchGoal.GetNextAction();
            Assert.True(nextAction4.Equals(_actionGetToy));
        }

        [Test]
        public void SwitchGoalRelevancyTest() {
            AbstractGoapAction action1 = new ToA();
            AbstractGoapAction action2 = new AToB();
            AbstractGoapAction action3 = new BToC();

            AbstractGoapAction action4 = new ToF();
            AbstractGoapAction action5 = new FToG();
            AbstractGoapAction action6 = new GToH();

            AbstractGoapAction nextAction1 = _goapActionSystemSwitchGoalRelevany.GetNextAction();
            Assert.True(nextAction1.Equals(action1));

            AbstractGoapAction nextAction2 = _goapActionSystemSwitchGoalRelevany.GetNextAction();
            Assert.True(nextAction2.Equals(action2));

            AbstractGoapAction nextAction3 = _goapActionSystemSwitchGoalRelevany.GetNextAction();
            Assert.True(nextAction3.Equals(action3));

            AbstractGoapAction nextAction4 = _goapActionSystemSwitchGoalRelevany.GetNextAction();
            Assert.True(nextAction4.Equals(action4));

            AbstractGoapAction nextAction5 = _goapActionSystemSwitchGoalRelevany.GetNextAction();
            Assert.True(nextAction5.Equals(action5));

            AbstractGoapAction nextAction6 = _goapActionSystemSwitchGoalRelevany.GetNextAction();
            Assert.True(nextAction6.Equals(action6));

            AbstractGoapAction nextAction7 = _goapActionSystemSwitchGoalRelevany.GetNextAction();
            Assert.True(nextAction7.Equals(action1));
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

        [Test]
        public void TestHighBranching3XFindPath() {
            AbstractGoapAction chosenAction = _goapActionSystemHighBranching3X.GetNextAction();
            Assert.False(chosenAction.Equals(new SurrogateAction()));
        }

        [Test]
        public void TestHighBranching4XFindPath() {
            AbstractGoapAction chosenAction = _goapActionSystemHighBranching4X.GetNextAction();
            Assert.False(chosenAction.Equals(new SurrogateAction()));
        }

        [Test]
        public void TestHighBranching5XFindPath() {
            AbstractGoapAction chosenAction = _goapActionSystemHighBranching5X.GetNextAction();
            Assert.False(chosenAction.Equals(new SurrogateAction()));
        }

        [Test]
        public void TestHighBranching6XFindPath() {
            AbstractGoapAction chosenAction = _goapActionSystemHighBranching6X.GetNextAction();
            Assert.False(chosenAction.Equals(new SurrogateAction()));
        }
    }

}
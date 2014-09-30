using GoapCommon.Abstract;
using GoapCommon.Interfaces;
using GoapModelTest.Actions;
using GoapModelTest.Goals;
using GoapModelTest.Worldstates;
using NUnit.Framework;

namespace GoapTests {
    [TestFixture]
    internal class CustomGraphServiceComponentTest {
        private AbstractGoapAction _clean = new ActionClean();
        private AbstractGoapAction _getToy = new ActionGetToy();
        private AbstractGoapAction _play = new ActionPlay();

        private IGoapGoal _happyGoal = new GoalBeHappy();

        private IGoapWorldstate _happyTrue = new Happy(true);
        private IGoapWorldstate _happyFalse = new Happy(false);
        private IGoapWorldstate _hasMoneyTrue = new HasMoney(true);
        private IGoapWorldstate _hasMoneyFalse = new HasMoney(false);
        private IGoapWorldstate _hasToyTrue = new HasToy(true);
        private IGoapWorldstate _hasToyFalse = new HasToy(false);

        [Test]
        public void GetShortestPathSuccessfulTest() {
            
        }

        [Test]
        public void GetActualDepthFromRootTest() {
            
        }


    }
}
using System;
using System.Collections.Generic;
using GoapComponent;
using GoapComponent.GoapActionsStock;
using GoapComponent.GoapKnowledgeProcessingComponent;
using GoapComponent.GoapKnowledgeProcessingComponent.GoapWorldStatesStock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestGoap
{
    [TestClass]
    public class UnitTestGoapAction
    {
        [TestMethod]
        public void Action_IsExecutable_With_Valid_Preconditions()
        {
            var goap = new Goap();
            
            goap.KnowledgeProcessing.AddWorldState(new GoapWorldStateIsHungry(true));
            goap.KnowledgeProcessing.AddWorldState(new GoapWorldStateSunIsShining(true));

            List<GoapWorldState> status = goap.KnowledgeProcessing.AggregatedGoapWorldStates;

            GoapAction eatIce = new GoapActionEatIce(goap);

            Assert.IsTrue(eatIce.IsExecutable(status));
        }


        [TestMethod]
        public void Action_IsExecutable_With_Invalid_Preconditions()
        {
            Goap goap = new Goap();
            GoapAction eatIce = new GoapActionEatIce(goap);
            List<GoapWorldState> status = goap.KnowledgeProcessing.AggregatedGoapWorldStates;
            Assert.IsFalse(eatIce.IsExecutable(status));
            }

    }
}

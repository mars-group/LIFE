using GoapComponent.GoapKnowledgeProcessingComponent.GoapWorldStatesStock;

namespace GoapComponent.GoapActionsStock {
    internal class GoapActionEatIce : GoapAction {
        public GoapActionEatIce() {
            var preIsHungry = new GoapWorldStateIsHungry(true);
            var preSunIsShining = new GoapWorldStateSunIsShining(true);
            preconditions.Add(preIsHungry);
            preconditions.Add(preSunIsShining);

            var postIsHungry = new GoapWorldStateIsHungry(false);
            postconditions.Add(postIsHungry);
        }
    }
}
using GoapComponent.GoapKnowledgeProcessingComponent.GoapWorldStatesStock;

namespace GoapComponent.GoapActionsStock {

    class GoapActionEatIce : GoapAction {


        public GoapActionEatIce() {
            this.AddTestData();
        }
        
        private void AddTestData() {
            var preIsHungry = new GoapWorldStateIsHungry(true);
            var preSunIsShining = new GoapWorldStateSunIsShining(true);
            this.Preconditions.Add(preIsHungry);
            this.Preconditions.Add(preSunIsShining);
            
            var postIsHungry = new GoapWorldStateIsHungry(false);
            this.Postconditions.Add(postIsHungry);
        }


    }

    
}
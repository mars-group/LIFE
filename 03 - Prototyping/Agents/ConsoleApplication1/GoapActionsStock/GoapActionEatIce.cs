using GoapComponent.GoapKnowledgeProcessingComponent;
using GoapComponent.GoapKnowledgeProcessingComponent.GoapWorldStatesStock;

namespace GoapComponent.GoapActionsStock {

    class GoapActionEatIce : GoapAction {


        public GoapActionEatIce(Goap goap)
            : base(goap)
        {
            this.AddTestData();
        }
        
        private void AddTestData() {
            Preconditions.Add(new GoapWorldStateIsHungry(true));
            Preconditions.Add(new GoapWorldStateSunIsShining(true));
            
            var postIsHungry = new GoapWorldStateIsHungry(false);
            Postconditions.Add(postIsHungry);
        }


    }

    
}
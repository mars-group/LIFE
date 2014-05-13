using System.Collections.Generic;
using GoapComponent.GoapKnowledgeProcessingComponent;
using GoapComponent.GoapKnowledgeProcessingComponent.GoapWorldStatesStock;

namespace GoapComponent.GoapGoalsStock
{
    class GoapGoalBeSated :GoapGoal
    {

        internal GoapGoalBeSated() {
            this.TargetWorldState = new List<GoapWorldState> {new GoapWorldStateIsHungry(false)};
        }
    }
}

using System.Collections.Generic;
using GoapComponent.GoapKnowledgeProcessingComponent;

namespace GoapComponent {
    /// <summary>
    /// </summary>
    public class GoapGoal {
        public List<GoapWorldState> targetWorldState;


        public GoapGoal(List<GoapWorldState> targetWorldState) {
            this.targetWorldState = targetWorldState;
        }

        public bool isGoalFulfilled(List<GoapWorldState> comparingWorlState) {
            /**
             * TODO compare function neccessary - better is targetWorldState a subset of comparingWorldState
             */
            return false;
        }
    }
}
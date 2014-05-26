using System.Collections.Generic;
using System.Linq;
using GoapComponent.GoapKnowledgeProcessingComponent;

namespace GoapComponent {
    /// <summary>
    /// </summary>
    public abstract class GoapGoal {
        protected List<GoapWorldState> TargetWorldState;


        public bool IsGoalFulfilled(List<GoapWorldState> comparingWorlState) {
            return TargetWorldState.All(comparingWorlState.Contains);
        }
    }
}
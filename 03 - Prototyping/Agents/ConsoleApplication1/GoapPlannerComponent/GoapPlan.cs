using System.Collections.Generic;

namespace GoapComponent.GoapPlannerComponent {
    internal class GoapPlan {
        private Stack<GoapAction> plan;

        public GoapPlan(Stack<GoapAction> goapActions) {
            this.plan = goapActions;
        }

        public GoapAction GetNextAction() {
            if (plan != null && plan.Count > 0) return plan.Pop();

            return null;
        }
    }
}
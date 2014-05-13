using System.Collections.Generic;
using GoapComponent.GoapActionsStock;
using GoapComponent.GoapKnowledgeProcessingComponent;

namespace GoapComponent.GoapPlannerComponent {
    internal class GoapPlanner {
        private GoapPlan _currentGoapPlan;
        private Stack<GoapAction> allAvailableActions;
        private GoapKnowledgeProcessing _knowledgeProcessing;

        public GoapPlanner(GoapKnowledgeProcessing knowledgeProcessing) {
            this._knowledgeProcessing = knowledgeProcessing;
        }

        /// <summary>
        /// </summary>
        /// <returns> return the next GoapAction of the plan</returns>
        internal GoapAction GetNextAction() {
            if (_currentGoapPlan == null) CreateGoapPlan();
            if (_currentGoapPlan != null) return _currentGoapPlan.GetNextAction();
            return null;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private GoapPlan CreateGoapPlan() {
            var actionStack = new Stack<GoapAction>();
            var actionEat = new GoapActionEatIce();
            actionStack.Push(actionEat);
            var plan = new GoapPlan(actionStack);
            _currentGoapPlan = plan;
            return plan;
        }
    }
}
using System.Collections.Generic;
using GoapComponent.GoapKnowledgeProcessingComponent;
using GoapComponent.Implementation.GoapActionsStock;

namespace GoapComponent.GoapPlannerComponent {
    

    internal class GoapPlanner {
        private GoapPlan currentGoapPlan;
        private Stack<GoapAction> allAvailableActions;
        private GoapKnowledgeProcessing knowledgeProcessing;

        public GoapPlanner(GoapKnowledgeProcessing knowledgeProcessing)
        {
            this.knowledgeProcessing = knowledgeProcessing;
        }

        
        /// <summary>
        /// </summary>
        /// <returns> return the next GoapAction of the plan</returns>
        internal GoapAction GetNextAction() {
            if (currentGoapPlan == null) {
                this.CreateGoapPlan();
            }
            return currentGoapPlan.GetNextAction();
        }

        /// <summary>
        /// </summary>
        /// <param name="currentGoapWorldStates"></param>
        /// <param name="targetGoapWorldStates"></param>
        /// <returns></returns>
        private GoapPlan CreateGoapPlan() {

            Stack<GoapAction> actionStack = new Stack<GoapAction>();
            var actionEat = new GoapActionEatIce(); 
            actionStack.Push(actionEat);
            GoapPlan plan = new GoapPlan(actionStack);
            this.currentGoapPlan = plan;
            return plan;

        }
    }
}
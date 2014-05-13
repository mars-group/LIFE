using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Common.Interfaces;
using GoapComponent.GoapGoalsStock;
using GoapComponent.GoapKnowledgeProcessingComponent;
using GoapComponent.GoapPlannerComponent;

[assembly: InternalsVisibleTo("UnitTestGoap")]

namespace GoapComponent {
    /// <summary>
    ///     Is the only access point for the whole GOAP functions from outside the GOAPComponent
    /// </summary>
    public class Goap : IAgentLogic {
        internal GoapGoal CurrentGoal;
        internal List<GoapGoal> allAvailableGoals;
        private readonly GoapPlanner _planner;
        public readonly GoapKnowledgeProcessing KnowledgeProcessing;


        public Goap(IPerception perception) {
            KnowledgeProcessing = new GoapKnowledgeProcessing(perception);
            _planner = new GoapPlanner(KnowledgeProcessing);
            ChooseGoal();
        }

        /// <summary> only for testing
        /// </summary>
        public Goap() {
            KnowledgeProcessing = new GoapKnowledgeProcessing();
            _planner = new GoapPlanner(KnowledgeProcessing);
            allAvailableGoals = new List<GoapGoal> {new GoapGoalBeCool(), new GoapGoalBeSated()};
        }

        private GoapGoal ChooseGoal() {
            if (KnowledgeProcessing.IsSunShining() && KnowledgeProcessing.IsHungry())
                CurrentGoal = new GoapGoalBeSated();
            else CurrentGoal = new GoapGoalBeCool();
            return CurrentGoal;
        }

        public IInteraction Reason() {
            return _planner.GetNextAction();
        }

        public void SenseAll() {
            KnowledgeProcessing.SenseAll();
        }

        public static void Main(string[] args) {}
    }
}
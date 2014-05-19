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
        private GoapGoal _currentGoal;
        private List<GoapGoal> _allAvailableGoals;
        private readonly GoapPlanner _planner;
        public readonly GoapKnowledgeProcessing KnowledgeProcessing;

        public Goap(IPerception perception) {
            KnowledgeProcessing = new GoapKnowledgeProcessing(perception);
            _planner = new GoapPlanner(KnowledgeProcessing, this);
            ProtSetStartData();
        }

        /// <summary> 
        /// only for testing
        /// </summary>
        public Goap() {
            KnowledgeProcessing = new GoapKnowledgeProcessing();
            _planner = new GoapPlanner(KnowledgeProcessing, this);
            ProtSetStartData();
            ChooseGoal();
        }

        public GoapGoal CurrentGoal {
            get { return _currentGoal; }
        }

        public List<GoapGoal> AllAvailableGoals {
            get { return _allAvailableGoals; }
        }

        private GoapGoal ChooseGoal() {
            if (KnowledgeProcessing.IsSunShining() && KnowledgeProcessing.IsHungry())
                _currentGoal = new GoapGoalBeSated();
            else _currentGoal = new GoapGoalBeCool();
            return _currentGoal;
        }

        public IInteraction Reason() {
            
            KnowledgeProcessing.SenseAll();
            return _planner.GetNextAction();
        }

        /*private void SenseAll() {
            KnowledgeProcessing.SenseAll();
        }*/

        private void ProtSetStartData() {
            _allAvailableGoals = new List<GoapGoal> { new GoapGoalBeCool(), new GoapGoalBeSated() };
        }

        public void ExecuteAction(GoapAction goapAction) {
            if (goapAction.IsExecutable(KnowledgeProcessing.AggregatedGoapWorldStates)) {
                
            }
            
        }

        public static void Main(string[] ar) {
            
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Common.Interfaces;
using GoapComponent.GoapKnowledgeProcessingComponent;
using GoapComponent.GoapKnowledgeProcessingComponent.GoapWorldStatesStock;
using GoapComponent.GoapPlannerComponent;

namespace GoapComponent {
    /// <summary>
    ///     Is the only access point for the whole GOAP functions from outside the GOAPComponent
    /// </summary>
    public class Goap : IAgentLogic {
        private GoapGoal currentGoal;
        private List<GoapGoal> allAvailableGoals;
        private GoapPlanner planner;
        private readonly GoapKnowledgeProcessing knowledgeProcessing;

        public Goap(List<GoapWorldState> currentWorldStates, List<GoapGoal> allAvailableGoals) {
            this.allAvailableGoals = allAvailableGoals;

            knowledgeProcessing = new GoapKnowledgeProcessing();
            planner = new GoapPlanner(knowledgeProcessing);
            ChooseGoal();
        }


        private GoapGoal ChooseGoal() {
            /** every agent has one goal at each time except at he initializing phase
            * TODO choose a goal out of the list of goals: this.allAvailableGoapGoals and set the var this.currentGoal
            */
            throw new NotImplementedException();
        }

        public IInteraction Reason() {
            throw new NotImplementedException();
        }

        public void SenseAll() {
            knowledgeProcessing.SenseAll();
        }


        private static void Main(string[] args) {
            GoapWorldStateIsHungry isHungry1 = new GoapWorldStateIsHungry(true);
            GoapWorldStateIsHungry isfed1 = new GoapWorldStateIsHungry(true);
            GoapWorldStateIsHungry isfed2 = new GoapWorldStateIsHungry(true);


            var worldStates1 = new List<GoapWorldState> {isHungry1, isfed1};

            var worldStates2 = new List<GoapWorldState> {isfed2};


            Console.WriteLine(worldStates2.All(worldStates1.Contains));
            Console.ReadLine();
        }
    }
}
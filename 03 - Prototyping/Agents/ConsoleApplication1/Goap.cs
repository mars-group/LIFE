using System;
using System.Collections.Generic;
using Common.Interfaces;
using GoapComponent.GoapKnowledgeProcessingComponent;
using GoapComponent.GoapPlannerComponent;

namespace GoapComponent {
  /// <summary>
  ///   Is the only access point for the whole GOAP functions from outside the GOAPComponent
  /// </summary>
  public class Goap : IAgentLogic {
    private GoapGoal currentGoal;
    private List<GoapGoal> allAvailableGoals;
    private GoapPlanner planner;
    private readonly GoapKnowledgeProcessing knowledgeProcessing;


    public Goap(IPerception perception) {
      knowledgeProcessing = new GoapKnowledgeProcessing(perception);
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


    public static void Main(String[] args) {}
  }
}
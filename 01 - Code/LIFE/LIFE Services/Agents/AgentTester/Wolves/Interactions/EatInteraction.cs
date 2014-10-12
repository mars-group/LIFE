using System;
using GenericAgentArchitectureCommon.Interfaces;

namespace AgentTester.Wolves.Interactions {
  
  /// <summary>
  ///   This is an eating interaction, where one agent eats another.
  /// </summary>
  internal class EatInteraction : IInteraction {
    private readonly IEatInteractionSource _predator; // The source agent.
    private readonly IEatInteractionTarget _prey; // The target agent.


    /// <summary>
    ///   Create an eat interaction.
    /// </summary>
    /// <param name="predator">The source agent.</param>
    /// <param name="prey">The target agent.</param>
    public EatInteraction(IEatInteractionSource predator, IEatInteractionTarget prey) {
      _predator = predator;
      _prey = prey;
    }


    /// <summary>
    ///   Execute the action. Remove the prey agent and reward the predator.
    /// </summary>
    public void Execute() {
      _predator.IncreaseEnergy(_prey.GetFoodValue());
      _prey.RemoveAgent();
    }
  }


  internal interface IEatInteractionSource {
    void IncreaseEnergy(int points);
  }


  internal interface IEatInteractionTarget {
    int GetFoodValue();
    void RemoveAgent();
  }
}
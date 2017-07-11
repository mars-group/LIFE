using LIFE.Components.Agents.BasicAgents.Reasoning;

namespace WolvesModel.Interactions {
  
  /// <summary>
  ///   This is an eating interaction, where one agent eats another.
  /// </summary>
  public class EatInteraction : IInteraction {
    
    private readonly IEatInteractionSource _predator; // The source agent.
    private readonly IEatInteractionTarget _prey;     // The target agent.


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


  /// <summary>
  ///   Common methods required from an agent that consumes another.
  /// </summary>
  public interface IEatInteractionSource {
    void IncreaseEnergy(int points);
  }


  /// <summary>
  ///   Common methods required from an agent that could be consumed.
  /// </summary>
  public interface IEatInteractionTarget {
    int GetFoodValue();
    void RemoveAgent();
  }
}
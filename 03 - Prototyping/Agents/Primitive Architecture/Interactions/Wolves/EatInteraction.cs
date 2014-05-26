namespace Primitive_Architecture.Interactions.Wolves {
  
  /// <summary>
  ///   This is an eating interaction, where one agent eats another.
  /// </summary>
  internal class EatInteraction : Interaction {
    private readonly IEatInteractionSource _predator; // The source agent.
    private readonly IEatInteractionTarget _prey; // The target agent.


    /// <summary>
    ///   Create an eat interaction.
    /// </summary>
    /// <param name="predator">The source agent.</param>
    /// <param name="prey">The target agent.</param>
    public EatInteraction(IEatInteractionSource predator, IEatInteractionTarget prey) : base(null) {
      _predator = predator;
      _prey = prey;
    }


    //TODO These are not needed here ... concept change advised.
    public override bool CheckPreconditions() { return true; }
    public override bool CheckTrigger()       { return true; }


    /// <summary>
    ///   Execute the action. Remove the prey agent and reward the predator.
    /// </summary>
    public override void Execute() {
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
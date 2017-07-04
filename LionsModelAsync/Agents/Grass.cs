using System;
using LionsModelAsync.Environment;
using LIFE.API.Layer;
using LIFE.API.LIFECapabilities;
using LIFE.API.Results;
using LIFE.Components.Agents.BasicAgents.Agents;
using LIFE.Components.Agents.BasicAgents.Reasoning;
using LIFE.Components.Environments.GridEnvironment;
using WolvesModel.Environment;
using WolvesModel.Interactions;

namespace WolvesModel.Agents {


  /// <summary>
  ///   Grass is also represented by an agent.
  /// </summary>
  public class Grass : GridAgent<Grass>, IEatInteractionTarget, ISimResult {

    public int FoodValue { get; private set; }    // Nutrition value (energy).
    public int FoodValueMax { get; }              // Maximum food value.
    public override Grass AgentReference => this; // Concrete agent reference. 
    private readonly Random _random;              // Random number generator for unequal growing.


    /// <summary>
    ///   Create a new grass agent.
    /// </summary>
    /// <param name="layer">Layer reference needed for delegate calls.</param>
    /// <param name="regFkt">Agent registration function pointer.</param>
    /// <param name="unregFkt"> Delegate for unregistration function.</param>
    /// <param name="grid">Grid environment implementation reference.</param>
    [PublishForMappingInMars]
    public Grass(IEnvironmentLayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt,
                 IGridEnvironment<GridAgent<Grass>> grid) : base(layer, regFkt, unregFkt, grid) {
      _random = new Random(ID.GetHashCode());
      FoodValue = 2;
      FoodValueMax = 60;
      Mover.InsertIntoEnvironment(_random.Next(layer.DimensionX), _random.Next(layer.DimensionY));
    }


	  /// <summary>
	  ///   ♫ Let it grow, let it grow, let it grow ...
	  /// </summary>
	  /// <returns>Nothing yet, later an interaction to execute.</returns>
	  protected override IInteraction Reason() {
		  FoodValue += _random.Next(4);
		  if (FoodValue > FoodValueMax) FoodValue = FoodValueMax;

	    AgentData["FoodValue"] = FoodValue;
	    AgentData["FoodValueMax"] = FoodValueMax;
		  return null;
	  }


    //_____________________________________________________________________________________________
    // Implementation of interaction primitives. 


    /// <summary>
    ///   Return the food value of this agent.
    /// </summary>
    /// <returns>The food value.</returns>
    public int GetFoodValue() {
      return FoodValue;
    }


    /// <summary>
    ///   Remove this agent (as result of an eating interaction).
    /// </summary>
    public void RemoveAgent() {
      IsAlive = false;
    }
  }
}
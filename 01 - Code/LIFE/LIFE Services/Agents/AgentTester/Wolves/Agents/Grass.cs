using System;
using AgentTester.Wolves.Interactions;
using GenericAgentArchitecture.Agents;
using GenericAgentArchitectureCommon.Interfaces;
using TVector = CommonTypes.DataTypes.Vector;

namespace AgentTester.Wolves.Agents {

  internal class Grass : SpatialAgent, IAgentLogic, IEatInteractionTarget {

    private int _foodValue = 2;          // Nutrition value (energy).
    public const int FoodvalueMax = 60;  // Maximum food value.
    private readonly Random _random;     // Random number generator for unequal growing.
    private readonly Grassland _env;     // Grassland reference (for removal).

    /// <summary>
    ///   Create a new grass agent.
    /// </summary>
    /// <param name="id">The agent identifier.</param>
    /// <param name="env">Grassland reference.</param>
    /// <param name="pos">The position.</param>
    public Grass(long id, Grassland env, TVector pos) : base(id, env, pos) {
      _random = new Random(Id.GetHashCode());
      _env = env;
    }


    /// <summary>
    ///   ♫ Let it grow, let it grow, let it grow ...
    /// </summary>
    /// <returns>Nothing yet, later an interaction to execute.</returns>
    public IInteraction Reason() {
      _foodValue += _random.Next(4);
      if (_foodValue > FoodvalueMax) _foodValue = FoodvalueMax;
      return null; //TODO Put code in external IA class! 
    }


    /// <summary>
    ///   Output the grass agent's id, position and food value.
    /// </summary>
    /// <returns>Console output string.</returns>
    public override string ToString() {
      return String.Format(Id + " | Gras  | ({0,2:00},{1,2:00})  |  {2,2:0}/{3,2:00}  |     |       |         |",
        Data.Position.X, Data.Position.Y, _foodValue, FoodvalueMax);
    }


    //_____________________________________________________________________________________________
    // Implementation of interaction primitives. 


    /// <summary>
    ///   Return the food value of this agent.
    /// </summary>
    /// <returns>The food value.</returns>
    public int GetFoodValue() {
      return _foodValue;
    }


    /// <summary>
    ///   Remove this agent (as result of an eating interaction).
    /// </summary>
    public void RemoveAgent() {
      _env.RemoveAgent(this);
    }
  }
}
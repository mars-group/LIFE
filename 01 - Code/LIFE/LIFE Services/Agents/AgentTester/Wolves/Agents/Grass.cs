using System;
using CommonTypes.TransportTypes;
using AgentTester.Wolves.Interactions;
using GenericAgentArchitecture.Agents;
using GenericAgentArchitecture.Environments;
using GenericAgentArchitectureCommon.Interfaces;

namespace AgentTester.Wolves.Agents {

  /// <summary>
  ///   Grass is also represented by an agent.
  /// </summary>
  internal class Grass : SpatialAgent, IAgentLogic, IEatInteractionTarget {

    private int _foodValue = 2;           // Nutrition value (energy).
    private const int FoodvalueMax = 60;  // Maximum food value.
    private readonly Random _random;      // Random number generator for unequal growing.


    /// <summary>
    ///   Create a new grass agent.
    /// </summary>
    /// <param name="id">The agent identifier.</param>
    /// <param name="env">Environment reference.</param>
    /// <param name="pos">The position.</param>
    public Grass(long id, IEnvironment env, TVector pos) : base(id, env, pos) {
      _random = new Random(Id.GetHashCode());
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
      return String.Format("{0,3:00} | Gras  | ({1,2:00},{2,2:00})  |  {3,2:0}/{4,2:00}  |     |       |         |",
        Id, Data.Position.X, Data.Position.Y, _foodValue, FoodvalueMax);
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
      Remove();
    }
  }
}
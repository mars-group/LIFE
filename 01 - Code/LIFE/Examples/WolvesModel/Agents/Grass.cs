using System;
using DalskiAgent.Agents;
using DalskiAgent.Reasoning;
using LifeAPI.Environment;
using LifeAPI.Layer;
using WolvesModel.Interactions;

namespace WolvesModel.Agents {

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
    /// <param name="layer">Layer reference needed for delegate calls.</param>
    /// <param name="regFkt">Agent registration function pointer.</param>
    /// <param name="unregFkt"> Delegate for unregistration function.</param>
    /// <param name="env">Environment implementation reference.</param> 
    public Grass(ILayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt, IEnvironment env) :    
      base(layer, regFkt, unregFkt, env) {
      _random = new Random(base.GetHashCode());
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
        ID, GetPosition().X, GetPosition().Y, _foodValue, FoodvalueMax);
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
      IsAlive = false;
    }
  }
}
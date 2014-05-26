using System;
using Common.Interfaces;
using Primitive_Architecture.Dummies;
using Primitive_Architecture.Interactions.Wolves;

namespace Primitive_Architecture.Agents.Wolves {

  internal class Grass : Agent, IAgentLogic, IEatInteractionTarget {

    public int Foodvalue = 2;
    public const int FoodvalueMax = 60;
    private readonly Random _random;
    private readonly Grassland _environment;

    public Grass(Grassland environment, string id) : base(id) {
      Position = new Vector(-1, -1);
      _random = new Random(Id.GetHashCode() + (int) DateTime.Now.Ticks);
      _environment = environment;
    }


    /// <summary>
    ///   ♫ Let it grow, let it grow, let it grow ...
    /// </summary>
    /// <returns>Nothing yet, later an interaction to execute.</returns>
    public IInteraction Reason() {
      Foodvalue += _random.Next(4);
      if (Foodvalue > FoodvalueMax) Foodvalue = FoodvalueMax;
      return null; //TODO Put code in external IA class! 
    }


    /// <summary>
    ///   Output the grass agent's id, position and food value.
    /// </summary>
    /// <returns>Console output string.</returns>
    public override string ToString() {
      return String.Format(Id + " | Gras  | ({0,2:00},{1,2:00})  |  {2,2:0}/{3,2:00}  |     |       |         |",
        Position.X, Position.Y, Foodvalue, FoodvalueMax);
    }


    //_____________________________________________________________________________________________
    // Implementation of interaction primitives. 


    /// <summary>
    ///   Return the food value of this agent.
    /// </summary>
    /// <returns>The food value.</returns>
    public int GetFoodValue() {
      return Foodvalue;
    }


    /// <summary>
    ///   Remove this agent (as result of an eating interaction).
    /// </summary>
    public void RemoveAgent() {
      _environment.RemoveAgent(this);
    }
  }
}
using System;
using LIFE.API.Layer;
using LIFE.API.LIFECapabilities;
using LIFE.API.Results;
using LIFE.Components.Agents.BasicAgents.Agents;
using LIFE.Components.Agents.BasicAgents.Movement;
using LIFE.Components.Agents.BasicAgents.Reasoning;
using LIFE.Components.Environments.GridEnvironment;
using WolvesModel.Environment;
using WolvesModel.Interactions;

/* Ignore the 'not used' warnings, because the properties *are* used (but externally). */
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace WolvesModel.Agents {


  /// <summary>
  ///   The wolf is the predator in the wolves vs. sheep scenario. 
  ///   It exists for a single purpose: Killing sheep!
  /// </summary>
  public class Wolf : GridAgent<Wolf>, IEatInteractionSource, IEatInteractionTarget, ISimResult {
    
    public int Energy { get; private set; }            // Current energy (with initial value).
    public int EnergyMax { get; private set; }         // Maximum health.
    public int Hunger { get; private set; }            // Hunger value of this sheep.
    public string Rule { get; private set; }           // Agent behaviour rule.
    public string Targets { get; private set; }        // Agent target sightings.
    public double TargetDistance { get; private set; } // Distance to active target. (-1 if not used)
    public override Wolf AgentReference => this;       // Reference to the concrete wolf agent type.
    private readonly Random _random;                   // Random number generator for energy loss.
    private readonly IEnvironmentLayer _environment;   // Environment reference for random movement.


    /// <summary>
    ///   Create a new wolf agent.
    /// </summary>
    /// <param name="layer">Layer reference needed for delegate calls.</param>
    /// <param name="regFkt">Agent registration function pointer.</param>
    /// <param name="unregFkt"> Delegate for unregistration function.</param>
    /// <param name="grid">Grid environment implementation reference.</param>
    [PublishForMappingInMars]
    public Wolf(IEnvironmentLayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt,
                IGridEnvironment<GridAgent<Wolf>> grid) : base(layer, regFkt, unregFkt, grid) {
      _random = new Random(ID.GetHashCode());
      _environment = layer;
      Mover.InsertIntoEnvironment(_random.Next(layer.DimensionX), _random.Next(layer.DimensionY));
      Energy = 80;
      EnergyMax = 100;
      Rule = "";
      Targets = "";
      TargetDistance = -1f;
    }


    /// <summary>
    ///   The wolf reasoning logic.
    /// </summary>
    /// <returns>The interaction to execute.</returns>
    protected override IInteraction Reason() {

      // Energy substraction is made first. 
      Energy -= 1 + _random.Next(3);
      if (Energy <= 0) {
        IsAlive = false;
        return null;
      }

      // Calculate hunger percentage, read-out nearby agents and remove own agent from perception list.
      Hunger = (int) ((double) (EnergyMax - Energy)/EnergyMax*100);
      var sheep = _environment.FindSheep(X, Y, 10);
      var wolves = _environment.FindWolves(X, Y, 10);
      for (var i = 0; i < wolves.Count; i++) {
        if (wolves[i].Equals(this)) {
          wolves.RemoveAt(i);
          break;
        }
      }


      Targets = "S:" + sheep.Count + " / W:" + wolves.Count;
      IInteraction interaction;


      // The wolf is hungry and has sheep spotted.
      if (sheep.Count > 0 && Hunger > 20) {

        // Get the nearest sheep and calculate the distance towards it.
        var nearest = sheep[0];
        TargetDistance = AgentMover.CalculateDistance2D(X, Y, nearest.X, nearest.Y);
        foreach (var s in sheep) {
          var dist = AgentMover.CalculateDistance2D(X, Y, s.X, s.Y);
          if (dist < TargetDistance) {
            nearest = s;
            TargetDistance = dist;
          }
        }

        // R1: Sheep nearby: Go for the kill!
        if (TargetDistance <= 1.4143) {
          Rule = "R1 - Kill the sheep.";
          interaction = new EatInteraction(this, nearest);
        }

        // R2: Move to the sheep. You better watch out ...
        else {
          Rule = "R2 - Moving towards sheep ("+nearest.X+","+nearest.Y+").";
          interaction = Mover.MoveTowardsTarget(nearest.X, nearest.Y, 2);
        }
      }


      // Perform random movement.
      else {
        TargetDistance = -1f;
        Rule = "R3 - No target: Random movement.";
        var x = _random.Next(_environment.DimensionX);
        var y = _random.Next(_environment.DimensionY);
        interaction = Mover.MoveTowardsTarget(x, y);
      }


      // Write the properties to the result structure.
      AgentData["Energy"] = Energy;
      AgentData["EnergyMax"] = EnergyMax;
      AgentData["Hunger"] = Hunger;
      AgentData["Rule"] = Rule;
      AgentData["Targets"] = Targets;
      AgentData["TargetDistance"] = TargetDistance;
      //Console.WriteLine(this);

      return interaction;  // End of reasoning.
    }



    //_____________________________________________________________________________________________
    // Implementation of interaction primitives. 


    /// <summary>
    ///   Increase the hitpoints of this agent. 
    /// </summary>
    /// <param name="points">The points to add.</param>
    public void IncreaseEnergy(int points) {
      Energy += points;
      if (Energy > EnergyMax) Energy = EnergyMax;
    }


    /// <summary>
    ///   Return the food value of this agent.
    /// </summary>
    /// <returns>The food value.</returns>
    public int GetFoodValue() {
      return Energy;
    }


    /// <summary>
    ///   Remove this agent (as result of an eating interaction).
    /// </summary>
    public void RemoveAgent() {
      IsAlive = false;
    }


    /// <summary>
    ///   Output this agent's data into a string.
    /// </summary>
    /// <returns>Formatted agent output.</returns>
    public override string ToString() {
      return $"Wolf  {ID}: ({X:D2},{Y:D2}) |{Hunger,3}% | {Rule}";
    }
  }
}
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

  public enum Sex { Male, Female }


  /// <summary>
  ///   The poor prey animal in the wolves vs. sheep scenario. 
  ///   But hey, at least it can kill grass agents!
  /// </summary>
  public class Sheep : GridAgent<Sheep>, IEatInteractionTarget, IEatInteractionSource, ISimResult {

    public Sex Sex { get; private set; }               // The sex of this animal.
    public int Energy { get; private set; }            // Current energy (with initial value).
    public int EnergyMax { get; private set; }         // Maximum health.
    public int Hunger { get; private set; }            // Hunger value of this sheep.
    public string Rule { get; private set; }           // Agent behaviour rule.
    public string Targets { get; private set; }        // Agent target sightings.
    public double TargetDistance { get; private set; } // Distance to active target. (-1 if not used)
    public override Sheep AgentReference => this;      // Concrete agent reference. 
    private readonly Random _random;                   // Random number generator for energy loss.
    private readonly IEnvironmentLayer _environment;   // Environment reference for random movement.


    /// <summary>
    ///   Create a new sheep agent.
    /// </summary>
    /// <param name="layer">Layer reference needed for delegate calls.</param>
    /// <param name="regFkt">Agent registration function pointer.</param>
    /// <param name="unregFkt"> Delegate for unregistration function.</param>
    /// <param name="grid">Grid environment implementation reference.</param>
    /// <param name="sex">The sex of this animal.</param>
    // ReSharper disable once SuggestBaseTypeForParameter
    [PublishForMappingInMars]
    public Sheep(IEnvironmentLayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt,
                 IGridEnvironment<GridAgent<Sheep>> grid, Sex sex) : base(layer, regFkt, unregFkt, grid) {
      _random = new Random(ID.GetHashCode());
      _environment = layer;
      Mover.InsertIntoEnvironment(_random.Next(layer.DimensionX), _random.Next(layer.DimensionY));
      Energy = 50;
      EnergyMax = 80;
      Hunger = 0;
      Sex = sex;
      Rule = "";
      Targets = "";
      TargetDistance = -1f;
    }


    /// <summary>
    ///   The sheep's brain.
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
      var grass = _environment.FindGrass(X, Y, 10);
      var sheep = _environment.FindSheep(X, Y, 10);
      for (var i = 0; i < sheep.Count; i++) {
        if (sheep[i].ID.Equals(ID)) {
          sheep.RemoveAt(i);
          break;
        }
      }


      Targets = "G:" + grass.Count + " / S:" + sheep.Count;
      IInteraction interaction;


      // If grass exists and the sheep is hungry at all.
      if (grass.Count > 0 && Hunger > 20) {

        // Get the nearest grass agent and calculate the distance towards it.
        var nearest = grass[0];
        TargetDistance = AgentMover.CalculateDistance2D(X, Y, nearest.X, nearest.Y);
        foreach (var g in grass) {
          var dist = AgentMover.CalculateDistance2D(X, Y, g.X, g.Y);
          if (dist < TargetDistance) {
            nearest = g;
            TargetDistance = dist;
          }
        }

        // R1: Eat nearby grass.
        if (TargetDistance <= 1.4143) {
          Rule = "R1 - Eat nearby grass.";
          interaction = new EatInteraction(this, nearest);
        }

        // R2: Grass exists, but not on adjoining cells: We have to move.
        else {
          Rule = "R2 - Moving towards grass ("+nearest.X+","+nearest.Y+").";
          interaction = Mover.MoveTowardsTarget(nearest.X, nearest.Y);
        }
      }


      // Either the grass is too far away or the sheep is not hungry.
      // Wander randomly around.
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
      AgentData["Sex"] = Sex.ToString();
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
      return $"Sheep {ID}: ({X:D2},{Y:D2}) |{Hunger,3}% | {Rule}";
    }
  }
}
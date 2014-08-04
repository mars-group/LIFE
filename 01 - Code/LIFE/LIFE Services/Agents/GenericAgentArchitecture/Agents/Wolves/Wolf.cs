using System;
using System.Linq;
using GenericAgentArchitecture.Dummies;
using GenericAgentArchitecture.Interactions.Wolves;
using GenericAgentArchitecture.Interfaces;
using GenericAgentArchitecture.Perception;
using GenericAgentArchitecture.Reasoning;

namespace GenericAgentArchitecture.Agents.Wolves {

  internal class Wolf : Agent, IAgentLogic, IEatInteractionSource {
    private int _energy = 80;
    private const int EnergyMax = 100;
    private readonly Random _random;
    private readonly Grassland _environment;
    private string _states;

    public Wolf(Grassland environment, string id) : base(id) {
      Position = new Vector(-1, -1); // We just need an object (coords set by env).
      _random = new Random(Id.GetHashCode() + (int) DateTime.Now.Ticks);
      _environment = environment;
      PerceptionUnit.AddSensor(new AgentSensor(_environment, new RadialHalo(Position, 8)));
    }


    public IInteraction Reason() {
      
      // Energy substraction is made first. 
      _energy -= 1 + _random.Next(3);
      if (_energy <= 0) {
        _environment.RemoveAgent(this);
        return null;
      }


      // Calculate hunger percentage, read-out nearby agents.
      var hunger = (int)(((double)(EnergyMax - _energy)/EnergyMax)*100);
      var agents = PerceptionUnit.GetData<GenericSensorInput>().Values.Values;
      var sheeps = agents.OfType<Sheep>().ToList();
      var wolves = agents.OfType<Wolf>().ToList();
      
      // Create status output.
      _states = String.Format("{0,3:00}% |", hunger) + " "+
        "- " +
        (sheeps.Count<10? sheeps.Count+"" : "+") + " " + 
        (wolves.Count<10? wolves.Count+"" : "+") + " │ ";

      if (sheeps.Count > 0) {

        // Get the nearest sheep and calculate the distance towards it.
        var sheep = CommonRCF.GetNearestAgent(sheeps, Position);
        var distance = Position.GetDistance(sheep.Position);
        _states += String.Format("E: {0,4:0.00} | ", distance);

        // R1: If there is a sheep directly ahead and hunger > 20%, eat it!
        if (distance <= 1 && hunger >= 20) {
          _states += "R1";
          return new EatInteraction(this, sheep);
        }

        // R2: Sheep at distance max. 5 and hunger > 40%? Move towards it!
        if (distance <= 5 && hunger > 40) {
          _states += "R2";
          return CommonRCF.MoveTowardsPosition(_environment, this, sheep.Position);
        }

        // R3: Very hungry wolf. You better watch out ...
        if (hunger > 60) {
          _states += "R3";
          return CommonRCF.MoveTowardsPosition(_environment, this, sheep.Position);
        }
      }

      // Just for distance output appendix (in case there was no target).
      if (sheeps.Count == 0) _states += "        | ";      

      // R4: Perform random movement.
      _states += "R4";
      return CommonRCF.GetRandomMoveInteraction(_environment, this);
    }


    /// <summary>
    ///   Print the wolf ID, position and its internal states.
    /// </summary>
    /// <returns>Console output string.</returns>
    public override string ToString() {
      return String.Format(Id + " | Wolf  | ({0,2:00},{1,2:00})  | {2,3:0}/{3,3:0} |" + _states,
        Position.X, Position.Y, _energy, EnergyMax);
    }


    //_____________________________________________________________________________________________
    // Implementation of interaction primitives. 


    /// <summary>
    ///   Increase the hitpoints of this agent. 
    /// </summary>
    /// <param name="points">The points to add.</param>
    public void IncreaseEnergy(int points) {
      _energy += points;
      if (_energy > EnergyMax) _energy = EnergyMax;
    }
  }
}
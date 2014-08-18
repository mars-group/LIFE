using System;
using System.Linq;
using AgentTester.Wolves.Interactions;
using AgentTester.Wolves.Reasoning;
using GenericAgentArchitecture.Agents;
using GenericAgentArchitecture.Dummies;
using GenericAgentArchitecture.Interfaces;
using GenericAgentArchitecture.Perception;

namespace AgentTester.Wolves.Agents {
  class Sheep : Agent, IAgentLogic, IEatInteractionTarget, IEatInteractionSource {

    private int _energy = 50;
    private const int EnergyMax = 80;
    private readonly Random _random;
    private readonly Grassland _environment;
    private string _states;


    public Sheep(Grassland environment, string id) : base(id) {
      Position = new Position(-1, -1, 0, 0); // We just need an object (coords set by env).
      _random = new Random(Id.GetHashCode() + (int)DateTime.Now.Ticks);
      _environment = environment;
      PerceptionUnit.AddSensor(new AgentSensor(_environment, new RadialHalo(Position, 8)));
    }


    public IInteraction Reason() {

      // Energy substraction is made first. 
      _energy -= 1+_random.Next(3);
      if (_energy <= 0) {
        _environment.RemoveAgent(this);
        return null;
      }

      // Calculate hunger percentage, read-out nearby agents.
      var hunger = (int)(((double)(EnergyMax - _energy)/EnergyMax)*100);
      var agents = PerceptionUnit.GetData<GenericSensorInput>().Values.Values;
      var grass  = agents.OfType<Grass>().ToList();
      var sheeps = agents.OfType<Sheep>().ToList();
      var wolves = agents.OfType<Wolf>().ToList();

      // Create status output.
      _states = String.Format("{0,3:00}% |", hunger) + " "+
        (grass.Count <10? grass.Count +"" : "+") + " " +
        (sheeps.Count<10? sheeps.Count+"" : "+") + " " + 
        (wolves.Count<10? wolves.Count+"" : "+") + " │ ";

      if (grass.Count > 0) {

        // Get the nearest sheep and calculate the distance towards it.
        var nGrass = CommonRCF.GetNearestAgent(grass, Position);
        var nSheep = CommonRCF.GetNearestAgent(sheeps, Position);
        var nWolf  = CommonRCF.GetNearestAgent(wolves, Position);          
        var dGrass = Position.GetDistance(nGrass.Position);  
        //var dWolf  = Position.GetDistance(nWolf.Position);
        _states += String.Format("E: {0,4:0.00} | ", dGrass);

        // R1: Eat nearby grass.
        if (dGrass <= 1 && hunger > 20) {
          _states += "R1";
          return new EatInteraction(this, nGrass);
        }

        // R2: Medium grass distance allowed.
        if (dGrass <= 5 && hunger > 40) {
          _states += "R2";
          return CommonRCF.MoveTowardsPosition(_environment, this, nGrass.Position.Center);
        }

        // R3: Move to the nearest grass, no matter the distance.
        if (hunger > 60) {
          _states += "R3";
          return CommonRCF.MoveTowardsPosition(_environment, this, nGrass.Position.Center);
        }
      }

      // Just for distance output appendix (in case there was no target).
      if (grass.Count == 0) _states += "        | "; 

      // R4: Perform random movement.
      _states += "R4";
      return CommonRCF.GetRandomMoveInteraction(_environment, this);
    }


    /// <summary>
    ///   Print this sheep's ID, position and its internal states.
    /// </summary>
    /// <returns>Console output string.</returns>
    public override string ToString () {
      return String.Format(Id+" | Schaf | ({0,2:00},{1,2:00})  |  {2,2:0}/{3,2:00}  |"+_states,
               Position.Center.X, Position.Center.Y, _energy, EnergyMax);
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


    /// <summary>
    ///   Return the food value of this agent.
    /// </summary>
    /// <returns>The food value.</returns>
    public int GetFoodValue() {
      return _energy;
    }


    /// <summary>
    ///   Remove this agent (as result of an eating interaction).
    /// </summary>
    public void RemoveAgent() {
      _environment.RemoveAgent(this);
    }
  }
}

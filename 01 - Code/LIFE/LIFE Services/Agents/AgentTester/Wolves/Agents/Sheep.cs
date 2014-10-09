using System;
using System.Collections.Generic;
using System.Linq;
using AgentTester.Wolves.Interactions;
using GenericAgentArchitecture.Agents;
using GenericAgentArchitecture.Environments;
using GenericAgentArchitecture.Movement;
using GenericAgentArchitecture.Movement.Movers;
using GenericAgentArchitecture.Perception;
using GenericAgentArchitectureCommon.Interfaces;
using TVector = CommonTypes.DataTypes.Vector;


namespace AgentTester.Wolves.Agents {
  
  internal class Sheep : SpatialAgent, IAgentLogic, IEatInteractionTarget, IEatInteractionSource {
    
    private const int EnergyMax = 80;
    private readonly Random _random;
    private readonly IEnvironment _environment;
    private int _energy = 50;
    private string _states;
    private readonly GridMover _mover;


    /// <summary>
    ///   Create a new sheep agent.
    /// </summary>
    /// <param name="id">The agent identifier.</param>
    /// <param name="env">Environment reference.</param>
    /// <param name="pos">The initial position.</param>
    public Sheep(long id, IEnvironment env, TVector pos) : base(id, env, pos) {
      _random = new Random(Id.GetHashCode() + (int) DateTime.Now.Ticks);
      _environment = env;
      
      // Add perception sensor.
      PerceptionUnit.AddSensor(new DataSensor(
        this, env,
        (int) Grassland.InformationTypes.Agents,
        new RadialHalo(Data.Position, 8))
      );

      // Add movement module.
      Mover = new GridMover(env, this, Data);
      _mover = (GridMover) Mover;  // Re-declaration to save casts.
    }


    public IInteraction Reason() {
      // Energy substraction is made first. 
      _energy -= 1 + _random.Next(3);
      if (_energy <= 0) {
        _environment.RemoveAgent(this);
        return null;
      }

      // Calculate hunger percentage, read-out nearby agents.
      int hunger = (int) (((double) (EnergyMax - _energy)/EnergyMax)*100);
      var rawData = PerceptionUnit.GetData((int) Grassland.InformationTypes.Agents).Data;
      var agents = ((Dictionary<long, SpatialAgent>) rawData).Values;
      var grass = agents.OfType<Grass>().ToList();
      var sheeps = agents.OfType<Sheep>().ToList();
      var wolves = agents.OfType<Wolf>().ToList();

      // Create status output.
      _states = String.Format("{0,3:00}% |", hunger) + " " +
                (grass.Count < 10 ? grass.Count + "" : "+") + " " +
                (sheeps.Count < 10 ? sheeps.Count + "" : "+") + " " +
                (wolves.Count < 10 ? wolves.Count + "" : "+") + " │ ";

      if (grass.Count > 0) {

        // Get the nearest grass agent and calculate the distance towards it.
        var grs = grass[0];
        var dist = Data.Position.GetDistance(grs.GetPosition());
        foreach (var g in grass) {
          if (Data.Position.GetDistance(g.GetPosition()) < dist) {
            grs = g;
            dist = Data.Position.GetDistance(grs.GetPosition());
          }
        }
        _states += String.Format("E: {0,4:0.00} | ", dist);

        // R1: Eat nearby grass.
        if (dist <= 1 && hunger > 20) {
          _states += "R1";
          return new EatInteraction(this, grs);
        }

        // R2: Medium grass distance allowed.
        if (dist <= 5 && hunger > 40) {
          _states += "R2";
          var options = _mover.GetMovementOptions(grs.GetPosition());
          return options.Count == 0 ? null : _mover.MoveInDirection(options[0].Direction);
        }

        // R3: Move to the nearest grass, no matter the distance.
        if (hunger > 60) {
          _states += "R3";
          var options = _mover.GetMovementOptions(grs.GetPosition());
          return options.Count == 0 ? null : _mover.MoveInDirection(options[0].Direction);
        }
      }

      // Just for distance output appendix (in case there was no target).
      if (grass.Count == 0) _states += "        | ";

      // R4: Perform random movement.
      _states += "R4";
      if (_environment is Environment2D) {
        var pos = ((Environment2D) _environment).GetRandomPosition();
        var options = _mover.GetMovementOptions(new Vector(pos.X, pos.Y, pos.Z));
        return options.Count == 0 ? null : _mover.MoveInDirection(options[0].Direction);
      }
      
      //TODO Build something for ESC case.  
      return null;
    }


    /// <summary>
    ///   Print this sheep's ID, position and its internal states.
    /// </summary>
    /// <returns>Console output string.</returns>
    public override string ToString() {
      return String.Format("{0,3:00} | Schaf | ({1,2:00},{2,2:00})  |  {3,2:0}/{4,2:00}  |" + _states,
          Id, Data.Position.X, Data.Position.Y, _energy, EnergyMax);
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
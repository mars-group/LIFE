using System;
using System.Collections.Generic;
using System.Linq;
using AgentTester.Wolves.Interactions;
using DalskiAgent.Agents;
using DalskiAgent.Auxiliary;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using DalskiAgent.Movement;
using DalskiAgent.Movement.Movers;
using DalskiAgent.Perception;
using GenericAgentArchitectureCommon.Interfaces;

namespace AgentTester.Wolves.Agents {
 
  /// <summary>
  ///   The wolf is the predator in the wolves vs. sheeps scenario. 
  ///   It exists for a single purpose: Killing sheeps!
  /// </summary>
  internal class Wolf : SpatialAgent, IAgentLogic, IEatInteractionSource {
    
    private int _energy = 80;                    // Current energy (with initial value).
    private const int EnergyMax = 100;           // Maximum health.
    private readonly Random _random;             // Random number generator for energy loss.
    private readonly IEnvironment _environment;  // Environment reference for random movement.
    private string _states;                      // Output string for console.
    private readonly GridMover _mover;           // Specific agent mover reference (to avoid casts).


    /// <summary>
    ///   Create a new wolf agent.
    /// </summary>
    /// <param name="exec">Agent execution container reference.</param>
    /// <param name="env">Environment reference.</param>
    /// <param name="pos">The initial position.</param>
    public Wolf(IExecution exec, IEnvironment env, Vector pos = null) : base(exec, env, pos) {
      _random = new Random(Id.GetHashCode() + (int) DateTime.Now.Ticks);
      _environment = env;
      
      // Add perception sensor.
      PerceptionUnit.AddSensor(new DataSensor(
        this, env,
        (int) InformationTypes.AllAgents,
        new RadialHalo(Data.Position, 8))
      );

      // Add movement module.
      Mover = new GridMover(env, this, Data);
      _mover = (GridMover) Mover;  // Re-declaration to save casts.
      Init();
    }


    /// <summary>
    ///   The wolf reasoning logic.
    /// </summary>
    /// <returns>The interaction to execute.</returns>
    public IInteraction Reason() {
      
      // Energy substraction is made first. 
      _energy -= 1 + _random.Next(3);
      if (_energy <= 0) {
        ConsoleView.AddMessage("["+GetTick()+"] Wolf "+Id+" ist verhungert!", ConsoleColor.DarkRed);
        IsAlive = false;
        return null;
      }


      // Calculate hunger percentage, read-out nearby agents.
      var hunger = (int)(((double)(EnergyMax - _energy)/EnergyMax)*100);
      var rawData = PerceptionUnit.GetData((int)InformationTypes.AllAgents).Data;
      var agents = ((List<SpatialAgent>) rawData);
      var sheeps = agents.OfType<Sheep>().ToList();
      var wolves = agents.OfType<Wolf>().ToList();
      
      // Create status output.
      _states = String.Format("{0,3:00}% |", hunger) + " - " +
        (sheeps.Count<10? sheeps.Count+"" : "+") + " " + 
        (wolves.Count<10? wolves.Count+"" : "+") + " │ ";

      if (sheeps.Count > 0) {

        // Get the nearest sheep and calculate the distance towards it.
        var sheep = sheeps[0];
        var dist = Data.Position.GetDistance(sheep.GetPosition());
        foreach (var shp in sheeps) {
          if (Data.Position.GetDistance(shp.GetPosition()) < dist) {
            sheep = shp;
            dist = Data.Position.GetDistance(sheep.GetPosition());
          }
        }
        _states += String.Format("E: {0,4:0.00} | ", dist);

        // R1: If there is a sheep directly ahead and hunger > 20%, eat it!
        if (dist <= 1 && hunger >= 20) {
          _states += "R1";
          ConsoleView.AddMessage("["+GetTick()+"] Wolf "+Id+" frißt Schaf "+sheep.Id+"!", ConsoleColor.Blue);
          return new EatInteraction(this, sheep);
        }

        // R2: Sheep at distance max. 5 and hunger > 40%? Move towards it!
        if (dist <= 5 && hunger > 40) {
          _states += "R2";
          var options = _mover.GetMovementOptions(sheep.GetPosition());
          return options.Count == 0 ? null : _mover.MoveInDirection(options[0].Direction);
        }

        // R3: Very hungry wolf. You better watch out ...
        if (hunger > 60) {
          _states += "R3";
          var options = _mover.GetMovementOptions(sheep.GetPosition());
          return options.Count == 0 ? null : _mover.MoveInDirection(options[0].Direction);
        }
      }

      // Just for distance output appendix (in case there was no target).
      if (sheeps.Count == 0) _states += "        | ";      

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
    ///   Print the wolf ID, position and its internal states.
    /// </summary>
    /// <returns>Console output string.</returns>
    public override string ToString() {
      return String.Format("{0,3:00} | Wolf  | ({1,2:00},{2,2:00})  | {3,3:0}/{4,3:0} |" + _states,
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
  }
}
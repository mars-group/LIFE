using System;
using System.Linq;
using DalskiAgent.Agents;
using DalskiAgent.Movement.Movers;
using DalskiAgent.Reasoning;
using LifeAPI.Layer;
using SpatialAPI.Entities.Transformation;
using SpatialAPI.Environment;
using WolvesModel.Interactions;

namespace WolvesModel.Agents {
 
  /// <summary>
  ///   The wolf is the predator in the wolves vs. sheeps scenario. 
  ///   It exists for a single purpose: Killing sheeps!
  /// </summary>
  internal class Wolf : SpatialAgent, IAgentLogic, IEatInteractionSource {
    
    private int _energy = 80;                   // Current energy (with initial value).
    private const int EnergyMax = 100;          // Maximum health.
    private readonly Random _random;            // Random number generator for energy loss.
    private readonly IEnvironment _environment; // Environment reference for random movement.
    private string _states;                     // Output string for console.
    private readonly GridMover _mover;          // Specific agent mover reference (to avoid casts).


    /// <summary>
    ///   Create a new wolf agent.
    /// </summary>
    /// <param name="layer">Layer reference needed for delegate calls.</param>
    /// <param name="regFkt">Agent registration function pointer.</param>
    /// <param name="unregFkt"> Delegate for unregistration function.</param>
    /// <param name="env">Environment implementation reference.</param> 
    public Wolf(ILayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt, IEnvironment env) : 
      base(layer, regFkt, unregFkt, env) {
      _random = new Random(base.GetHashCode());
      _environment = env;

      // Add movement module.
      Mover = new GridMover(env, this);
      _mover = (GridMover) Mover;  // Re-declaration to save casts.
    }


    /// <summary>
    ///   The wolf reasoning logic.
    /// </summary>
    /// <returns>The interaction to execute.</returns>
    public IInteraction Reason() {
      
      // Energy substraction is made first. 
      _energy -= 1 + _random.Next(3);
      if (_energy <= 0) {
        PrintMessage("["+GetTick()+"] Wolf "+Id+" ist verhungert!", ConsoleColor.DarkRed);
        IsAlive = false;
        return null;
      }

      // Calculate hunger percentage, read-out nearby agents and remove own agent from perception list.
      var hunger = (int) (((double) (EnergyMax - _energy)/EnergyMax)*100);
      var agents = _environment.ExploreAll().ToList();
      agents.Remove(SpatialEntity);

      // Differentiate between perceived agent types. 
      var sheeps = agents.OfType<Sheep>().ToList();
      var wolves = agents.OfType<Wolf>().ToList();
  
      // Create status output.
      _states = String.Format("{0,3:00}% |", hunger) + " - " +
        (sheeps.Count<10? sheeps.Count+"" : "+") + " " + 
        (wolves.Count<10? wolves.Count+"" : "+") + " │ ";

      if (sheeps.Count > 0) {

        // Get the nearest sheep and calculate the distance towards it.
        var nearest = sheeps[0];
        var dist = GetPosition().GetDistance(nearest.GetPosition());
        foreach (var sheep in sheeps) {
          if (GetPosition().GetDistance(sheep.GetPosition()) < dist) {
            nearest = sheep;
            dist = GetPosition().GetDistance(sheep.GetPosition());
          }
        }
        _states += String.Format("E: {0,4:0.00} | ", dist);

        // R1: If there is a sheep directly ahead and hunger > 20%, eat it!
        if (dist <= 1 && hunger >= 20) {
          _states += "R1";
          PrintMessage("["+GetTick()+"] Wolf "+Id+" frißt Schaf "+nearest.Id+"!", ConsoleColor.Blue);
          return new EatInteraction(this, nearest);
        }

        // R2: Sheep at distance max. 5 and hunger > 40%? Move towards it!
        if (dist <= 5 && hunger > 40) {
          _states += "R2";
          var options = _mover.GetMovementOptions(nearest.GetPosition());
          return options.Count == 0 ? null : _mover.MoveInDirection(options[0].Direction);
        }

        // R3: Very hungry wolf. You better watch out ...
        if (hunger > 60) {
          _states += "R3";
          var options = _mover.GetMovementOptions(nearest.GetPosition());
          return options.Count == 0 ? null : _mover.MoveInDirection(options[0].Direction);
        }
      }

      // Just for distance output appendix (in case there was no target).
      if (sheeps.Count == 0) _states += "        | "; 

      // R4: Perform random movement.
      _states += "R4";
      var x = _random.Next((int)_environment.MaxDimension.X);
      var y = _random.Next((int)_environment.MaxDimension.Y);
      var rndOpts = _mover.GetMovementOptions(new Vector3(x, y));
      return rndOpts.Count == 0 ? null : _mover.MoveInDirection(rndOpts[0].Direction);
    }


    /// <summary>
    ///   Print the wolf ID, position and its internal states.
    /// </summary>
    /// <returns>Console output string.</returns>
    public override string ToString() {
      return String.Format("{0,3:00} | Wolf  | ({1,2:00},{2,2:00})  | {3,3:0}/{4,3:0} |" + _states,
        Id, GetPosition().X, GetPosition().Y, _energy, EnergyMax);
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
using System;
using System.Collections.Generic;
using System.Linq;
using DalskiAgent.Agents;
using DalskiAgent.Movement.Movers;
using DalskiAgent.Reasoning;
using LifeAPI.Agent;
using LifeAPI.Layer;
using SpatialAPI.Entities.Transformation;
using SpatialAPI.Environment;
using WolvesModel.Interactions;

namespace WolvesModel.Agents {
  
  /// <summary>
  ///   The poor prey animal in the wolves vs. sheeps scenario. 
  ///   But hey, at least it can kill grass agent!
  /// </summary>
  internal class Sheep : SpatialAgent, IAgentLogic, IEatInteractionTarget, IEatInteractionSource {
    
    private int _energy = 50;                   // Current energy (with initial value).
    private const int EnergyMax = 80;           // Maximum health.
    private readonly Random _random;            // Random number generator for energy loss.
    private readonly IEnvironment _environment; // Environment reference for random movement.   
    private string _states;                     // Output string for console.
    private readonly GridMover _mover;          // Specific agent mover reference (to avoid casts).
    private readonly WolvesLayer _layer;        // WolvesLayer reference.


    /// <summary>
    ///   Create a new sheep agent.
    /// </summary>
    /// <param name="layer">Layer reference needed for delegate calls.</param>
    /// <param name="regFkt">Agent registration function pointer.</param>
    /// <param name="unregFkt"> Delegate for unregistration function.</param>
    /// <param name="env">Environment implementation reference.</param> 
    public Sheep(WolvesLayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt, IEnvironment env) : 
      base(layer, regFkt, unregFkt, env, Guid.NewGuid()) {
      _random = new Random(base.GetHashCode());
      _environment = env;
      layer.Agents[ID] = this;
      _layer = layer;

      // Add movement module.
      Mover = new GridMover(env, this);
      _mover = (GridMover) Mover;  // Re-declaration to save casts.
    }


    /// <summary>
    ///   The sheeps brain.
    /// </summary>
    /// <returns>The interaction to execute.</returns>
    public IInteraction Reason() {

      // Energy substraction is made first. 
      _energy -= 1 + _random.Next(3);
      if (_energy <= 0) {
        PrintMessage("["+GetTick()+"] Schaf "+AgentNumber+" ist verhungert!", ConsoleColor.DarkRed);
        IsAlive = false;
        return null;
      }

      // Calculate hunger percentage, read-out nearby agents and remove own agent from perception list.
      var hunger = (int) (((double) (EnergyMax - _energy)/EnergyMax)*100);
      var spatials = _environment.ExploreAll().ToList();
      spatials.Remove(SpatialEntity);
      var agents = new List<IAgent>();
      foreach(var spatial in spatials) agents.Add(_layer.Agents[spatial.AgentGuid]);

      // Differentiate between perceived agent types. 
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
        var nearest = grass[0];
        var dist = GetPosition().GetDistance(nearest.GetPosition());
        foreach (var g in grass) {
          if (GetPosition().GetDistance(g.GetPosition()) < dist) {
            nearest = g;
            dist = GetPosition().GetDistance(nearest.GetPosition());
          }
        }
        _states += String.Format("E: {0,4:0.00} | ", dist);

        // R1: Eat nearby grass.
        if (dist <= 1.4143 && hunger > 20) {
          _states += "R1";
          PrintMessage("[" + GetTick() + "] Schaf " + AgentNumber + " frißt Gras " + nearest.AgentNumber + "!", ConsoleColor.Green);
          return new EatInteraction(this, nearest);
        }

        // R2: Medium grass distance allowed.
        if (dist <= 5 && hunger > 40) {
          _states += "R2";
          var options = _mover.GetMovementOptions(nearest.GetPosition());
          return options.Count == 0 ? null : _mover.MoveInDirection(options[0].Direction);
        }

        // R3: Move to the nearest grass, no matter the distance.
        if (hunger > 60) {
          _states += "R3";
          var options = _mover.GetMovementOptions(nearest.GetPosition());
          return options.Count == 0 ? null : _mover.MoveInDirection(options[0].Direction);
        }
      }

      // Just for distance output appendix (in case there was no target).
      if (grass.Count == 0) _states += "        | ";

      // R4: Perform random movement.
      _states += "R4";
      var x = _random.Next((int)_environment.MaxDimension.X);
      var y = _random.Next((int)_environment.MaxDimension.Y);
      var rndOpts = _mover.GetMovementOptions(new Vector3(x, y));
      return rndOpts.Count == 0 ? null : _mover.MoveInDirection(rndOpts[0].Direction);
    }


    /// <summary>
    ///   Print this sheep's ID, position and its internal states.
    /// </summary>
    /// <returns>Console output string.</returns>
    public override string ToString() {
      return String.Format("{0,3:00} | Schaf | ({1,2:00},{2,2:00})  |  {3,2:0}/{4,2:00}  |" + _states,
          AgentNumber, GetPosition().X, GetPosition().Y, _energy, EnergyMax);
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
      IsAlive = false;
    }
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using DalskiAgent.Agents;
using DalskiAgent.Auxiliary;
using DalskiAgent.Execution;
using DalskiAgent.Movement.Movers;
using DalskiAgent.Perception;
using DalskiAgent.Reasoning;
using EnvironmentServiceComponent.Implementation;
using LayerAPI.Perception;
using LayerAPI.Spatial;
using WolvesModel.Environment;
using WolvesModel.Interactions;
using ISpatialObject = DalskiAgent.Environments.ISpatialObject;

namespace WolvesModel.Agents {
  
  /// <summary>
  ///   The poor prey animal in the wolves vs. sheeps scenario. 
  ///   But hey, at least it can kill grass agent!
  /// </summary>
  internal class Sheep : SpatialAgent, IAgentLogic, IEatInteractionTarget, IEatInteractionSource {
    
    private int _energy = 50;                 // Current energy (with initial value).
    private const int EnergyMax = 80;         // Maximum health.
    private readonly Random _random;          // Random number generator for energy loss.
    private readonly Grassland _environment;  // Environment reference for random movement.   
    private string _states;                   // Output string for console.
    private readonly GridMover _mover;        // Specific agent mover reference (to avoid casts).


    /// <summary>
    ///   Create a new sheep agent.
    /// </summary>
    /// <param name="exec">Agent execution container reference.</param>
    /// <param name="env">Grassland reference.</param>
    /// <param name="pos">Initial agent position.</param>
    public Sheep(IExecution exec, Grassland env, Vector pos = null) : base(exec, env, CollisionType.MassiveAgent, pos) {
      _random = new Random(Id.GetHashCode() + (int) DateTime.Now.Ticks);
      _environment = env;
      
      // Add perception sensor.
      ISpecification halo;
      if (env.UsesEsc) halo = new SpatialHalo(MyGeometryFactory.Rectangle(100, 100), InformationTypes.AllAgents);
      else             halo = new RadialHalo(Data, InformationTypes.AllAgents, 8);
      PerceptionUnit.AddSensor(new DataSensor(this, env, halo));

      // Add movement module.
      Mover = new GridMover(env, this, Data);
      _mover = (GridMover) Mover;  // Re-declaration to save casts.
      Init();
    }


    /// <summary>
    ///   The sheeps brain.
    /// </summary>
    /// <returns>The interaction to execute.</returns>
    public IInteraction Reason() {

      // Energy substraction is made first. 
      _energy -= 1 + _random.Next(3);
      if (_energy <= 0) {
        ConsoleView.AddMessage("["+GetTick()+"] Schaf "+Id+" ist verhungert!", ConsoleColor.DarkRed);
        IsAlive = false;
        return null;
      }


      // Calculate hunger percentage, read-out nearby agents.
      int hunger = (int) (((double) (EnergyMax - _energy)/EnergyMax)*100);
      var rawData = PerceptionUnit.GetData(InformationTypes.AllAgents).Data;
      var agents = ((List<ISpatialObject>) rawData);

      // Remove own agent from perception list.
      if (_environment.UsesEsc) {
        for (var i = 0; i < agents.Count; i ++) {
          if (agents[i] == this) {
            agents.RemoveAt(i);
            break;
          }
        }
      }

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
        if (dist <= 1.4143 && hunger > 20) {
          _states += "R1";
          ConsoleView.AddMessage("[" + GetTick() + "] Schaf " + Id + " frißt Gras " + grs.Id + "!", ConsoleColor.Green);
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
      var rndOpts = _mover.GetMovementOptions(_environment.GetRandomPosition());
      return rndOpts.Count == 0 ? null : _mover.MoveInDirection(rndOpts[0].Direction);
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
      IsAlive = false;
    }
  }
}
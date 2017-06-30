using System;
using System.Collections.Generic;
using System.Linq;
using LIFE.API.Layer;
using LIFE.API.Layer.Initialization;
using LIFE.Components.Agents.BasicAgents.Agents;
using LIFE.Components.Environments.GridEnvironment;
using LIFE.Components.Services.AgentManagerService.Implementation;
using WolvesModel.Agents;
// ReSharper disable ObjectCreationAsStatement
#pragma warning disable 162  // Code is not reachable.

namespace WolvesModel.Environment {

  /// <summary>
  ///   This layer serves as an environment for the Wolves vs. Sheep scenario.
  ///   In addition, it randomly spawns new grass agents for the sheep to eat.
  /// </summary>
  public class EnvironmentLayer : IEnvironmentLayer {

    private readonly IGridEnvironment<GridAgent<Grass>> _gridGrass; //| Separate grids for the
    private readonly IGridEnvironment<GridAgent<Sheep>> _gridSheep; //| positioning of grass,
    private readonly IGridEnvironment<GridAgent<Wolf>> _gridWolves; //| sheep and wolf agents.
    private readonly Random _random;           // Random number generator for agent spawning.
    private readonly int[] _initCounts;        // Agent init counts (if AgentManager is not used).
    private RegisterAgent _regFkt;             // Agent registration function pointer.
    private UnregisterAgent _unregFkt;         // Delegate for unregistration function.
    private long _tick;                        // Current tick.
    public int DimensionX { get; }             // Grid X dimension (- left <=> right +).
    public int DimensionY { get; }             // Grid Y dimension (- down <=>    up +).

    private const bool UseAgentManager = false;


    /// <summary>
    ///   Layer constructor. Set up the grid environments and their dimension.
    /// </summary>
    public EnvironmentLayer() {
      DimensionX = 60;
      DimensionY = 60;
      _initCounts = new[] { 2000, 300, 150 };
      _gridGrass = new GridEnvironment<GridAgent<Grass>>(DimensionX, DimensionY);
      _gridSheep = new GridEnvironment<GridAgent<Sheep>>(DimensionX, DimensionY);
      _gridWolves = new GridEnvironment<GridAgent<Wolf>>(DimensionX, DimensionY);
      _random = new Random(Guid.NewGuid().GetHashCode());
    }


    /// <summary>
    ///   Initializes this layer.
    /// </summary>
    /// <param name="layerInitData">Generic layer init data object. Not used here!</param>
    /// <param name="regHndl">Delegate for agent registration function.</param>
    /// <param name="unregHndl">Delegate for agent unregistration function.</param>
    /// <returns>Initialization success flag.</returns>
    public bool InitLayer(TInitData layerInitData, RegisterAgent regHndl, UnregisterAgent unregHndl) {
      _regFkt = regHndl;
      _unregFkt = unregHndl;

      // ReSharper disable once ConditionIsAlwaysTrueOrFalse
      // ReSharper disable HeuristicUnreachableCode
      if (UseAgentManager) {

        var grass = AgentManager.GetAgentsByAgentInitConfig<Grass>(
          layerInitData.AgentInitConfigs.First(e => e.AgentName.Equals("Grass")),
          _regFkt, _unregFkt, new List<ILayer>() {this}, _gridGrass);
        Console.WriteLine("[EnvironmentLayer] Grass spawned (" + grass.Count + ").");

        var sheep = AgentManager.GetAgentsByAgentInitConfig<Sheep>(
          layerInitData.AgentInitConfigs.First(e => e.AgentName.Equals("Sheep")),
          _regFkt, _unregFkt, new List<ILayer>() {this}, _gridSheep);
        Console.WriteLine("[EnvironmentLayer] Sheep spawned (" + sheep.Count + ").");

        var wolves = AgentManager.GetAgentsByAgentInitConfig<Wolf>(
          layerInitData.AgentInitConfigs.First(e => e.AgentName.Equals("Wolf")),
          _regFkt, _unregFkt, new List<ILayer>() {this}, _gridWolves);
        Console.WriteLine("[EnvironmentLayer] Wolves spawned (" + wolves.Count + ").");
      }

      else {
        for (var i = 0; i < _initCounts[0]; i++)
          new Grass(this, _regFkt, _unregFkt, _gridGrass);
        Console.WriteLine("[EnvironmentLayer] Grass spawned ("+_initCounts[0]+").");

        for (var i = 0; i < _initCounts[1]; i++)
          new Sheep(this, _regFkt, _unregFkt, _gridSheep, (Sex)_random.Next(0, 1));
        Console.WriteLine("[EnvironmentLayer] Sheep spawned ("+_initCounts[1]+").");

        for (var i = 0; i < _initCounts[2]; i++)
          new Wolf(this, _regFkt, _unregFkt, _gridWolves);
        Console.WriteLine("[EnvironmentLayer] Wolves spawned ("+_initCounts[2]+").");
      }
      // ReSharper restore HeuristicUnreachableCode

      return true;
    }


    /// <summary>
    ///   Returns the current tick.
    /// </summary>£
    /// <returns>Current tick value.</returns>
    public long GetCurrentTick() {
      return _tick;
    } 


    /// <summary>
    ///   Sets the current tick. This function is called by the RTE manager in each tick.
    /// </summary>
    /// <param name="currentTick">current tick value.</param>
    public void SetCurrentTick(long currentTick) {
      _tick = currentTick;
    }


    /// <summary>
    ///   Post tick execution logic.
    ///   Spawns new grass agents based on their current count.
    /// </summary>
    public void PostTick() {
      var nrGrass = _gridGrass.Explore(0, 0).Count();
      var create = _random.Next(40 + nrGrass*2) < 60;
      if (create) new Grass(this, _regFkt, _unregFkt, _gridGrass);
      Console.WriteLine("\n[EnvironmentLayer] Tick " + _tick +
                        " | Grass: " + _gridGrass.Explore(0, 0).Count() +
                        ", Sheep: " + _gridSheep.Explore(0, 0).Count() +
                        ", Wolves: " + _gridWolves.Explore(0, 0).Count());
    }


    /// <summary>
    ///   Find grass agents in the environment.
    /// </summary>
    /// <param name="posX">Explore base position (X).</param>
    /// <param name="posY">Explore base position (Y).</param>
    /// <param name="viewRange">Exploration range.</param>
    /// <returns>A list of grass agents found.</returns>
    public IList<Grass> FindGrass(int posX, int posY, int viewRange) {
      var list = new List<Grass>();
      var agents = _gridGrass.Explore(posX, posY, viewRange);
      foreach (var agent in agents) list.Add(agent.AgentReference);
      return list;      
    }



    /// <summary>
    ///   Find sheep in the environment.
    /// </summary>
    /// <param name="posX">Explore base position (X).</param>
    /// <param name="posY">Explore base position (Y).</param>
    /// <param name="viewRange">Exploration range.</param>
    /// <returns>A list of sheep agents found.</returns>
    public IList<Sheep> FindSheep(int posX, int posY, int viewRange) {
      var list = new List<Sheep>();
      var agents = _gridSheep.Explore(posX, posY, viewRange);
      foreach (var agent in agents) list.Add(agent.AgentReference);
      return list;      
    }



    /// <summary>
    ///   Find wolves in the environment.
    /// </summary>
    /// <param name="posX">Explore base position (X).</param>
    /// <param name="posY">Explore base position (Y).</param>
    /// <param name="viewRange">Exploration range.</param>
    /// <returns>A list of wolves found.</returns>
    public IList<Wolf> FindWolves(int posX, int posY, int viewRange) {
      var list = new List<Wolf>();
      var agents = _gridWolves.Explore(posX, posY, viewRange);
      foreach (var agent in agents) list.Add(agent.AgentReference);
      return list;      
    }

    
    /// <summary>
    ///   Returns a random free grid cell.
    /// </summary>
    /// <returns>the [x,y] coordinates or 'null', if none could be found.</returns>
    public int[] GetFreeCell() {
      var x = _random.Next(DimensionX);
      var y = _random.Next(DimensionY);
      var found = _gridGrass.Explore(x, y, 1);
      if (found.Any()) {
        Console.WriteLine("Occupied at ("+x+","+y+").");
        return null;
      }
      Console.WriteLine("Free at ("+x+","+y+").");
      return new[] {x, y};
    }


    public void PreTick() {}
    public void Tick() {}
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using EnvironmentServiceComponent.SpatialAPI.Environment;
using GpuEnvironment.Implementation;
using LionsModelAsync.Agents;
using LionsModelAsync.Environment;
using LIFE.API.Layer;
using LIFE.API.Layer.Initialization;
using LIFE.Components.Agents.BasicAgents.Agents;
using LIFE.Components.Environments.GridEnvironment;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.Services.AgentManagerService.Implementation;
using WolvesModel.Agents;
// ReSharper disable ObjectCreationAsStatement
#pragma warning disable 162  // Code is not reachable.

namespace WolvesModel.Environment {

  /// <summary>
  ///   This layer serves as an environment for the Wolves vs. Sheep scenario.
  ///   In addition, it randomly spawns new grass agents for the sheep to eat.
  /// </summary>
  public class EnvironmentLayer : IEnvironmentLayer
  {
      // The GpuESC needs to know the Size of the largest objects/explores which it contains,
      // to determine the internal grid size.
      private const int MAX_EXPLORE_RADIUS = 20;
      private const int ENVIRONMENT_DIM_X = 200;
      private const int ENVIRONMENT_DIM_Y = 200;



        // TODO: Maybe create separate evironments for grass and moving agents.
        private readonly IAsyncEnvironment _environment;
//        private readonly IGridEnvironment<GridAgent<Sheep>> _gridSheep; //| positioning of grass,
//    private readonly IGridEnvironment<GridAgent<Wolf>> _gridWolves; //| sheep and wolf agents.
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
      DimensionX = 150;
      DimensionY = 150;
      _initCounts = new[] { 2000, 300, 150 };
      _environment = new HierarchicalGpuESC(new Vector3(MAX_EXPLORE_RADIUS,MAX_EXPLORE_RADIUS), new Vector3(ENVIRONMENT_DIM_X,ENVIRONMENT_DIM_Y) );
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
          _regFkt, _unregFkt, new List<ILayer>() {this}, _environment);
        Console.WriteLine("[EnvironmentLayer] Grass spawned (" + grass.Count + ").");



        var antelopes = AgentManager.GetAgentsByAgentInitConfig<Antelope>(
          layerInitData.AgentInitConfigs.First(e => e.AgentName.Equals("Antelope")),
          _regFkt, _unregFkt, new List<ILayer>() {this}, _environment);
        Console.WriteLine("[EnvironmentLayer] Sheep spawned (" + antelopes.Count + ").");

        var lions= AgentManager.GetAgentsByAgentInitConfig<Lion>(
          layerInitData.AgentInitConfigs.First(e => e.AgentName.Equals("Lion")),
          _regFkt, _unregFkt, new List<ILayer>() {this}, _environment);
        Console.WriteLine("[EnvironmentLayer] Wolves spawned (" + lions.Count + ").");
      }

      else {
//        for (var i = 0; i < _initCounts[0]; i++)
//          new Grass(this, _regFkt, _unregFkt, _environment);
        Console.WriteLine("[EnvironmentLayer] Grass spawned ("+_initCounts[0]+").");

        for (var i = 0; i < _initCounts[1]; i++)
          new Antelope(this, _regFkt, _unregFkt, _environment);
        Console.WriteLine("[EnvironmentLayer] Sheep spawned ("+_initCounts[1]+").");

        for (var i = 0; i < _initCounts[2]; i++)
          new Lion(this, _regFkt, _unregFkt, _environment);
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

            //TODO: Implement grass respawn

//      var nrGrass = _gridGrass.Explore(0, 0).Count();
//      var create = _random.Next(40 + nrGrass*2) < 60;
//      if (create) new Grass(this, _regFkt, _unregFkt, _gridGrass);
//      Console.WriteLine("\n[EnvironmentLayer] Tick " + _tick +
//                        " | Grass: " + _gridGrass.Explore(0, 0).Count() +
//                        ", Sheep: " + _gridSheep.Explore(0, 0).Count() +
//                        ", Wolves: " + _gridWolves.Explore(0, 0).Count());
    }



    
    /// <summary>
    ///   Returns a random free grid cell.
    /// </summary>
    /// <returns>the [x,y] coordinates or 'null', if none could be found.</returns>
    public int[] GetFreeCell() {
      var x = _random.Next(DimensionX);
      var y = _random.Next(DimensionY);
//      var found = _gridGrass.Explore(x, y, 1);
//      if (found.Any()) {
//        Console.WriteLine("Occupied at ("+x+","+y+").");
//        return null;
//      }
//      Console.WriteLine("Free at ("+x+","+y+").");
      return new[] {x, y};
    }


    public void PreTick() {}
    public void Tick() {}
  }
}
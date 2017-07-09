using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EnvironmentServiceComponent.SpatialAPI.Environment;
using GpuEnvironment.Implementation;
using LionsModelAsync.Agents;
using LionsModelAsync.Environment;
using LIFE.API.Layer;
using LIFE.API.Layer.Initialization;
using LIFE.Components.Agents.BasicAgents.Agents;
using LIFE.Components.Agents.BasicAgents.Environment;
using LIFE.Components.Agents.BasicAgents.Reasoning;
using LIFE.Components.Environments.GridEnvironment;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.Services.AgentManagerService.Implementation;
using WolvesModel.Agents;

// ReSharper disable ObjectCreationAsStatement
#pragma warning disable 162  // Code is not reachable.

namespace WolvesModel.Environment
{
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
        private IDictionary<Guid, Lion> _lionAgents;
        private IDictionary<Guid, Antelope> _antelopeAgents;
        private IDictionary<Guid, Grass> _grassAgents;


        // TODO: Maybe create separate evironments for grass and moving agents.
        private readonly IAsyncEnvironment _environment;

//        private readonly IGridEnvironment<GridAgent<Sheep>> _gridSheep; //| positioning of grass,
//    private readonly IGridEnvironment<GridAgent<Wolf>> _gridWolves; //| sheep and wolf agents.
        private readonly Random _random; // Random number generator for agent spawning.

        private readonly int[] _initCounts; // Agent init counts (if AgentManager is not used).
        private RegisterAgent _regFkt; // Agent registration function pointer.
        private UnregisterAgent _unregFkt; // Delegate for unregistration function.
        private long _tick; // Current tick.


        public void RemoveAgent(Guid agentID)
        {
            if (_lionAgents.ContainsKey(agentID))
            {
                _lionAgents.Remove(agentID);
                return;
            }
            if (_antelopeAgents.ContainsKey(agentID))
            {
                _antelopeAgents.Remove(agentID);
                return;
            }
            if (_grassAgents.ContainsKey(agentID))
            {
                _grassAgents.Remove(agentID);
                return;
            }
        }

        public int DimensionX { get; } // Grid X dimension (- left <=> right +).
        public int DimensionY { get; } // Grid Y dimension (- down <=>    up +).

        private const bool UseAgentManager = false;


        /// <summary>
        ///   Layer constructor. Set up the grid environments and their dimension.
        /// </summary>
        public EnvironmentLayer()
        {
            DimensionX = 150;
            DimensionY = 150;
            _initCounts = new[] {2000, 300, 150};
//            _lionAgents = new ConcurrentDictionary<Guid, Lion>();
//            _antelopeAgents = new ConcurrentDictionary<Guid, Antelope>();
//            _grassAgents = new ConcurrentDictionary<Guid, Grass>();

            _environment = new HierarchicalGpuESC(new Vector3(MAX_EXPLORE_RADIUS, MAX_EXPLORE_RADIUS),
                new Vector3(ENVIRONMENT_DIM_X, ENVIRONMENT_DIM_Y));
            _random = new Random(Guid.NewGuid().GetHashCode());
        }


        /// <summary>
        ///   Initializes this layer.
        /// </summary>
        /// <param name="layerInitData">Generic layer init data object. Not used here!</param>
        /// <param name="regHndl">Delegate for agent registration function.</param>
        /// <param name="unregHndl">Delegate for agent unregistration function.</param>
        /// <returns>Initialization success flag.</returns>
        public bool InitLayer(TInitData layerInitData, RegisterAgent regHndl, UnregisterAgent unregHndl)
        {
            _regFkt = regHndl;
            _unregFkt = unregHndl;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            // ReSharper disable HeuristicUnreachableCode
            if (UseAgentManager)
            {
                _grassAgents = AgentManager.GetAgentsByAgentInitConfig<Grass>(
                    layerInitData.AgentInitConfigs.First(e => e.AgentName.Equals("Grass")),
                    _regFkt, _unregFkt, new List<ILayer>() {this}, _environment);
                Console.WriteLine("[EnvironmentLayer] Grass spawned (" + _grassAgents.Count + ").");


                _antelopeAgents = AgentManager.GetAgentsByAgentInitConfig<Antelope>(
                    layerInitData.AgentInitConfigs.First(e => e.AgentName.Equals("Antelope")),
                    _regFkt, _unregFkt, new List<ILayer>() {this}, _environment);
                Console.WriteLine("[EnvironmentLayer] Sheep spawned (" + _antelopeAgents.Count + ").");

                _lionAgents = AgentManager.GetAgentsByAgentInitConfig<Lion>(
                    layerInitData.AgentInitConfigs.First(e => e.AgentName.Equals("Lion")),
                    _regFkt, _unregFkt, new List<ILayer>() {this}, _environment);
                Console.WriteLine("[EnvironmentLayer] Wolves spawned (" + _lionAgents.Count + ").");
            }

            else
            {
                _lionAgents = new ConcurrentDictionary<Guid, Lion>();
                _antelopeAgents = new Dictionary<Guid, Antelope>();
                _grassAgents = new ConcurrentDictionary<Guid, Grass>();
                for (var i = 0; i < _initCounts[0]; i++)
                {
                    var tmp = new Grass(this, _regFkt, _unregFkt, _environment);
                    _grassAgents.Add(tmp.ID, tmp);
                }
                Console.WriteLine("[EnvironmentLayer] Grass spawned (" + _initCounts[0] + ").");

                for (var i = 0; i < _initCounts[1]; i++)
                {
                    var tmp = new Antelope(this, _regFkt, _unregFkt, _environment);
                    _antelopeAgents.Add(tmp.ID, tmp);
                }
                Console.WriteLine("[EnvironmentLayer] antelopes spawned (" + _initCounts[1] + ").");

                for (var i = 0; i < _initCounts[2]; i++)
                {
                    var tmp = new Lion(this, _regFkt, _unregFkt, _environment);
                    ;
                    _lionAgents.Add(tmp.ID, tmp);
                }

                Console.WriteLine("[EnvironmentLayer] lions spawned (" + _initCounts[2] + ").");
            }
            // ReSharper restore HeuristicUnreachableCode

            return true;
        }


        /// <summary>
        ///   Returns the current tick.
        /// </summary>£
        /// <returns>Current tick value.</returns>
        public long GetCurrentTick()
        {
            return _tick;
        }


        /// <summary>
        ///   Sets the current tick. This function is called by the RTE manager in each tick.
        /// </summary>
        /// <param name="currentTick">current tick value.</param>
        public void SetCurrentTick(long currentTick)
        {
            _tick = currentTick;
        }


        /// <summary>
        ///   Post tick execution logic.
        ///   Spawns new grass agents based on their current count.
        /// </summary>
        public void PostTick()
        {
            //TODO: Maybe spawn more grass according to agent count

            var nrGrass = _grassAgents.Count;
            var create = _random.Next(40 + nrGrass * 2) < 60;
            if (create)
            {
                var tmp = new Grass(this, _regFkt, _unregFkt, _environment);
                _grassAgents.Add(tmp.ID, tmp);
            }
            Console.WriteLine("\n[EnvironmentLayer] Tick " + _tick +
                              " | Grass: " + _grassAgents.Count +
                              ", Antelopes: " + _antelopeAgents.Count +
                              ", Lions: " + _lionAgents.Count);
        }


        /// <summary>
        ///   Returns a random free grid cell.
        /// </summary>
        /// <returns>the [x,y] coordinates or 'null', if none could be found.</returns>
        public int[] GetFreeCell()
        {
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

        public void GetMoveTowardsPositionVector(Guid agentId, double distance, double x, double y, out Vector3 newPos,
            out Direction newDirection)
        {
            var actShape = _environment.GetSpatialEntity(agentId).Shape;
            // Check, if we are already there. Otherwise no need to move anyway.
            var distanceToPos = actShape.Position.GetDistance(new Vector3(x, y, 0));
            if (Math.Abs(distanceToPos) <= float.Epsilon)
            {
                newPos = actShape.Position;
                newDirection = actShape.Rotation;
                return;
            }


            // Get the right direction.
            var diff = new Vector3(x - actShape.Position.X,
                y - actShape.Position.Y,
                0);
            var dir = new Direction();
            dir.SetDirectionalVector(diff);

            // Check the speed. If we would go too far, reduce it accordingly.
            if (distanceToPos < distance) distance = distanceToPos;

            // Save calculated values to new movement class and return.
//            return MoveInDirection(distance, dir.Yaw, dir.Pitch);
//            var dir = _env.GetSpatialEntity(_agentId).Shape.Rotation;
//            dir.SetYaw(yawAs);
//            dir.SetPitch(pitchAs);
            // Calculate target position based on current position, heading and speed.     
            var pitchRad = Direction.DegToRad(dir.Pitch);
            var yawRad = Direction.DegToRad(dir.Yaw);
            var factor = distance;
            var xNew = factor * Math.Cos(pitchRad) * Math.Sin(yawRad);
            var yNew = factor * Math.Sin(pitchRad);
            var z = 0;
            newPos = new Vector3(xNew, yNew, z);
            newDirection = dir;
        }

        public Grass GetGrass(Guid agentID)
        {
            lock (_grassAgents)
            {
                return _grassAgents.ContainsKey(agentID) ? _grassAgents[agentID] : null;
            }
        }

        public Antelope GetAntelope(Guid agentID)
        {
            lock (_antelopeAgents)
            {
                return _antelopeAgents.ContainsKey(agentID) ? _antelopeAgents[agentID] : null;
            }
        }

        public Lion GetLion(Guid agentID)
        {
            lock (_lionAgents)
            {
                return _lionAgents.ContainsKey(agentID) ? _lionAgents[agentID] : null;
            }
        }


        public void PreTick()
        {
        }

        public void Tick()
        {
        }
    }
}
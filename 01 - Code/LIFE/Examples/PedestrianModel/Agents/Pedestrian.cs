using System;
using System.Collections.Generic;
using DalskiAgent.Agents;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using DalskiAgent.Movement.Actions;
using DalskiAgent.Movement.Movers;
using DalskiAgent.Perception;
using ESCTestLayer.Implementation;
using GenericAgentArchitectureCommon.Datatypes;
using GenericAgentArchitectureCommon.Interfaces;
using PedestrianModel.Agents.Reasoning.Movement;
using PedestrianModel.Agents.Reasoning.Pathfinding;
using PedestrianModel.Agents.Reasoning.Pathfinding.Astar;
using PedestrianModel.Agents.Reasoning.Pathfinding.Raytracing;
using PedestrianModel.Environment;

namespace PedestrianModel.Agents {

    /// <summary>
    ///     A pedestrian agent which moves to a target position using wayfinding and collision avoidance.
    /// </summary>
    public class Pedestrian : SpatialAgent, IAgentLogic {
        public string Name { get; private set; }

        public float MaxVelocity { get; private set; }

        public string SimulationId { get; private set; }

        public IList<Vector> TargetPositions { get { return _targetPositions; } }

        public ReactiveBehaviorPipeline MovementPipeline { get { return _movementPipeline; } }

        /// <summary>
        ///     Track the last n positions to notice if I'm stuck...
        /// </summary>
        private const int PositionTrackerSize = 20;

        private readonly DirectMover _mover;

        private readonly ReactiveBehaviorPipeline _movementPipeline = new ReactiveBehaviorPipeline();
        private readonly IList<Waypoint> _waypoints = new List<Waypoint>();

        /// <summary>
        ///     List of the last n positions.
        /// </summary>
        private readonly IList<Vector> _positionTracker = new List<Vector>();

        private readonly IList<Vector> _targetPositions;

        private bool _preparedFirstStep;

        private Vector _startPosition;
        private RaytracingGraph _pathfindingSearchGraph;
        private IPathfinder<Vector> _pathfinder;

        private int _targetPositionIndex; // if target list type is Sequential, this is the current index

        private Vector _debugLastPosition;

        /// <summary>
        ///     Create a new pedestrian agent.
        /// </summary>
        /// <param name="exec">Executer.</param>
        /// <param name="env">Environment reference.</param>
        /// <param name="simulationId">ID of the simulation which contains the agent.</param>
        /// <param name="position">Initial position.</param>
        /// <param name="dimension">Initial dimension.</param>
        /// <param name="direction">Initial direction.</param>
        /// <param name="targetPosition">Target position.</param>
        /// <param name="maxVelocity">Maximum Velocity.</param>
        /// <param name="name">Name.</param>
        public Pedestrian
            (IExecution exec,
                ObstacleEnvironment env,
                string simulationId,
                Vector position,
                Vector dimension,
                Direction direction,
                Vector targetPosition,
                float maxVelocity,
                String name = "pedestrian")
            : base(exec, env, position, dimension, direction) {
            IEnvironment environment = env;
            MaxVelocity = maxVelocity;
            Name = name;
            SimulationId = simulationId;

            ISpecificator obstaclesHalo;
            ISpecificator pedestriansHalo;
            ISpecificator allAgentsHalo;
            if (env.UsesESC) {
                # warning Size?
                obstaclesHalo = new SpatialHalo(MyGeometryFactory.Rectangle(100, 100), InformationType.Obstacles);
                pedestriansHalo = new SpatialHalo(MyGeometryFactory.Rectangle(100, 100), InformationType.Pedestrians);
                allAgentsHalo = new SpatialHalo(MyGeometryFactory.Rectangle(100, 100), InformationType.AllAgents);
            }
            else {
                obstaclesHalo = new OmniHalo(InformationType.Obstacles);
                pedestriansHalo = new OmniHalo(InformationType.Pedestrians);
                allAgentsHalo = new OmniHalo(InformationType.AllAgents);
            }

            // Add perception sensor for obstacles.
            PerceptionUnit.AddSensor(new DataSensor(this, (IGenericDataSource) environment, obstaclesHalo));

            // Add perception sensor for pedestrians.
            PerceptionUnit.AddSensor(new DataSensor(this, (IGenericDataSource) environment, pedestriansHalo));

            // Add perception sensor for everything.
            PerceptionUnit.AddSensor(new DataSensor(this, (IGenericDataSource) environment, allAgentsHalo));

            // Add movement module.
            Mover = new DirectMover(environment, this, Data);
            _mover = (DirectMover) Mover; // Re-declaration to save casts.

            // WALK
            _targetPositions = new List<Vector> {targetPosition};

            _startPosition = position;

            Init();
        }

        #region IAgentLogic Members

        public IInteraction Reason() {
            if (!_preparedFirstStep) {
                PrepareFirstStep();
                _preparedFirstStep = true;
            }

            IInteraction movementAction = CalculateNewPosition();

            if (Config.DebugEnabled) {
                Console.SetBufferSize(160, 9999);
                Console.SetWindowSize(160, 50);
                if (_debugLastPosition == null) _debugLastPosition = _startPosition;
                string waypoint = " COMPLETED MOVEMENT";
                if (_waypoints.Count > 0)
                    waypoint = (_waypoints[0].TargetPosition*1f).ToString(); // * 1f -> converts 2d Vector to 3d Vector 
                Console.WriteLine
                    ("Tick: " + GetTick().ToString("0000") + ", ID: " + Id.ToString("0000") + ", Position: "
                     + GetPosition()*1f + ", Target: " + TargetPositions[0]*1f + ", Waypoint: " + waypoint
                     + ", Distance: " + GetPosition().GetDistance(TargetPositions[0]).ToString("00.0000000")
                     + ", Velocity: "
                     + (GetPosition().GetDistance(_debugLastPosition)*(1000f/Config.LengthOfTimestepsInMilliseconds)).
                         ToString("00.0000000"));
                _debugLastPosition = GetPosition();
            }

            return movementAction;
        }

        #endregion

        /// <summary>
        ///     Initialize the agent.
        /// </summary>
        private void PrepareFirstStep() {
            _startPosition = GetPosition();
            _movementPipeline.AddBehavior(new ObstacleAvoidanceBehavior(this, PerceptionUnit));

            // - ok, we don't really have an Y axis, but it's Vector3D, so define a hard coded y-value
            // - the 0.28 is in relation to the hardcoded 0.4m size of the agents bounding-box
            object rawObstaclesData = PerceptionUnit.GetData(InformationType.Obstacles).Data;
            IList<Obstacle> obstacles = (List<Obstacle>) rawObstaclesData;
            _pathfindingSearchGraph = new RaytracingGraph
                (SimulationId, obstacles, 0, Math.Max(Config.TargetReachedDistance*2.0f, 0.28f));
            _pathfinder = new AStarPathfinder<Vector>(_pathfindingSearchGraph);

            CreateMovePlan(_targetPositions);
        }

        /// <summary>
        ///     Approaches the first waypoint in the waypoint queue. If the first waypoint was reached or is no longer valid, it
        ///     will be removed without execution. If the queue is empty and the agent is started with the
        ///     <code>walkLoops = true</code> parameter, the agent teleports to its start
        ///     position and starts the movement to the target position again.
        /// </summary>
        private DirectMovementAction CalculateNewPosition() {
            DirectMovementAction directMovementAction;

            // save current position in track-list
            if (GotStuck()) CreateMovePlan(_targetPositions);

            // first check if the current waypoint is reached
            // remove all waypoints from top of the queue until we have one that is not reached
            while (_waypoints.Count > 0) {
                if (_waypoints[0].IsReached(this)) _waypoints.RemoveAt(0);
                else break;
            }

            if (_waypoints.Count > 0) directMovementAction = _waypoints[0].Approach(this, Mover);
            else {
                // code only executed if looped or more than 1 target!
                if (Config.TargetListType == TargetListType.SequentialLoop
                    || (Config.TargetListType == TargetListType.Sequential
                        && _targetPositionIndex + 1 < _targetPositions.Count)) {
                    // walk to the next position in the list
                    _targetPositionIndex = (_targetPositionIndex + 1)%_targetPositions.Count;
                    CreateMovePlan(_targetPositions);
                    directMovementAction = _waypoints[0].Approach(this, Mover);
                }
                    // code only executed if looped
                else if (Config.WalkLoops) {
                    // last target reached, get back to start...
                    directMovementAction = new DirectMovementAction(_mover, _startPosition);
                    // Allowed to already execute this here to immediately change position and have the right agent position for creation of move plan?
                    // Will be executed twice, but shouldn't have any negative effect.
                    directMovementAction.Execute();

                    CreateMovePlan(_targetPositions);
                }
                else {
                    // Don't change position. Agent should remove itself in the next Reason().
                    IsAlive = false;
                    directMovementAction = new DirectMovementAction(_mover, GetPosition());
                }
            }
            return directMovementAction;
        }

        /// <summary>
        ///     Creates a plan with move actions which lead the agent to the target position. The plan will be enqueued
        ///     into the action queue.
        /// </summary>
        /// <param name="targetPositions"> the target positions of the movement. </param>
        private void CreateMovePlan(IList<Vector> targetPositions) {
            IList<Vector> shortestPath = null;

            if (Config.TargetListType == TargetListType.Parallel) {
                double shortestPathLength = double.MaxValue;
                foreach (Vector target in targetPositions) {
                    IList<Vector> path = GetPathToTarget(target);

                    if (path == null) continue;

                    path.Insert(0, GetPosition());

                    double pathLength = 0.0;
                    for (int i = 1; i < path.Count; i++) {
                        pathLength += path[i - 1].GetDistance(path[i]);
                    }

                    if (shortestPathLength > pathLength) {
                        shortestPathLength = pathLength;
                        shortestPath = path;
                    }
                }
            }
            else {
                Vector currentTarget = targetPositions[_targetPositionIndex];
                shortestPath = GetPathToTarget(currentTarget);
            }

            _waypoints.Clear();

            if (shortestPath != null) {
                foreach (Vector waypoint in shortestPath) {
                    _waypoints.Add(new Waypoint(waypoint, Config.TargetReachedDistance));
                }
            }
        }

        /// <summary>
        ///     Calculates a path from the current agent's position to the given target position using the agent's
        ///     pathfinder and searchgraph.
        /// </summary>
        /// <param name="targetPosition"> the position to engage </param>
        /// <returns>
        ///     the resulting path as a list of <seealso cref="Vector" /> positions or <code>null</code> if no path
        ///     could be found.
        /// </returns>
        private IList<Vector> GetPathToTarget(Vector targetPosition) {
            _pathfindingSearchGraph.TargetPosition = targetPosition;

            IList<Vector> path = _pathfinder.FindPath
                (new RaytracingPathNode(GetPosition()), new RaytracingPathNode(_pathfindingSearchGraph.TargetPosition));

            if (path == null || path.Count == 0) return null;

            return path;
        }

        /// <summary>
        ///     Checks if the agent got stuck at a position and needs new pathfinding, to get to it's target position.
        /// </summary>
        /// <returns> true if the agent has not moved a lot during the last steps. </returns>
        private bool GotStuck() {
            _positionTracker.Add(GetPosition());

            if (_positionTracker.Count > PositionTrackerSize) _positionTracker.RemoveAt(0);
            else return false; // only do check if enough positions

            float sumX = 0;
            float sumY = 0;
            float sumZ = 0;
            foreach (Vector v in _positionTracker) {
                sumX += v.X;
                sumY += v.Y;
                sumZ += v.Z;
            }
            Vector averagePosition = new Vector
                (sumX/PositionTrackerSize, sumY/PositionTrackerSize, sumZ/PositionTrackerSize);

            double sumDistance = 0;
            foreach (Vector v in _positionTracker) {
                sumDistance += v.GetDistance(averagePosition);
            }

            double avgDistance = sumDistance/PositionTrackerSize;

            double movingSpeed = MaxVelocity;
            double estDistance = movingSpeed*0.2;

            return avgDistance < estDistance;
        }
    }

}
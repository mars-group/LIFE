using System;
using System.Collections.Generic;
using DalskiAgent.Agents;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using DalskiAgent.Movement;
using DalskiAgent.Movement.Actions;
using DalskiAgent.Movement.Movers;
using DalskiAgent.Perception;
using GenericAgentArchitectureCommon.Interfaces;
using PedestrianModel.Agents.Action;
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
        //private readonly ContinuousMover mover;

        public string Name { get; set; }

        public float MaxVelocity { get { return _maxVelocity; } set { _maxVelocity = value; } }

        public IList<Vector> TargetPositions { get { return _targetPositions; } set { _targetPositions = value; } }

        public ReactiveBehaviorPipeline MovementPipeline { get { return _movementPipeline; } }

        /// <summary>
        ///     Track the last n positions to notice if I'm stuck...
        /// </summary>
        private const int PositionTrackerSize = 20;

        private readonly DirectMover _mover;

        private readonly string _simulationId;
        private readonly ReactiveBehaviorPipeline _movementPipeline = new ReactiveBehaviorPipeline();
        private readonly IList<MoveAction> _actions = new List<MoveAction>();

        /// <summary>
        ///     List of the last n positions.
        /// </summary>
        private readonly IList<Vector> _positionTracker = new List<Vector>();

        private bool _startComplete;

        private float _maxVelocity = 1.34f; // Maximum movement velocity of agent        

        private Vector _startPosition;
        private IList<Vector> _targetPositions;
        private RaytracingGraph _pathfindingSearchGraph;
        private IPathfinder<Vector> _pathfinder;

        private int _targetPositionIndex; // if target list type is Sequential, this is the current index

        private Vector _debugLastPosition;

        /// <summary>
        ///     Create a new pedestrian agent.
        /// </summary>
        /// <param name="exec"></param>
        /// <param name="env">Environment reference.</param>
        /// <param name="simulationId">ID of the simulation which contains the agent.</param>
        /// <param name="position">Initial position.</param>
        /// <param name="dimension">Initial dimension.</param>
        /// <param name="direction">Initial direction.</param>
        /// <param name="targetPosition">Target position.</param>
        /// <param name="name"></param>
        public Pedestrian
            (IExecution exec,
                IEnvironment env,
                string simulationId,
                Vector position,
                Vector dimension,
                Direction direction,
                Vector targetPosition,
                String name = "pedestrian")
            : base(exec, env, position, dimension, direction) {
            IEnvironment environment = env;
            Name = name;
            _simulationId = simulationId;

            // Add perception sensor for obstacles.
            PerceptionUnit.AddSensor
                (new DataSensor
                    (
                    this,
                    (IGenericDataSource) environment,
                    new OmniHalo((int) InformationTypes.Obstacles))
                );

            // Add perception sensor for pedestrians.
            PerceptionUnit.AddSensor
                (new DataSensor
                    (
                    this,
                    (IGenericDataSource) environment,
                    new OmniHalo((int) InformationTypes.Pedestrians))
                );

            // Add perception sensor for everything.
            PerceptionUnit.AddSensor
                (new DataSensor
                    (
                    this,
                    (IGenericDataSource) environment,
                    new OmniHalo((int) InformationTypes.AllAgents))
                );

            // Add movement module.
            Mover = new DirectMover(environment, this, Data);
            _mover = (DirectMover) Mover; // Re-declaration to save casts.

            //Mover = new ContinuousMover(env, this, Data);
            //_mover = (ContinuousMover)Mover;  // Re-declaration to save casts.


            // WALK
            _targetPositions = new List<Vector> {targetPosition};

            _startPosition = position;

            Init();
        }

        #region IAgentLogic Members

        public IInteraction Reason() {
            if (!_startComplete) {
                Start();
                _startComplete = true;
            }

            IInteraction movementAction = Act();

            if (Config.DebugEnabled) {
                Console.SetBufferSize(160, 9999);
                Console.SetWindowSize(160, 50);
                if (_debugLastPosition == null) _debugLastPosition = _startPosition;
                string waypoint = " COMPLETED MOVEMENT";
                if (_actions.Count > 0)
                    waypoint = (_actions[0].TargetPosition*1f).ToString(); // * 1f -> converts 2d Vector to 3d Vector 
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
        //protected internal void ProcessStartEvent(StartEvent @event)
        private void Start() {
            //Environment.VisionSensor.PushMode = false;

            //startPosition = Environment.CurrentPosition;
            _startPosition = GetPosition();

            //movementPipeline.addBehavior(new ObstacleAvoidanceBehavior(this));
            _movementPipeline.AddBehavior(new ObstacleAvoidanceBehavior(this, PerceptionUnit));

            // - ok, we don't really have an Y axis, but it's Vector3D, so define a hard coded y-value
            // - the 0.28 is in relation to the hardcoded 0.4m size of the agents bounding-box
            //pathfindingSearchGraph = new RaytracingGraph(SimulationId, Environment.VisionSensor.ObstaclesAsObjectList, 0.43, Math.Max(targetReachedDistance * 2.0, 0.28));
            object rawObstaclesData = PerceptionUnit.GetData((int) InformationTypes.Obstacles).Data;
            IList<Obstacle> obstacles = (List<Obstacle>) rawObstaclesData;
            _pathfindingSearchGraph = new RaytracingGraph
                (_simulationId, obstacles, 0, Math.Max(Config.TargetReachedDistance*2.0f, 0.28f));
            _pathfinder = new AStarPathfinder<Vector>(_pathfindingSearchGraph);

            CreateAndExecuteMovePlan(_targetPositions);

            // starting agent, starting movement loop
            //int firstMoveDelay = (int?) getStartParameter("firstMoveDelay");
            //Environment.addTimer("act", firstMoveDelay, MOVE_TIMER_INTERVAL);
        }

        /// <summary>
        ///     Executes the first action in the action queue. If the first action is finished or no longer valid, it
        ///     will be removed without execution. If the queue is empty and the agent is started with the
        ///     <code>walkLoops = true</code> parameter, the agent teleports to its start
        ///     position and starts the movement to the target position again.
        /// </summary>
        private DirectMovementAction Act() {
            DirectMovementAction directMovementAction;

            // save current position in track-list
            if (GotStuck()) CreateAndExecuteMovePlan(_targetPositions);

            // first check if the current action has finished
            // remove all actions from top of the queue until we have one that is not finished
            while (_actions.Count > 0) {
                if (_actions[0].IsFinished(this)) {
                    //LOGGER.trace("Agent " + Id + ": finished action " + actions.Remove(0));
                    _actions.RemoveAt(0);
                }
                else break;
            }

            if (_actions.Count > 0) {
#warning Don't perform the action, yet. Just return it here, so it can be returned in Reason().
                //actions[0].PerformAction(this);
                directMovementAction = _actions[0].PerformAction(this, Mover);
            }
            else {
                // code only executed if looped or more than 1 target!
                if (Config.TargetListType == TargetListType.SequentialLoop
                    || (Config.TargetListType == TargetListType.Sequential
                        && _targetPositionIndex + 1 < _targetPositions.Count)) {
                    // walk to the next position in the list
                    _targetPositionIndex = (_targetPositionIndex + 1)%_targetPositions.Count;
                    CreateAndExecuteMovePlan(_targetPositions);

#warning Correct to return DirectMovementAction here?
                    directMovementAction = _actions[0].PerformAction(this, Mover);
                }
                    // code only executed if looped
                else if (Config.WalkLoops) {
                    // last target reached, get back to start...
                    // kind of a hack, should be replaced some time and be done in the environment...
                    //TeleportAction action = new TeleportAction(startPosition);
                    //Environment.executeAction(action);
#warning Return a direct move action which puts the agent back to startPosition (if there's space)
                    directMovementAction = new DirectMovementAction(_mover, _startPosition);
                    // Allowed to already execute this here to immediately change position and have the right agent position for creation of move plan?
                    // Will be executed twice, but shouldn't have any negative effect.
                    directMovementAction.Execute();

                    CreateAndExecuteMovePlan(_targetPositions);
                }
                else {
                    //Environment.executeAction(new SuicideAction());
#warning Remove agents from the evironment here.
                    // Don't change position. Agent should remove itself in the next Reason().
                    IsAlive = false;
                    directMovementAction = new DirectMovementAction(_mover, GetPosition());
                }
            }
            return directMovementAction;
        }

        /// <summary>
        ///     Creates a plan with move action which leads the agent to the target position. The plan will be enqueued
        ///     into the action queue.
        /// </summary>
        /// <param name="targetPositions"> the target positions of the movement. </param>
        private void CreateAndExecuteMovePlan(IList<Vector> targetPositions) {
            IList<Vector> shortestPath = null;

            if (Config.TargetListType == TargetListType.Parallel) {
                double shortestPathLength = double.MaxValue;
                foreach (Vector target in targetPositions) {
                    IList<Vector> path = GetPathToTarget(target);

                    if (path == null) continue;

                    //path.Insert(0, Environment.CurrentPosition);
                    path.Insert(0, GetPosition());

                    double pathLength = 0.0;
                    for (int i = 1; i < path.Count; i++) {
                        //pathLength += path[i - 1].distance(path[i]);
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

            _actions.Clear();

            if (shortestPath != null) {
                foreach (Vector waypoint in shortestPath) {
                    _actions.Add(new MoveAction(waypoint, Config.TargetReachedDistance));
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

            //IList<Vector3D> path = pathfinder.FindPath(new RaytracingPathNode(Environment.CurrentPosition), new RaytracingPathNode(pathfindingSearchGraph.TargetPosition));
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
            //this.positionTracker.Add(Environment.CurrentPosition);
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
                //sumDistance += v.distance(averagePosition);
                sumDistance += v.GetDistance(averagePosition);
            }

            double avgDistance = sumDistance/PositionTrackerSize;

            //double movingSpeed = (double?) getStartParameter("movingSpeed");
            double movingSpeed = _maxVelocity;
            double estDistance = movingSpeed*0.2;

            return avgDistance < estDistance;
        }
    }

}
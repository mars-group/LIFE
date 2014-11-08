using CommonTypes.TransportTypes;
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
using PedestrianModel.Util.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedestrianModel.Agents
{
    /// <summary>
    ///   A pedestrian agent which moves to a target position using wayfinding and collision avoidance.
    /// </summary>
    public class Pedestrian : SpatialAgent, IAgentLogic
    {
        private bool startComplete = false;

        private String name;               // Name or ID of agent      
        private float maxVelocity;         // Maximum movement velocity of agent        

        private readonly IEnvironment environment;
        private readonly DirectMover mover;
        //private readonly ContinuousMover mover;

        /// <summary>
        /// Recalculate movement direction every n milli seconds. </summary>
        private const long MOVE_TIMER_INTERVAL = 100;
        /// <summary>
        /// Track the last n positions to notice if I'm stuck... </summary>
        private const int POSITION_TRACKER_SIZE = 20;

        private string simulationId;

        private Vector startPosition;
        private IList<Vector> targetPositions;        
        private RaytracingGraph pathfindingSearchGraph;
        private IPathfinder<Vector> pathfinder;
        private readonly ReactiveBehaviorPipeline movementPipeline = new ReactiveBehaviorPipeline();
        private readonly IList<MoveAction> actions = new List<MoveAction>();
        
        private int targetPositionIndex = 0; // if target list type is Sequential, this is the current index

        /// <summary>
        /// List of the last n positions. </summary>
        private readonly IList<Vector> positionTracker = new List<Vector>();

        /// <summary>
        ///   Create a new pedestrian agent.
        /// </summary>
        /// <param name="id">Agent identifier.</param>
        /// <param name="env">Environment reference.</param>
        /// <param name="pos">Initial position.</param>
        public Pedestrian(IExecution exec, IEnvironment env, string simulationId, Vector position, Vector dimension, Direction direction, Vector targetPosition, String name = "pedestrian")
            : base(exec, env, position)
        {
            this.environment = env;
            this.name = name;
            this.simulationId = simulationId;

            Data.Dimension.X = dimension.X;
            Data.Dimension.Y = dimension.Y;
            Data.Dimension.Z = dimension.Z;

            // Up/Down
            Data.Direction.SetPitch(direction.Pitch);
            // Left/Right
            Data.Direction.SetYaw(direction.Yaw);

            // Add perception sensor for obstacles.
            PerceptionUnit.AddSensor(new DataSensor(
              this, environment,
              (int)InformationTypes.Obstacles,
              //new RadialHalo(Data.Position, 8))
              new OmniHalo())
            );

            // Add perception sensor for pedestrians.
            PerceptionUnit.AddSensor(new DataSensor(
              this, environment,
              (int)InformationTypes.Pedestrians,
              //new RadialHalo(Data.Position, 8))
              new OmniHalo())
            );

            // Add perception sensor for everything.
            PerceptionUnit.AddSensor(new DataSensor(
              this, environment,
              (int)InformationTypes.AllAgents,
                //new RadialHalo(Data.Position, 8))
              new OmniHalo())
            );

            // Add movement module.
            Mover = new DirectMover(environment, this, Data);
            this.mover = (DirectMover)Mover;  // Re-declaration to save casts.

            //Mover = new ContinuousMover(env, this, Data);
            //_mover = (ContinuousMover)Mover;  // Re-declaration to save casts.


            // WALK
            this.targetPositions = new List<Vector>();
            this.targetPositions.Add(targetPosition);

            this.startPosition = position;
                        
            Init();
        }

        public IInteraction Reason()
        {
            if (!startComplete)
            {
                Start();
                startComplete = true;
            }

            // TODO:
            // - agent logging
            // - simulation visualization

            return Act();
        }

        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        public float MaxVelocity
        {
            get { return maxVelocity; }
            set { maxVelocity = value; }
        }

        public IList<Vector> TargetPositions
        {
            get { return targetPositions; }
            set { targetPositions = value; }
        }

        public ReactiveBehaviorPipeline MovementPipeline
        {
            get
            {
                return movementPipeline;
            }
        }

		/// <summary>
		/// Process start event.
		/// </summary>
		/// <param name="event"> the event </param>
		//protected internal void ProcessStartEvent(StartEvent @event)
        protected internal void Start()
		{
			//Environment.VisionSensor.PushMode = false;

			//startPosition = Environment.CurrentPosition;
            startPosition = GetPosition();

            //movementPipeline.addBehavior(new ObstacleAvoidanceBehavior(this));
			movementPipeline.AddBehavior(new ObstacleAvoidanceBehavior(this, PerceptionUnit));

			// - ok, we don't really have an Y axis, but it's Vector3D, so define a hard coded y-value
			// - the 0.28 is in relation to the hardcoded 0.4m size of the agents bounding-box
			//pathfindingSearchGraph = new RaytracingGraph(SimulationId, Environment.VisionSensor.ObstaclesAsObjectList, 0.43, Math.Max(targetReachedDistance * 2.0, 0.28));
            var rawObstaclesData = PerceptionUnit.GetData((int)InformationTypes.Obstacles).Data;
            IList<Obstacle> obstacles = (List<Obstacle>)rawObstaclesData;
            pathfindingSearchGraph = new RaytracingGraph(simulationId, obstacles, 0.43f, Math.Max(Config.targetReachedDistance * 2.0f, 0.28f));
			pathfinder = new AStarPathfinder<Vector>(pathfindingSearchGraph);

			CreateAndExecuteMovePlan(targetPositions);

			// starting agent, starting movement loop
			//int firstMoveDelay = (int?) getStartParameter("firstMoveDelay");
			//Environment.addTimer("act", firstMoveDelay, MOVE_TIMER_INTERVAL);
		}

		/// <summary>
		/// Executes the first action in the action queue. If the first action is finished or no longer valid, it
		/// will be removed without execution. If the queue is empty and the agent is started with the
		/// <code>walkLoops = true</code> parameter, the agent executes a <seealso cref="TeleportAction"/> to it#s start
		/// position and starts the movement to the target position again.
		/// </summary>
		private DirectMovementAction Act()
		{
            DirectMovementAction directMovementAction;

			// save current position in track-list
			if (GotStuck())
			{
				CreateAndExecuteMovePlan(targetPositions);
			}

			// first check if the current action has finished
			// remove all actions from top of the queue until we have one that is not finished
			while (actions.Count > 0)
			{
				if (actions[0].IsFinished(this))
				{
					//LOGGER.trace("Agent " + Id + ": finished action " + actions.Remove(0));
                    actions.RemoveAt(0);					
				}
				else
				{
					break;
				}
			}

			if (actions.Count > 0)
			{
                #warning Don't perform the action, yet. Just return it here, so it can be returned in Reason().
                //actions[0].PerformAction(this);
				directMovementAction = actions[0].PerformAction(this, Mover);
			}
			else
			{
                // code only executed if looped or more than 1 target!
				if (Config.targetListType == TargetListType.SequentialLoop || (Config.targetListType == TargetListType.Sequential && this.targetPositionIndex + 1 < this.targetPositions.Count))
				{
					// walk to the next position in the list
					this.targetPositionIndex = (this.targetPositionIndex + 1) % this.targetPositions.Count;
					CreateAndExecuteMovePlan(targetPositions);

                    #warning Correct to return DirectMovementAction here?
                    directMovementAction = actions[0].PerformAction(this, Mover);
				}
                // code only executed if looped
				else if (Config.walkLoops)
				{
					// last target reached, get back to start...
					// kind of a hack, should be replaced some time and be done in the environment...
					//TeleportAction action = new TeleportAction(startPosition);
					//Environment.executeAction(action);
                    #warning Return a direct move action which puts the agent back to startPosition (if there's space)
                    directMovementAction = new DirectMovementAction(mover, startPosition);
                    // Allowed to already execute this here to immediately change position and have the right agent position for creation of move plan?
                    // Will be executed twice, but shouldn't have any negative effect.
                    directMovementAction.Execute();

					CreateAndExecuteMovePlan(targetPositions);
				}
				else
				{
					//Environment.executeAction(new SuicideAction());
                    #warning Remove agents from the evironment here.
                    // Don't change position. Agent should remove itself in the next Reason().
                    IsAlive = false;
                    directMovementAction = new DirectMovementAction(mover, GetPosition());
				}
			}
            return directMovementAction;
		}

		/// <summary>
		/// Creates a plan with move action which leads the agent to the target position. The plan will be enqueued
		/// into the action queue.
		/// </summary>
		/// <param name="targetPositions"> the target positions of the movement. </param>
		private void CreateAndExecuteMovePlan(IList<Vector> targetPositions)
		{
			IList<Vector> shortestPath = null;

			if (Config.targetListType == TargetListType.Parallel)
			{
				double shortestPathLength = double.MaxValue;
				foreach (Vector target in targetPositions)
				{
					IList<Vector> path = GetPathToTarget(target);

					if (path == null)
					{
						continue;
					}

					//path.Insert(0, Environment.CurrentPosition);
                    path.Insert(0, GetPosition());

					double pathLength = 0.0;
					for (int i = 1; i < path.Count; i++)
					{
						//pathLength += path[i - 1].distance(path[i]);
                        pathLength += path[i - 1].GetDistance(path[i]);
					}

					if (shortestPathLength > pathLength)
					{
						shortestPathLength = pathLength;
						shortestPath = path;
					}
				}
			}
			else
			{
				Vector currentTarget = targetPositions[targetPositionIndex];
				shortestPath = GetPathToTarget(currentTarget);

			}

			this.actions.Clear();

			if (shortestPath != null)
			{
				foreach (Vector waypoint in shortestPath)
				{
					this.actions.Add(new MoveAction(waypoint, Config.targetReachedDistance));
				}
			}
		}

		/// <summary>
		/// Calculates a path from the current agent's position to the given target position using the agent's
		/// pathfinder and searchgraph.
		/// </summary>
		/// <param name="targetPosition"> the position to engage </param>
		/// <returns> the resulting path as a list of <seealso cref="Vector3D"/> positions or <code>null</code> if no path
		///         could be found. </returns>
		private IList<Vector> GetPathToTarget(Vector targetPosition)
		{

			this.pathfindingSearchGraph.TargetPosition = targetPosition;

			//IList<Vector3D> path = pathfinder.FindPath(new RaytracingPathNode(Environment.CurrentPosition), new RaytracingPathNode(pathfindingSearchGraph.TargetPosition));
            IList<Vector> path = pathfinder.FindPath(new RaytracingPathNode(GetPosition()), new RaytracingPathNode(pathfindingSearchGraph.TargetPosition));

			if (path == null || path.Count == 0)
			{
				return null;
			}

			return path;
		}

		/// <summary>
		/// Checks if the agent got stuck at a position and needs new pathfinding, to get to it's target position.
		/// </summary>
		/// <returns> true if the agent has not moved a lot during the last steps. </returns>
		private bool GotStuck()
		{
			//this.positionTracker.Add(Environment.CurrentPosition);
            this.positionTracker.Add(GetPosition());
            
			if (this.positionTracker.Count > POSITION_TRACKER_SIZE)
			{
				this.positionTracker.RemoveAt(0);
			}
			else
			{
				return false; // only do check if enough positions
			}

			float sumX = 0;
			float sumY = 0;
			float sumZ = 0;
			foreach (Vector v in this.positionTracker)
			{
				sumX += v.X;
				sumY += v.Y;
				sumZ += v.Z;
			}
            Vector averagePosition = new Vector(sumX / POSITION_TRACKER_SIZE, sumY / POSITION_TRACKER_SIZE, sumZ / POSITION_TRACKER_SIZE);

			double sumDistance = 0;
			foreach (Vector v in this.positionTracker)
			{
				//sumDistance += v.distance(averagePosition);
                sumDistance += v.GetDistance(averagePosition);
			}

			double avgDistance = sumDistance / POSITION_TRACKER_SIZE;

			//double movingSpeed = (double?) getStartParameter("movingSpeed");
            double movingSpeed = maxVelocity;
			double estDistance = movingSpeed * 0.2;

			return avgDistance < estDistance;
		}

    }
}

using CommonTypes.TransportTypes;
using DalskiAgent.Agents;
using DalskiAgent.Environments;
using DalskiAgent.Movement;
using DalskiAgent.Movement.Actions;
using DalskiAgent.Movement.Movers;
using DalskiAgent.Perception;
using GenericAgentArchitectureCommon.Interfaces;
using PedestrianModel.Environment;
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
    internal class Pedestrian : SpatialAgent, IAgentLogic
    {

        private String _name;               // Name or ID of agent
        private Vector _targetPosition;     // Position agent tries to reach
        private float _maxVelocity;         // Maximum movement velocity of agent

        private readonly IEnvironment _environment;
        private readonly DirectMover _mover;
        //private readonly ContinuousMover _mover;

        /// <summary>
        ///   Create a new pedestrian agent.
        /// </summary>
        /// <param name="id">Agent identifier.</param>
        /// <param name="env">Environment reference.</param>
        /// <param name="pos">Initial position.</param>
        public Pedestrian(long id, IEnvironment environment, Vector position, Vector dimension, Direction direction, Vector targetPosition, String name = "pedestrian")
            : base(id, environment, position)
        {
            _environment = environment;

            _name = name;
            _targetPosition = targetPosition;

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
              (int)ObstacleEnvironment.InformationTypes.Obstacles,
              //new RadialHalo(Data.Position, 8))
              new OmniHalo())
            );

            // Add perception sensor for pedestrians.
            PerceptionUnit.AddSensor(new DataSensor(
              this, environment,
              (int)ObstacleEnvironment.InformationTypes.Pedestrians,
              //new RadialHalo(Data.Position, 8))
              new OmniHalo())
            );

            // Add movement module.
            Mover = new DirectMover(environment, this, Data);
            _mover = (DirectMover)Mover;  // Re-declaration to save casts.

            //Mover = new ContinuousMover(env, this, Data);
            //_mover = (ContinuousMover)Mover;  // Re-declaration to save casts.
        }

        public IInteraction Reason()
        {
            // - Wayfinding
            // - Collision avoidance
            // - Position logging

            var rawObstaclesData = PerceptionUnit.GetData((int)ObstacleEnvironment.InformationTypes.Obstacles).Data;
            var obstacles = ((Dictionary<long, Obstacle>)rawObstaclesData).Values;

            Console.WriteLine("Obstacles:");
            foreach (Obstacle obstacle in obstacles) {
                Console.WriteLine(obstacle);
            }
            Console.WriteLine("---");

            var rawPedestriansData = PerceptionUnit.GetData((int)ObstacleEnvironment.InformationTypes.Pedestrians).Data;
            var pedestrians = ((Dictionary<long, Pedestrian>)rawPedestriansData).Values;

            Console.WriteLine("Pedestrians:");
            foreach (Pedestrian pedestrian in pedestrians)
            {
                Console.WriteLine(pedestrian);
            }
            Console.WriteLine("---");

            

            // TODO:
            // - Some kind of starter (create a scenario, run it)
            // - Reasoning logic

            // decide where to move in the next step
            // if collision occurs old position is kept

            var nextPosition = new Vector(GetPosition().X + 1, GetPosition().Y + 1);
            return new DirectMovementAction(_mover, nextPosition);
        }
    }
}

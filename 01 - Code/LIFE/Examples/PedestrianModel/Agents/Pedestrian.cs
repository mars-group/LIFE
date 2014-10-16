using CommonTypes.TransportTypes;
using GenericAgentArchitecture.Agents;
using GenericAgentArchitecture.Environments;
using GenericAgentArchitecture.Movement;
using GenericAgentArchitecture.Movement.Actions;
using GenericAgentArchitecture.Movement.Movers;
using GenericAgentArchitecture.Perception;
using GenericAgentArchitectureCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedestrianModel
{
    /// <summary>
    ///   A pedestrian agent which moves to a target position using wayfinding and collision avoidance.
    /// </summary>
    internal class Pedestrian : SpatialAgent, IAgentLogic
    {

        private String _name;               // Name or ID of agent
        private TVector _bounds;             // Size (x, y, z) of agent
        private TVector _targetPosition;     // Position agent tries to reach
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
        public Pedestrian(long id, IEnvironment env, TVector pos, TVector bounds, TVector targetPosition, String name = "pedestrian")
            : base(id, env, pos)
        {
            _environment = env;

            _name = name;
            _bounds = bounds;
            _targetPosition = targetPosition;

            // Add perception sensor for obstacles.
            PerceptionUnit.AddSensor(new DataSensor(
              this, env,
              (int)ObstacleEnvironment.InformationTypes.Obstacles,
              //new RadialHalo(Data.Position, 8))
              new OmniHalo())
            );

            // Add perception sensor for pedestrians.
            PerceptionUnit.AddSensor(new DataSensor(
              this, env,
              (int)ObstacleEnvironment.InformationTypes.Pedestrians,
              //new RadialHalo(Data.Position, 8))
              new OmniHalo())
            );

            // Add movement module.
            Mover = new DirectMover(env, this, Data);
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

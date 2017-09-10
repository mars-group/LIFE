using System;
using System.Collections.Generic;
using System.Text;
using LIFE.Components.Agents.BasicAgents.Movement;
using LIFE.Components.Agents.BasicAgents.Perception;
using LIFE.Components.Agents.BasicAgents.Reasoning;
using LIFE.Components.ESC.SpatialAPI.Shape;

namespace AsyncAgents.Movement
{
    public class AgentMover2DAsync : AgentMover
    {
        private readonly AgentMover3DAsync _mover3D; // The 3D mover to use internally.


        /// <summary>
        ///   Create a new 2D mover for continuous environments.
        /// </summary>
        /// <param name="mover3D">The 3D mover to use internally.</param>
        /// <param name="sensors">SensorArray (Should be the same array as the mover3D uses.</param>
        public AgentMover2DAsync(AgentMover3DAsync mover3D, SensorArray sensors): base(sensors)
        {
            _mover3D = mover3D;
        }


        /// <summary>
        ///   Try to insert this agent into the environment at the given position.
        /// </summary>
        /// <param name="x">AgentReference start position (x-coordinate).</param>
        /// <param name="y">AgentReference start position (y-coordinate).</param>
        /// <returns>Success flag. If failed, the agent may not be moved!</returns>
        public void InsertIntoEnvironment(double x, double y, IShape shape = null)
        {
            _mover3D.InsertIntoEnvironment(x, y, 0, shape);
        }


        /// <summary>
        ///   MoveToPosition the agent forward with a given speed.
        /// </summary>
        /// <param name="distance">The distance to move.</param>
        /// <returns>Interaction expressing this movement.</returns>
        public MovementAction MoveForward(double distance)
        {
            return _mover3D.MoveForward(distance);
        }


        /// <summary>
        ///   MoveToPosition the agent into a direction.
        /// </summary>
        /// <param name="distance">The distance to move.</param>
        /// <param name="yaw">New agent heading.</param>
        /// <returns>An interaction describing the movement.</returns>
        public MovementAction MoveInDirection(double distance, double yaw)
        {
            return _mover3D.MoveInDirection(distance, yaw, 0);
        }


        /// <summary>
        ///   MoveToPosition the agent to a position.
        /// </summary>
        /// <param name="distance">The distance to move.</param>
        /// <param name="x">X-coordinate target position.</param>
        /// <param name="y">Y-coordinate of target position.</param>
        /// <returns>An interaction describing the movement.</returns>
        public MovementAction MoveTowardsPosition(double distance, double x, double y)
        {
            return _mover3D.MoveTowardsPosition(distance, x, y, 0);
        }


        /// <summary>
        ///   Set the agent to a new position.
        /// </summary>
        /// <param name="x">X-coordinate target position.</param>
        /// <param name="y">Y-coordinate of target position.</param>
        /// <returns>An interaction describing the movement.</returns>
        public MovementAction SetToPosition(double x, double y)
        {
            return _mover3D.SetToPosition(x, y, 0);
        }
    }
    
}

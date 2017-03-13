﻿using System;
using System.Collections.Generic;
using LIFE.API.Environment.GridCommon;
using LIFE.Components.Agents.BasicAgents.Agents;
using LIFE.Components.Agents.BasicAgents.Perception;
using LIFE.Components.Agents.BasicAgents.Reasoning;
using LIFE.Components.Environments.GridEnvironment;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace LIFE.Components.Agents.BasicAgents.Movement
{
    /// <summary>
    ///   Two-dimensional, grid-based movement module.
    ///   For now, this mover rests upon the 2D continuous environment, emulating grid behaviour.
    /// </summary>
    public class GridMover<T> : AgentMover where T : IGridCoordinate {

        private readonly IGridEnvironment<GridAgent<T>> _grid; // The grid environment to use.
        private GridAgent<T> _agent; // AgentReference position structure.

        public bool DiagonalEnabled { get; set; } // This flag enables diagonal movement [default: disabled].


        /// <summary>
        ///   Create a new grid movement module.
        /// </summary>
        /// <param name="env">The grid environment to use.</param>
        /// <param name="pos">AgentReference position data structure.</param>
        /// <param name="sensorArray">The agent's sensor array (to provide movement feedback).</param>
        public GridMover(IGridEnvironment<GridAgent<T>> env, GridAgent<T> agent, SensorArray sensorArray)
            : base(sensorArray)
        {
            _grid = env;
            _agent = agent;
        }

        public void InsertIntoEnvironment(int x, int y)
        {
            _agent.SetPosition(new GridCoordinate(x,y));
            _grid.Insert(_agent);
        }

        /// <summary>
        ///   Perform grid-based movement.
        /// </summary>
        /// <param name="dir">The direction to move (enumeration value).</param>
        /// <returns>An interaction object that contains the code to execute this movement.</returns>
        public MovementAction MoveInDirection(GridDirection dir)
        {
            var x = _agent.X;
            var y = _agent.Y;
            switch (dir)
            {
                case GridDirection.Up:
                    return SetToPosition(x, y + 1, dir);
                case GridDirection.UpRight:
                    return SetToPosition(x + 1, y + 1, dir);
                case GridDirection.Right:
                    return SetToPosition(x + 1, y, dir);
                case GridDirection.DownRight:
                    return SetToPosition(x + 1, y - 1, dir);
                case GridDirection.Down:
                    return SetToPosition(x, y - 1, dir);
                case GridDirection.DownLeft:
                    return SetToPosition(x - 1, y - 1, dir);
                case GridDirection.Left:
                    return SetToPosition(x - 1, y, dir);
                case GridDirection.UpLeft:
                    return SetToPosition(x - 1, y + 1, dir);
                default:
                    return null;
            }
        }


        /// <summary>
        ///   Set this agent to a new position.
        /// </summary>
        /// <param name="x">X coordinate to move to.</param>
        /// <param name="y">Y coordinate to move to.</param>
        /// <param name="dir">AgentReference orientation (optional).</param>
        /// <returns></returns>
        public MovementAction SetToPosition(int x, int y, GridDirection dir = GridDirection.NotSet)
        {
            return new MovementAction(() =>
            {
                var result = _grid.MoveToPosition(_agent, x, y);
                if (result.X == _agent.X && result.Y == _agent.Y)
                {
                    MovementSensor.SetMovementResult(new MovementResult(MovementStatus.OutOfBounds));
                }
                else
                {
                    if (dir == GridDirection.NotSet)
                    {
                        dir = _agent.GridDirection;
                    }
                    _agent.SetPosition(new GridCoordinate(result.X, result.Y, dir));
                    MovementSensor.SetMovementResult(new MovementResult(MovementStatus.Success));
                }
            });
        }

        public MovementAction MoveTowardsTarget(int xDestination, int yDestination, int distance = 1)
        {
            return new MovementAction(() =>
            {
                var result = _grid.MoveTowardsTarget(_agent, xDestination, yDestination, distance);
                if (result.X == _agent.X && result.Y == _agent.Y)
                {
                    MovementSensor.SetMovementResult(new MovementResult(MovementStatus.OutOfBounds));
                }
                else
                {
                    _agent.SetPosition(new GridCoordinate(result.X, result.Y, result.GridDirection));
                    MovementSensor.SetMovementResult(new MovementResult(MovementStatus.Success));
                }
            });
        }


    }
}
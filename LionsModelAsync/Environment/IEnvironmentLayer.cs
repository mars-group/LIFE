using System;
using System.Collections.Generic;
using LionsModelAsync.Agents;
using LIFE.API.Agent;
using LIFE.API.Layer;
using LIFE.Components.Agents.BasicAgents.Environment;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using WolvesModel.Agents;

namespace LionsModelAsync.Environment
{
    /// <summary>
    ///   Interface for the environment layer.
    /// </summary>
    public interface IEnvironmentLayer : ISteppedActiveLayer
    {
        /// <summary>
        ///   Returns a random free grid cell.
        /// </summary>
        /// <returns>the [x,y] coordinates or 'null', if none could be found.</returns>
        int[] GetFreeCell();


        void GetMoveTowardsPositionVector(Guid agentId, double distance, double x, double y, out Vector3 newPos, out Direction newDirection);

        Antelope GetAntelope(Guid agentID);
        Lion GetLion(Guid agentID);
        Grass GetGrass(Guid agentID);

        /// <summary>
        ///   The environment's extent in X-direction.
        /// </summary>
        int DimensionX { get; }


        /// <summary>
        ///   Y-direction environment extent.
        /// </summary>
        int DimensionY { get; }
    }
}
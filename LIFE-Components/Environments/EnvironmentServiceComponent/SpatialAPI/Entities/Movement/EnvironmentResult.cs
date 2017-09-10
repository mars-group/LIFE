using System;
using System.Collections.Generic;
using System.Text;
using LIFE.Components.ESC.SpatialAPI.Entities;

namespace EnvironmentServiceComponent.SpatialAPI.Entities.Movement
{
    public class EnvironmentResult
    {
        /// <summary>
        /// True, if the movement attempt succeeded. Fals otherwise.
        /// </summary>
        public readonly bool Success;

        public bool GetSuccess()
        {
            return Success;
        }

        public readonly EnvironmentResultCode ResultCode;

        public EnvironmentResultCode GetEnvironmentResult()
        {
            return ResultCode;
        }

        /// <summary>
        /// All spatial entities with whom a collision occured OR who are in the explored area.
        /// </summary>
        public IEnumerable<ISpatialEntity> InvolvedEntities { get; private set; }

        /// <summary>
        /// Create a EnvironmentResult by defining its involvedEntities.
        /// </summary>
        /// <param name="involvedEntities">All spatial entities with whom a collision occured OR who are in the explored area.</param>
        /// <param name="resultCode">Indicates if the Movement was successful.</param>
        public EnvironmentResult(IEnumerable<ISpatialEntity> involvedEntities = null, EnvironmentResultCode resultCode = EnvironmentResultCode.OK)
        {
            InvolvedEntities = involvedEntities ?? new List<ISpatialEntity>();
            ResultCode = resultCode;
            Success = ResultCode == EnvironmentResultCode.OK;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace EnvironmentServiceComponent.SpatialAPI.Entities.Movement
{
    [Serializable]
    public enum EnvironmentResultCode
    {
        OK,
        COLLISION,
        ERROR_AGENT_NOT_FOUND,
        ERROR_OUT_OF_BOUNDS,
        ERROR_GENERAL,
        ERROR_INVALID_POSITION,
        ERROR_INVALID_AGENT_SIZE
    }
}

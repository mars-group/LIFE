using System;

namespace LIFE.Components.Services.AgentManagerService.Interface.Exceptions
{
    [Serializable]
    public class ParameterIsNotMappedOrHasNotDefaultValue : Exception
    {
        public ParameterIsNotMappedOrHasNotDefaultValue(string msg) : base(msg)
        {

        }

    }
}


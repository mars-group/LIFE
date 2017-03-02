using System;

namespace LIFE.Components.Services.AgentManagerService.Interface.Exceptions
{
    [Serializable]
    public class AgentConstructorWasNullException : Exception
    {
        public AgentConstructorWasNullException(string msg) : base(msg)
        {
        }
    }
}
using System;

namespace AgentManagerService.Interface.Exceptions
{
    [Serializable]
    public class AgentConstructorWasNullException : Exception
    {
        public AgentConstructorWasNullException(string msg) : base(msg)
        {
        }
    }
}
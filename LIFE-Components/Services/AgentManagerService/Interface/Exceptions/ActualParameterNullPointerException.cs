using System;

namespace LIFE.Components.Services.AgentManagerService.Interface.Exceptions
{
    [Serializable]
    public class ActualParameterNullPointerException : Exception
    {
        public ActualParameterNullPointerException(string msg) : base(msg){}
    }
}
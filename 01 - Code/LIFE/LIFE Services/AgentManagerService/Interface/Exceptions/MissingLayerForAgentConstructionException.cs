using System;

namespace AgentManager.Interface.Exceptions
{
    [Serializable]
    public class MissingLayerForAgentConstructionException : Exception
    {
        public MissingLayerForAgentConstructionException(string msg) : base(msg) {

        }
    }
}
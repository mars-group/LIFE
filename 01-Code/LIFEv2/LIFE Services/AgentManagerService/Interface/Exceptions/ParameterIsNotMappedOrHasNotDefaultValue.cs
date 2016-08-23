using System;

namespace AgentManager
{
    [Serializable]
    public class ParameterIsNotMappedOrHasNotDefaultValue : Exception
    {
        public ParameterIsNotMappedOrHasNotDefaultValue(string msg) : base(msg)
        {

        }

    }
}


using System;
using System.Runtime.Serialization;

namespace AgentManager
{
    [Serializable]
    public class ParameterIsNotMappedOrHasNotDefaultValue : Exception
    {
        public ParameterIsNotMappedOrHasNotDefaultValue(string msg) : base(msg)
        {

        }
        public ParameterIsNotMappedOrHasNotDefaultValue(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context) { }
    }
}


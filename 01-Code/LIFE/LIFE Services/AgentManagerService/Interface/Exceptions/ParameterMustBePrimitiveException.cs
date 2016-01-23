using System;

namespace AgentManager.Interface.Exceptions {
    [Serializable]
    public class ParameterMustBePrimitiveException : Exception {
        public ParameterMustBePrimitiveException(string msg) : base(msg) {
            
        }

		public ParameterMustBePrimitiveException(){}
    }
}
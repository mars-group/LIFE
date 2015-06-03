using System;

namespace AgentManager.Interface.Exceptions {
    [Serializable]
    public class NotEnoughParametersProvidedException : Exception {
        public NotEnoughParametersProvidedException(string msg) : base(msg) {
            
        }
    }
}
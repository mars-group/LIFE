using System;

namespace RuntimeEnvironment.Interfaces
{
    [Serializable]
    public class NoLayerConfigurationPresentException : Exception {
        public NoLayerConfigurationPresentException(string msg) : base(msg) {
            
        }
    }
}

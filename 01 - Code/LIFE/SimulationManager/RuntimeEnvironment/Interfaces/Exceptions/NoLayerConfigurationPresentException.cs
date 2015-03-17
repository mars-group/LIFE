using System;

namespace RuntimeEnvironment.Interfaces
{
    public class NoLayerConfigurationPresentException : Exception {
        public NoLayerConfigurationPresentException(string msg) : base(msg) {
            
        }
    }
}

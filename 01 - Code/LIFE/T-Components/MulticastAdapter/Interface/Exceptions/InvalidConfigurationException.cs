using System;

namespace MulticastAdapter.Interface.Exceptions {
    internal class InvalidConfigurationException : Exception {
        public InvalidConfigurationException(string msg) : base(msg) {}
    }
}
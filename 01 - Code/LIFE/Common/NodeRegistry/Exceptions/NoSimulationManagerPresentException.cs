using System;

namespace NodeRegistry.Exceptions
{
    [Serializable]
    public class NoSimulationManagerPresentException : Exception
    {
        public NoSimulationManagerPresentException(string msg) : base(msg) { }
    }
}

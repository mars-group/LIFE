using System;

namespace ModelContainer.Interfaces.Exceptions
{
    [Serializable]
    public class NoSimulationConfigFoundException : Exception
    {
        public NoSimulationConfigFoundException(string msg) : base(msg)
        {
            
        }
    }
}

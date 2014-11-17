using System;

namespace SMConnector.Exceptions
{
    public class SimulationHasNotBeenStartedException : Exception
    {
        public SimulationHasNotBeenStartedException(string msg)
            : base(msg)
        {

        }
    }
}

using System;


namespace RuntimeEnvironment.Interfaces.Exceptions
{
    public class LayerFailedToInitializeException : Exception
    {
        public LayerFailedToInitializeException(string msg) : base(msg)
        {
            
        }
    }
}

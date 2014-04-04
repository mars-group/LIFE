using System;

namespace LCConnector.Exceptions
{
    /// <summary>
    /// This exception is thrown, when a layer is tried to initialize, that was not yet instantiated.
    /// </summary>
    public class LayerNotInstantiatedException : Exception
    {
    }
}

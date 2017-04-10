using System;

namespace LayerLoader.Interface.Exceptions
{
    public class ModelCodeFailedToLoadException : Exception
    {
        public ModelCodeFailedToLoadException(string msg) : base(msg)
        {
        }
    }
}
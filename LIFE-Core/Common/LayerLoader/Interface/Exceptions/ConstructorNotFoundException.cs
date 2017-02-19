using System;

namespace LayerLoader.Interface.Exceptions
{
    public class ConstructorNotFoundException : Exception
    {
        public ConstructorNotFoundException(string msg) : base(msg) { }
    }
}

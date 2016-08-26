using System;

namespace LayerLoader.Interface.Exceptions
{
    public class LayerNotFoundException : Exception
    {
        public LayerNotFoundException(string msg) : base(msg) { }
    }
}

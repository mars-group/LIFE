using System;

namespace MulticastAdapter.Interface.Exceptions {
    internal class NoInterfaceFoundException : Exception {
        public NoInterfaceFoundException(string msg) : base(msg) {}
    }
}
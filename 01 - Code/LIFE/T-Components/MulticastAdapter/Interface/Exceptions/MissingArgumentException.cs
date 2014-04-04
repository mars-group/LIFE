using System;

namespace MulticastAdapter.Interface.Exceptions {
    internal class MissingArgumentException : Exception {
        public MissingArgumentException(string msg) : base(msg) {}
    }
}
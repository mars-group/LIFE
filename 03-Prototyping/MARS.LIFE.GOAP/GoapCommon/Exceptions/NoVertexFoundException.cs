using System;

namespace GoapCommon.Exceptions {
    public class NoVertexFoundException : Exception {
        public NoVertexFoundException() {}

        public NoVertexFoundException(string message)
            : base(message) {}

        public NoVertexFoundException(string message, Exception inner)
            : base(message, inner) {}
    }
}
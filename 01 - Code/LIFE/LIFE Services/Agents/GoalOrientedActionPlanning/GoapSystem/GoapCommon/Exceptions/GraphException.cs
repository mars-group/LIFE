using System;

namespace GoapCommon.Exceptions {

    public class GraphException : Exception {
        public GraphException() {}

        public GraphException(string message)
            : base(message) {}

        public GraphException(string message, Exception inner)
            : base(message, inner) {}
    }

}
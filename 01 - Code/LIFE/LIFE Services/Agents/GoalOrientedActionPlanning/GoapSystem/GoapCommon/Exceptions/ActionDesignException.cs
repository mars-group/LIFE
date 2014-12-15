using System;

namespace GoapCommon.Exceptions {

    public class ActionDesignException : Exception {
        public ActionDesignException() {}

        public ActionDesignException(string message)
            : base(message) {}

        public ActionDesignException(string message, Exception inner)
            : base(message, inner) {}
    }

}
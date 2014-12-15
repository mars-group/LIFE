using System;

namespace GoapCommon.Exceptions {

    public class GoalDesignException : Exception {
        public GoalDesignException() {}

        public GoalDesignException(string message)
            : base(message) {}

        public GoalDesignException(string message, Exception inner)
            : base(message, inner) {}
    }

}
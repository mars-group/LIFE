﻿using System;

namespace GoapCommon.Exceptions
{
    public class AlgorithmException : Exception
    {
        public AlgorithmException() {}

        public AlgorithmException(string message)
            : base(message) {}

        public AlgorithmException(string message, Exception inner)
            : base(message, inner) {}
    }
}

using System;
using System.Runtime.Serialization;

namespace ASC.Communication.ScsServices.Communication.Messages {
    /// <summary>
    ///     Represents a ASC Remote Exception.
    ///     This exception is used to send an exception from an application to another application.
    /// </summary>
    [Serializable]
    public class AcsRemoteException : Exception {
        /// <summary>
        ///     Contstructor.
        /// </summary>
        public AcsRemoteException() {}

        /// <summary>
        ///     Contstructor.
        /// </summary>
        public AcsRemoteException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context) {}

        /// <summary>
        ///     Contstructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        public AcsRemoteException(string message)
            : base(message) {}

        /// <summary>
        ///     Contstructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public AcsRemoteException(string message, Exception innerException)
            : base(message, innerException) {}
    }
}
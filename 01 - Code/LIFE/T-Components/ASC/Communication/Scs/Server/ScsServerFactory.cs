using ASC.Communication.Scs.Communication.EndPoints;

namespace ASC.Communication.Scs.Server {
    /// <summary>
    ///     This class is used to create SCS servers.
    /// </summary>
    public static class ScsServerFactory {
        /// <summary>
        ///     Creates a new SCS Server using an EndPoint.
        /// </summary>
        /// <param name="endPoint">Endpoint that represents address of the server</param>
        /// <returns>Created TCP server</returns>
        public static IAscServer CreateServer(AscEndPoint endPoint) {
            return endPoint.CreateServer();
        }
    }
}
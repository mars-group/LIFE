using ASC.Communication.Scs.Communication.EndPoints;
using ASC.Communication.Scs.Server;

namespace ASC.Communication.ScsServices.Service {
    /// <summary>
    ///     This class is used to build AscService applications.
    /// </summary>
    public static class AscServiceBuilder {
        /// <summary>
        /// Creates an ASC Service with the provided and multiCast Address and by using the default 
        /// </summary>
        /// <param name="port">The endpoint port to be used for the udp socket</param>
        /// <param name="multicastGroup">The mcastaddress to use for communication</param>
        /// <returns></returns>
        public static IAscServiceApplication CreateService(int port, string multicastGroup)
        {
            return new AscServiceApplication(ScsServerFactory.CreateServer(AscEndPoint.CreateEndPoint(port, multicastGroup)));
        }
    }
}
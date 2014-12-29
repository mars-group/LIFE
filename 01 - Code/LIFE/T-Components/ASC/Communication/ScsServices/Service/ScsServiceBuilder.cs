﻿using ASC.Communication.Scs.Communication.EndPoints;
using ASC.Communication.Scs.Server;

namespace ASC.Communication.ScsServices.Service {
    /// <summary>
    ///     This class is used to build AscService applications.
    /// </summary>
    public static class ScsServiceBuilder {
        /// <summary>
        ///     Creates a new SCS Service application using an EndPoint.
        /// </summary>
        /// <param name="endPoint">EndPoint that represents address of the service</param>
        /// <returns>Created SCS service application</returns>
        public static IScsServiceApplication CreateService(ScsEndPoint endPoint) {
            return new ScsServiceApplication(ScsServerFactory.CreateServer(endPoint));
        }

        public static IScsServiceApplication CreateService(string address)
        {
            return new ScsServiceApplication(ScsServerFactory.CreateServer(ScsEndPoint.CreateEndPoint(address)));
        }
    }
}
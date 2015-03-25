﻿using System;
using ASC.Communication.Scs.Client;
using ASC.Communication.Scs.Communication.EndPoints;
using ASC.Communication.Scs.Communication.Messengers;

namespace ASC.Communication.ScsServices.Client {
    /// <summary>
    ///     This class is used to build service clients to remotely invoke methods of a SCS service.
    /// </summary>
    public class AscServiceClientBuilder {
        /// <summary>
        ///     Creates a client to connect to a SCS service.
        /// </summary>
        /// <typeparam name="T">Type of service interface for remote method call</typeparam>
        /// <param name="endpoint">EndPoint of the server</param>
        /// <param name="serviceID"></param>
        /// <param name="clientObject">
        ///     Client-side object that handles remote method calls from server to client.
        ///     May be null if client has no methods to be invoked by server
        /// </param>
        /// <returns>Created client object to connect to the server</returns>
        private static IAscServiceClient<T> CreateClient<T>(AscEndPoint endpoint, Guid serviceID,
            object clientObject = null) where T : class {
            return new AscServiceClient<T>(new RequestReplyMessenger<IScsClient>(endpoint.CreateClient()), clientObject, serviceID);
        }

        /// <summary>
        ///     Creates a client to connect to a SCS service.
        /// </summary>
        /// <typeparam name="T">Type of service interface for remote method call</typeparam>
        /// <param name="listenPort"></param>
        /// <param name="serverListenPort"></param>
        /// <param name="multicastGroup"></param>
        /// <param name="serviceID">The serviceID of the specific serviceObject on the remote host.</param>
        /// <param name="clientObject">
        ///     Client-side object that handles remote method calls from server to client.
        ///     May be null if client has no methods to be invoked by server
        /// </param>
        /// <param name="endpointAddress">EndPoint address of the server</param>
        /// <returns>Created client object to connect to the server</returns>
        public static IAscServiceClient<T> CreateClient<T>(int listenPort, string multicastGroup, Guid serviceID, object clientObject = null) where T : class {
                return CreateClient<T>(AscEndPoint.CreateEndPoint(listenPort, multicastGroup), serviceID, clientObject);
        }


    }
}
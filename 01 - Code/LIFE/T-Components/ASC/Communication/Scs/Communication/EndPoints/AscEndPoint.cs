﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ASC.Communication.Scs.Client;
using ASC.Communication.Scs.Communication.EndPoints.Udp;
using ASC.Communication.Scs.Server;

namespace ASC.Communication.Scs.Communication.EndPoints {
    /// <summary>
    ///     Represents a server side end point in SCS.
    /// </summary>
    public abstract class AscEndPoint
    {

        private static IDictionary<string, AscEndPoint> udpEndPointDictionary = new ConcurrentDictionary<string, AscEndPoint>();

        /// <summary>
        ///     Create a Scs End Point from a string.
        ///     Address must be formatted as: protocol://address
        ///     For example: tcp://89.43.104.179:10048 for a TCP endpoint with
        ///     IP 89.43.104.179 and listenPort 10048.
        /// </summary>
        /// <param name="endPointAddress">Address to create endpoint</param>
        /// <param name="multicastGroup"></param>
        /// <returns>Created end point</returns>
        public static AscEndPoint CreateEndPoint(int listenPort, string multicastGroup) {
            //Check if end point address is null
            if (string.IsNullOrEmpty(multicastGroup)) throw new ArgumentNullException("multicastGroup");

            // check if endpoint for that multicastGroup has already been created
            if (udpEndPointDictionary.ContainsKey(multicastGroup))
            {
                // return if true
                return udpEndPointDictionary[multicastGroup];
            }
            // create endpoint if not already present
            var endpoint = new AscUdpEndPoint(listenPort, multicastGroup);
            udpEndPointDictionary[multicastGroup] = endpoint;
            return endpoint;
        }

        /// <summary>
        ///     Creates a Scs Server that uses this end point to listen incoming connections.
        /// </summary>
        /// <returns>Scs Server</returns>
        internal abstract IScsServer CreateServer();

        /// <summary>
        ///     Creates a Scs Server that uses this end point to connect to server.
        /// </summary>
        /// <returns>Scs Client</returns>
        internal abstract IScsClient CreateClient();
    }
}
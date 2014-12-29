using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ASC.Communication.Scs.Client;
using ASC.Communication.Scs.Communication.EndPoints.Tcp;
using ASC.Communication.Scs.Communication.EndPoints.Udp;
using ASC.Communication.Scs.Server;

namespace ASC.Communication.Scs.Communication.EndPoints {
    /// <summary>
    ///     Represents a server side end point in SCS.
    /// </summary>
    public abstract class ScsEndPoint
    {

        private static IDictionary<string, ScsEndPoint> udpEndPointDictionary = new ConcurrentDictionary<string, ScsEndPoint>();

        /// <summary>
        ///     Create a Scs End Point from a string.
        ///     Address must be formatted as: protocol://address
        ///     For example: tcp://89.43.104.179:10048 for a TCP endpoint with
        ///     IP 89.43.104.179 and port 10048.
        /// </summary>
        /// <param name="endPointAddress">Address to create endpoint</param>
        /// <returns>Created end point</returns>
        public static ScsEndPoint CreateEndPoint(string endPointAddress) {
            //Check if end point address is null
            if (string.IsNullOrEmpty(endPointAddress)) throw new ArgumentNullException("endPointAddress");

            //If not protocol specified, assume TCP.
            var endPointAddr = endPointAddress;
            if (!endPointAddr.Contains("://")) endPointAddr = "tcp://" + endPointAddr;

            //Split protocol and address parts
            var splittedEndPoint = endPointAddr.Split(new[] {"://"}, StringSplitOptions.RemoveEmptyEntries);
            if (splittedEndPoint.Length != 2)
                throw new ApplicationException(endPointAddress + " is not a valid endpoint address.");

            //Split end point, find protocol and address
            var protocol = splittedEndPoint[0].Trim().ToLower();
            var address = splittedEndPoint[1].Trim();
            switch (protocol) {
                case "tcp":
                    return new ScsTcpEndPoint(address);
                case "udp":
                    // check if endpoint for that ip:port has already been created
                    if (udpEndPointDictionary.ContainsKey(address))
                    {
                        // return if true
                        return udpEndPointDictionary[address];
                    }
                    // create endpoint if not already present
                    var endpoint = new ScsUdpEndPoint(address);
                    udpEndPointDictionary[address] = endpoint;
                    return endpoint;

                default:
                    throw new ApplicationException("Unsupported protocol " + protocol + " in end point " +
                                                   endPointAddress);
            }
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
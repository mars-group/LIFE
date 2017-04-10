//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using Hik.Communication.Scs.Communication.EndPoints;

namespace Hik.Communication.Scs.Client
{
    /// <summary>
    ///     This class is used to create SCS Clients to connect to a SCS server.
    /// </summary>
    public static class ScsClientFactory
    {
        /// <summary>
        ///     Creates a new client to connect to a server using an end point.
        /// </summary>
        /// <param name="endpoint">End point of the server to connect it</param>
        /// <returns>Created TCP client</returns>
        public static IScsClient CreateClient(ScsEndPoint endpoint)
        {
            return endpoint.CreateClient();
        }

        /// <summary>
        ///     Creates a new client to connect to a server using an end point.
        /// </summary>
        /// <param name="endpointAddress">End point address of the server to connect it</param>
        /// <returns>Created TCP client</returns>
        public static IScsClient CreateClient(string endpointAddress)
        {
            return CreateClient(ScsEndPoint.CreateEndPoint(endpointAddress));
        }
    }
}
//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System;
using Hik.Communication.Scs.Communication;
using Hik.Communication.Scs.Communication.EndPoints;

namespace Hik.Communication.ScsServices.Service
{
    /// <summary>
    ///     Represents a client that uses a SDS service.
    /// </summary>
    public interface IScsServiceClient
    {
        /// <summary>
        ///     This event is raised when client is disconnected from service.
        /// </summary>
        event EventHandler Disconnected;

        /// <summary>
        ///     Unique identifier for this client.
        /// </summary>
        long ClientId { get; }

        /// <summary>
        ///     Gets endpoint of remote application.
        /// </summary>
        ScsEndPoint RemoteEndPoint { get; }

        /// <summary>
        ///     Gets the communication state of the Client.
        /// </summary>
        CommunicationStates CommunicationState { get; }

        /// <summary>
        ///     Closes client connection.
        /// </summary>
        void Disconnect();

        /// <summary>
        ///     Gets the client proxy interface that provides calling client methods remotely.
        /// </summary>
        /// <param name="serviceID">The ID of the ServiceObject on the Server</param>
        /// <typeparam name="T">Type of client interface</typeparam>
        /// <returns>Client interface</returns>
        T GetClientProxy<T>(Guid serviceID) where T : class;
    }
}
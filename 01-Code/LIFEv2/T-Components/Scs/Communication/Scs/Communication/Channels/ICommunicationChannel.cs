//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using Hik.Communication.Scs.Communication.EndPoints;
using Hik.Communication.Scs.Communication.Messengers;

namespace Hik.Communication.Scs.Communication.Channels {
    /// <summary>
    ///     Represents a communication channel.
    ///     A communication channel is used to communicate (send/receive messages) with a remote application.
    /// </summary>
    internal interface ICommunicationChannel : IMessenger {
        /// <summary>
        ///     This event is raised when client disconnected from server.
        /// </summary>
        event EventHandler Disconnected;

        /// <summary>
        ///     Gets endpoint of remote application.
        /// </summary>
        ScsEndPoint RemoteEndPoint { get; }

        /// <summary>
        ///     Gets the current communication state.
        /// </summary>
        CommunicationStates CommunicationState { get; }

        /// <summary>
        ///     Starts the communication with remote application.
        /// </summary>
        void Start();

        /// <summary>
        ///     Closes messenger.
        /// </summary>
        void Disconnect();
    }
}
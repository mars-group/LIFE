﻿//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System;
using ASC.Communication.Scs.Communication.EndPoints;
using ASC.Communication.Scs.Communication.Messages;
using ASC.Communication.Scs.Communication.Protocols;

namespace ASC.Communication.Scs.Communication.Channels
{
    /// <summary>
    ///     This class provides base functionality for all communication channel classes.
    /// </summary>
    internal abstract class CommunicationChannelBase : ICommunicationChannel
    {
        #region Public events

        /// <summary>
        ///     This event is raised when a new message is received.
        /// </summary>
        public event EventHandler<MessageEventArgs> MessageReceived;

        /// <summary>
        ///     This event is raised when a new message is sent without any error.
        ///     It does not guaranties that message is properly handled and processed by remote application.
        /// </summary>
        public event EventHandler<MessageEventArgs> MessageSent;

        /// <summary>
        ///     This event is raised when communication channel closed.
        /// </summary>
        public event EventHandler Disconnected;

        #endregion

        #region Public abstract properties

        /// <summary>
        ///     Gets endpoint of remote application.
        /// </summary>
        public abstract AscEndPoint RemoteEndPoint { get; }

        #endregion

        #region Public properties

        /// <summary>
        ///     Gets the current communication state.
        /// </summary>
        public CommunicationStates CommunicationState { get; protected set; }

        /// <summary>
        ///     Gets the time of the last succesfully received message.
        /// </summary>
        public DateTime LastReceivedMessageTime { get; protected set; }

        /// <summary>
        ///     Gets the time of the last succesfully sent message.
        /// </summary>
        public DateTime LastSentMessageTime { get; protected set; }

        /// <summary>
        ///     Gets/sets wire protocol that the channel uses.
        ///     This property must set before first communication.
        /// </summary>
        public IAcsWireProtocol WireProtocol { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        ///     Constructor.
        /// </summary>
        protected CommunicationChannelBase()
        {
            CommunicationState = CommunicationStates.Disconnected;
            LastReceivedMessageTime = DateTime.MinValue;
            LastSentMessageTime = DateTime.MinValue;
        }

        #endregion

        #region Public abstract methods

        /// <summary>
        ///     Disconnects from remote application and closes this channel.
        /// </summary>
        public abstract void Disconnect();

        #endregion

        #region Public methods

        /// <summary>
        ///     Starts the communication with remote application.
        /// </summary>
        public void Start()
        {
            StartInternal();
            CommunicationState = CommunicationStates.Connected;
        }

        /// <summary>
        ///     Sends a message to the remote application.
        /// </summary>
        /// <param name="message">Message to be sent</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if message is null</exception>
        public void SendMessage(IAscMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");

            SendMessageInternal(message);
        }

        #endregion

        #region Protected abstract methods

        /// <summary>
        ///     Starts the communication with remote application really.
        /// </summary>
        protected abstract void StartInternal();

        /// <summary>
        ///     Sends a message to the remote application.
        ///     This method is overrided by derived classes to really send to message.
        /// </summary>
        /// <param name="message">Message to be sent</param>
        protected abstract void SendMessageInternal(IAscMessage message);

        #endregion

        #region Event raising methods

        /// <summary>
        ///     Raises Disconnected event.
        /// </summary>
        protected virtual void OnDisconnected()
        {
            var handler = Disconnected;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        ///     Raises MessageReceived event.
        /// </summary>
        /// <param name="message">Received message</param>
        protected virtual void OnMessageReceived(IAscMessage message)
        {
            var handler = MessageReceived;
            if (handler != null) handler(this, new MessageEventArgs(message));
        }

        /// <summary>
        ///     Raises MessageSent event.
        /// </summary>
        /// <param name="message">Received message</param>
        protected virtual void OnMessageSent(IAscMessage message)
        {
            var handler = MessageSent;

            if (handler != null)
            {
                handler(this, new MessageEventArgs(message));
            }
        }

        #endregion
    }
}
//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System.Collections.Concurrent;
using ASC.Communication.Scs.Communication.Channels;
using ASC.Communication.Scs.Communication.Messengers;
using ASC.Communication.Scs.Communication.Protocols;
using CustomUtilities.Collections;

namespace ASC.Communication.Scs.Server {
    /// <summary>
    ///     This class provides base functionality for server classes.
    /// </summary>
    internal abstract class AscServerBase : IAscServer {

        #region Public properties

        /// <summary>
        ///     Gets/sets wire protocol that is used while reading and writing messages.
        /// </summary>
        public IAcsWireProtocolFactory WireProtocolFactory { get; set; }

        /// <summary>
        ///     A collection of clients that are connected to the server.
        /// </summary>
        private ConcurrentDictionary<long, IAscServerClient> Clients { get; set; }

        #endregion

        #region Private properties

        /// <summary>
        ///     This object is used to listen incoming connections.
        /// </summary>
        private IConnectionListener _connectionListener;

        #endregion

        #region Constructor

        /// <summary>
        ///     Constructor.
        /// </summary>
        protected AscServerBase() {
            Clients = new ConcurrentDictionary<long, IAscServerClient>();
            WireProtocolFactory = WireProtocolManager.GetDefaultWireProtocolFactory();
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Starts the server.
        /// </summary>
        public virtual void Start() {
            
        }

        /// <summary>
        ///     Stops the server.
        /// </summary>
        public virtual void Stop() {
        }

        public abstract IMessenger GetMessenger();

        #endregion

        #region Protected abstract methods

        /// <summary>
        ///     This method is implemented by derived classes to create appropriate connection listener to listen incoming
        ///     connection requets.
        /// </summary>
        /// <returns></returns>
        protected abstract IConnectionListener CreateConnectionListener();

        #endregion

        #region Private methods


        #endregion
    }
}
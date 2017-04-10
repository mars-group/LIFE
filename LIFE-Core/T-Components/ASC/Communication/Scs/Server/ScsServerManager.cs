//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System.Threading;

namespace ASC.Communication.Scs.Server
{
    /// <summary>
    ///     Provides some functionality that are used by servers.
    /// </summary>
    internal static class ScsServerManager
    {
        /// <summary>
        ///     Used to set an auto incremential unique identifier to clients.
        /// </summary>
        private static long _lastClientId;

        /// <summary>
        ///     Gets an unique number to be used as idenfitier of a client.
        /// </summary>
        /// <returns></returns>
        public static long GetClientId()
        {
            return Interlocked.Increment(ref _lastClientId);
        }
    }
}
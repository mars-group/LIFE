//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using ASC.Communication.Scs.Communication.EndPoints;

namespace ASC.Communication.Scs.Server
{
    /// <summary>
    ///     This class is used to create SCS servers.
    /// </summary>
    public static class AcsServerFactory
    {
        /// <summary>
        ///     Creates a new SCS Server using an EndPoint.
        /// </summary>
        /// <param name="endPoint">Endpoint that represents address of the server</param>
        /// <returns>Created TCP server</returns>
        public static IAscServer CreateServer(AscEndPoint endPoint)
        {
            return endPoint.CreateServer();
        }
    }
}
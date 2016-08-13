//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using ASC.Communication.Scs.Communication.Messengers;
using ASC.Communication.Scs.Server;

namespace ASC.Communication.ScsServices.Service {
    /// <summary>
    ///     This class is used to create service client objects that is used in server-side.
    /// </summary>
    internal static class AscServiceClientFactory {
        /// <summary>
        ///     Creates a new service client object that is used in server-side.
        /// </summary>
        /// <param name="serverClient">Underlying server client object</param>
        /// <param name="requestReplyMessenger">RequestReplyMessenger object to send/receive messages over serverClient</param>
        /// <returns></returns>
        public static IAscServiceClient CreateServiceClient(IAscServerClient serverClient,
            RequestReplyMessenger<IAscServerClient> requestReplyMessenger) {
            return new AscServiceClient(serverClient, requestReplyMessenger);
        }
    }
}
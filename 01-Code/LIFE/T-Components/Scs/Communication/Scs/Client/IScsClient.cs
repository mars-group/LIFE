﻿//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using Hik.Communication.Scs.Communication.Messengers;

namespace Hik.Communication.Scs.Client {
    /// <summary>
    ///     Represents a client to connect to server.
    /// </summary>
    public interface IScsClient : IMessenger, IConnectableClient {
        //Does not define any additional member
    }
}
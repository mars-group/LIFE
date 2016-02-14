//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
namespace ASC.Communication.ScsServices.Communication.Messages
{
    public class RemoveShadowAgentEventArgs
    {
        public RemoveShadowAgentMessage RemoveShadowAgentMessage { get; private set; }

        public RemoveShadowAgentEventArgs(RemoveShadowAgentMessage message) {
            RemoveShadowAgentMessage = message;
        }
    }
}

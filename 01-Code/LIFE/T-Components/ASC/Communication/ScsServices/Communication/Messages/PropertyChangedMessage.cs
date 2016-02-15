//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using ASC.Communication.Scs.Communication.Messages;

namespace ASC.Communication.ScsServices.Communication.Messages {
    /// <summary>
    ///     Represents the update of a property in a remote stub
    /// </summary>
    [Serializable]
    public class PropertyChangedMessage : AscMessage {
        /// <summary>
        ///     The new value which is to be updated in the stub
        /// </summary>
        public object NewValue { get; set; }

        /// <summary>
        ///     The name of the property whos value is to be updated in the stub
        /// </summary>
        public string PropertyGetMethodName { get; set; }


        public PropertyChangedMessage(object newValue, string propertyGetMethodName) {
            NewValue = newValue;
            PropertyGetMethodName = propertyGetMethodName;
        }

        public override string ToString() {
            return string.Format("PropertyChangedMessage: {0}.{1}(...)", NewValue, PropertyGetMethodName);
        }
    }
}
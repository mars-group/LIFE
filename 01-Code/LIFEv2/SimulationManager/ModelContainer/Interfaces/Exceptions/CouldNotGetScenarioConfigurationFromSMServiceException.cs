// // /*******************************************************
// //  * Copyright (C) Christian Hüning - All Rights Reserved
// //  * Unauthorized copying of this file, via any medium is strictly prohibited
// //  * Proprietary and confidential
// //  * This file is part of the MARS LIFE project, which is part of the MARS System
// //  * More information under: http://www.mars-group.org
// //  * Written by Christian Hüning <christianhuening@gmail.com>, 01.11.2016
// //  *******************************************************/

using System;

namespace ModelContainer.Interfaces.Exceptions
{
    [Serializable]
    public class CouldNotGetScenarioConfigurationFromSMServiceException : Exception
    {
        public CouldNotGetScenarioConfigurationFromSMServiceException(string msg) : base(msg) { }

    }
}
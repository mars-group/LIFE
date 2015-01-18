// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/

namespace LifeAPI.Agent {
    public interface ITickClient {

        /// <summary>
        /// Will be called by the MARS Framework when a new simulation tick shall be made. 
        /// Usually you shouldn't call this method in your own code. Instead use the RegisterAgentHandle of each layers' 
        /// Initialization Method to register your Agent for execution at the LayerContainer
        /// </summary>
        void Tick();
    }
}
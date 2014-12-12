// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/

using LayerAPI.Agent;

namespace LayerAPI.Layer {
    /// <summary>
    ///     The ISteppedActiveLayer will get ticked by the LayerContainer, just as the average ITickClient.
    ///     In Addition it provides to more methods which allow to hook into the moment just before and after a tick.
    /// </summary>
    public interface ISteppedActiveLayer : ISteppedLayer, ITickClient {
        /// <summary>
        ///     Gets called just before all agents get ticked
        /// </summary>
        void PreTick();

        /// <summary>
        ///     Gets called after all agents get ticked, but before
        ///     the next Tick launches.
        /// </summary>
        void PostTick();
    }
}
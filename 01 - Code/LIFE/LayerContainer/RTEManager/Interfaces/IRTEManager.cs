// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/

using System.Collections.Generic;
using LayerAPI.Interfaces;
using LCConnector.TransportTypes;

namespace RTEManager.Interfaces {
    /// <summary>
    ///     The RunTimeEnvironment Manager manages the ressources available within this
    ///     LayerContainer and triggers each layer's Tick client according to the available ressources.
    /// </summary>
    public interface IRTEManager {
        /// <summary>
        ///     Registers a layer with the RuntimeEvironment
        ///     Hint: To retreive a layer instance, use the LayerRegistry-Component
        /// </summary>
        /// <param name="layer">The layer to register</param>
        void RegisterLayer(TLayerInstanceId instanceId, ILayer layer);

        /// <summary>
        ///     Un-registers a layer from the RTE.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="layerInstanceId"></param>
        void UnregisterLayer(TLayerInstanceId layerInstanceId);

        /// <summary>
        ///     Marks an Agent / a TickClient to be unregistered before the next tick
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="tickClient"></param>
        void UnregisterTickClient(ILayer layer, ITickClient tickClient);

        /// <summary>
        ///     Registers an agent with the LayerContainer.
        ///     If the simulation is already running the agent / tickClient will
        ///     get marked for registration and added before the next tick.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="tickClient"></param>
        void RegisterTickClient(ILayer layer, ITickClient tickClient);

        /// <summary>
        ///     Initializes the layer wit
        ///     <param name="instanceId" />
        ///     by using
        ///     <param name="initData" />
        ///     . Will be called for every registered
        ///     Layer prior to simulation start.
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="initData"></param>
        void InitializeLayer(TLayerInstanceId instanceId, TInitData initData);

        /// <summary>
        ///     Returns all the tickClients (agents) registered for
        ///     <param name="layer" />
        ///     .
        /// </summary>
        /// <param name="layer"></param>
        /// <returns>List of ITickClient, empty if none</returns>
        IEnumerable<ITickClient> GetAllTickClientsByLayer(TLayerInstanceId layer);

        /// <summary>
        ///     Advances the whole LayerContainer by one tick.
        ///     In other words: Simulates one tick.
        /// </summary>
        /// <returns>The milliseconds the last tick took to execute.</returns>
        long AdvanceOneTick();

        // TODO: Information methods needed!
    }
}
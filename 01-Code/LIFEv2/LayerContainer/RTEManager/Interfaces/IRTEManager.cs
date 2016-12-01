//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System.Collections.Generic;
using System.Dynamic;
using LCConnector.TransportTypes;
using LifeAPI.Agent;
using LifeAPI.Layer;

namespace RTEManager.Interfaces {
    /// <summary>
    ///     The RunTimeEnvironment Manager manages the ressources available within this
    ///     LayerContainer and triggers each layer's Tick client according to the available ressources.
    /// </summary>
    public interface IRTEManager
    {
        
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
        void UnregisterTickClient(ILayer layer, ITickClient tickClient, int executionInterval=1);

        /// <summary>
        ///     Registers an agent with the LayerContainer.
        ///     If the simulation is already running the agent / tickClient will
        ///     get marked for registration and added before the next tick.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="tickClient"></param>
        /// <param name="executionInterval">Set to tell LIFE to execute this agent in a specific interval
        /// other than every tick.</param>
        void RegisterTickClient(ILayer layer, ITickClient tickClient, int executionInterval=1);

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
        bool InitializeLayer(TLayerInstanceId instanceId, TInitData initData);

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
        long Advance(long ticksToAdvanceBy = 1);

        /// <summary>
        /// Calls DisposeLayer on all IDisposableLayer layers.
        /// </summary>
        void DisposeSuitableLayers();

        // TODO: Information methods needed!
    }
}
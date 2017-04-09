//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using LIFE.API.Agent;
using LIFE.API.Layer.Initialization;
using LIFE.API.LIFECapabilities;

namespace LIFE.API.Layer
{
    public delegate void RegisterAgent(ILayer layer, ITickClient tickClient, int executionInterval = 1);

    public delegate void UnregisterAgent(ILayer layer, ITickClient tickClient, int executionInterval = 1);

    /// <summary>
    ///   Base Interface for all layers.
    ///   DO NOT IMPLEMENT THIS UNLESS YOU KNOW WHAT YOU'RE DOING!
    ///   Instead implement either ISteppedLayer or one of its decendants depending on your requirements.
    /// </summary>
    public interface ILayer : ILifeAutoInitialized
    {
        /// <summary>
        ///   Initializes the layer with layerInitData.
        ///   Use this instead of the constructor, as it is
        ///   guaranteed to be called in the correct load order.
        ///   <pre>This layer was successfully added to its container.</pre>
        ///   <post>
        ///     This layer is in a state which allows
        ///     it to start the simulation.
        ///   </post>
        ///   <param name="layerInitData">
        ///     A datatype holding the
        ///     information of how to initialize a layer.
        ///   </param>
        ///   <param name="registerAgentHandle"> </param>
        /// </summary>
        /// <returns>True if init finished successfully, false otherwise</returns>
        bool InitLayer(TInitData layerInitData, RegisterAgent registerAgentHandle,
            UnregisterAgent unregisterAgentHandle);

        /// <summary>
        ///   The current Tick this layer is in
        /// </summary>
        /// <returns>
        ///   Positive long value in active simulation
        ///   or if simulation has ended, -1 otherwise
        /// </returns>
        long GetCurrentTick();

        /// <summary>
        ///   Sets the current Tick of the layer.
        ///   This will be called by the RuntimeEnvironment to
        ///   deliver the currenTick into the layer.
        /// </summary>
        /// <param name="currentTick">A positive number</param>
        void SetCurrentTick(long currentTick);
    }
}
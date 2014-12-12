// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 13.05.2014
//  *******************************************************/

using System;
using LayerAPI.Layer;

namespace LayerRegistry.Interfaces {
    /// <summary>
    ///     The LayerRegistry.
    ///     Takes care of resolving layer instances locally and remotely
    /// </summary>
    public interface ILayerRegistry {
        /// <summary>
        ///     Removes the layer with instance layerInstanceId.
        ///     CAUTION: Can not be undone! Use only in re-partitioning process
        ///     or if new simulation shall be startet.
        /// </summary>
        /// <param name="layerType"></param>
        /// <param name="layerID"></param>
        /// <returns>The removed ILayer, Null if no Layer could be found.</returns>
        ILayer RemoveLayerInstance(Type layerType);

        /// <summary>
        ///     Resets the whole LayerRegistry, loosing all implementations, statets and
        ///     remote endpoints.
        ///     CAUTION: This cannot be undone!
        /// </summary>
        void ResetLayerRegistry();

        /// <summary>
        ///     Returns an instance of parameterType either as local object or as a stub
        /// </summary>
        /// <param name="parameterType"></param>
        /// <returns></returns>
        ILayer GetLayerInstance(Type layerType);

        /// <summary>
        ///     Registers layer as being instantiated on this node.
        ///     Will store a reference for local usage, as well as
        ///     connection information in the DHT
        /// </summary>
        /// <param name="layer"></param>
        void RegisterLayer(ILayer layer);
    }
}
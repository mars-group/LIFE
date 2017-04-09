//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System;
using Hik.Communication.ScsServices.Service;
using LNSConnector.TransportTypes;

namespace LNSConnector.Interface
{
    /// <summary>
    /// This service allows to resolve connectiviy information by a valid layer name.
    /// </summary>
    [ScsService]
    public interface ILayerNameService
    {
        /// <summary>
        /// Resolves a layer by its type.
        /// </summary>
        /// <param name="layerType">The FullName property of the layer Type</param>
        /// <returns>A TLayerNameServiceEntry object containing all relevant information, null if nothing found.</returns>
        TLayerNameServiceEntry ResolveLayer(string layerType);

        /// <summary>
        /// Registers a layer of type <param name="layerType"/> with its entry.
        /// </summary>
        /// <param name="layerType"></param>
        /// <param name="layerNameServiceEntry"></param>
        void RegisterLayer(string layerType, TLayerNameServiceEntry layerNameServiceEntry);

        void RemoveLayer(string layerType, TLayerNameServiceEntry layerNameServiceEntry);
    }
}
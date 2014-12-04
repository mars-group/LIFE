// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/

using LCConnector.TransportTypes.ModelStructure;
using Mono.Addins;

namespace LayerAPI.AddinLoader {
    public interface IAddinLoader {
        /// <summary>
        ///     Loads a ModelContent. Will write out all assemblies from
        ///     the modelContent into the addingRegistry and make them available for
        ///     intialization.
        /// </summary>
        /// <param name="modelContent"></param>
        void LoadModelContent(ModelContent modelContent);

        /// <summary>
        ///     Loads a Layer by the given name and returns its TypeExtensionNode object.
        /// </summary>
        /// <param name="layerName">The name of the layer. Must be the name of the implementing class.</param>
        /// <returns>A TypeExtensionNode object. May be used to directly instanciate the layer or to reflect its dependencies</returns>
        TypeExtensionNode LoadLayer(string layerName);


        ExtensionNodeList LoadAllLayers();

        ExtensionNodeList LoadAllLayers(string modelName);
    }
}
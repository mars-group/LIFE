//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using LCConnector.TransportTypes;
using LCConnector.TransportTypes.ModelStructure;

namespace PartitionManager.Interfaces
{
    /// <summary>
    /// The PartitionaManager controls storage and loading
    /// operations of a layer which is about to be instantiated on 
    /// this LayerContainer.
    /// </summary>
    public interface IPartitionManager
    {
        /// <summary>
        /// Adds a Layer derived from the provied TLayerInstanceId.
        /// </summary>
        /// <param name="instanceId">The TLayerInstanceId object which holds information about the 
        /// which is to be added.</param>
        /// <returns>True if success, false otherwise</returns>
        bool AddLayer(TLayerInstanceId instanceId);

        /// <summary>
        /// Loads the model content passed via the ModelContent 
        /// parameter. 
        /// </summary>
        /// <param name="content">The ModelContent object containing
        /// the serialized model contents.</param>
        void LoadModelContent(ModelContent content);
    }
}
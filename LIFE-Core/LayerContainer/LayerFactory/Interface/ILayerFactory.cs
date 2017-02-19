//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using LCConnector.TransportTypes.ModelStructure;
using LIFE.API.Layer;


namespace LayerFactory.Interface {
    public interface ILayerFactory {
        /// <summary>
        ///     Retreives a layer from internal structures
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="binaryData"></param>
        /// <returns></returns>
        ILayer GetLayer(string layerName);

        void LoadModelContent(ModelContent content);
    }
}
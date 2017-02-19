//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using LNSConnector.Interface;
using LNSConnector.TransportTypes;

namespace LayerNameService.Implementation
{

    public class LayerNameServiceComponent : ILayerNameService
    {
        private readonly ILayerNameService _layerNameServiceUseCase;

        public LayerNameServiceComponent()
        {
            _layerNameServiceUseCase = new LayerNameServiceUseCase();
        }

        public TLayerNameServiceEntry ResolveLayer(string layerType)
        {
            return _layerNameServiceUseCase.ResolveLayer(layerType);
        }

        public void RegisterLayer(string layerType, TLayerNameServiceEntry layerNameServiceEntry)
        {
            _layerNameServiceUseCase.RegisterLayer(layerType, layerNameServiceEntry);
        }

        public void RemoveLayer(string layerType, TLayerNameServiceEntry layerNameServiceEntry)
        {
            _layerNameServiceUseCase.RemoveLayer(layerType, layerNameServiceEntry);
        }
    }
}

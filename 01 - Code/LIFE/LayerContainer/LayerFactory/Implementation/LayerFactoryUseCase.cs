// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/

using System.Linq;
using LayerFactory.Interface;
using LayerRegistry.Interfaces;
using LCConnector.TransportTypes.ModelStructure;
using LifeAPI.AddinLoader;
using LifeAPI.Layer;

namespace LayerFactory.Implementation {
    internal class LayerFactoryUseCase : ILayerFactory {
        private readonly ILayerRegistry _layerRegistry;
        private readonly IAddinLoader _addinLoader;

        public LayerFactoryUseCase(ILayerRegistry layerRegistry) {
            _layerRegistry = layerRegistry;
            _addinLoader = AddinLoader.Instance;
        }

        #region ILayerFactory Members

        public ILayer GetLayer(string layerName) {
            ILayer result;

            var typeExtensionNode = _addinLoader.LoadLayer(layerName);

            var constructors = typeExtensionNode.Type.GetConstructors();

            // check if there is an empty constructor
            if (constructors.Any(c => c.GetParameters().Length == 0))
                result = (ILayer) typeExtensionNode.CreateInstance();
            else {
                // take first constructor, resolve dependencies from LayerRegistry and instanciate Layer
                var currentConstructor = constructors[0];
                var neededParameters = currentConstructor.GetParameters();

                var actualParameters = new object[neededParameters.Length];

                var i = 0;
                foreach (var parameterInfo in neededParameters) {
                    actualParameters[i] = _layerRegistry.GetLayerInstance(parameterInfo.ParameterType);
                    i++;
                }
                result = (ILayer) currentConstructor.Invoke(actualParameters);
            }
            _layerRegistry.RegisterLayer(result);
            return result;
        }

        public void LoadModelContent(ModelContent content) {
            _addinLoader.LoadModelContent(content);
        }

        #endregion
    }
}
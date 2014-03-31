using System;
using System.Linq;
using LayerAPI.AddinLoader;
using LayerAPI.Interfaces;
using LayerFactory.Interface;
using LayerRegistry.Interfaces;

namespace LayerFactory.Implementation
{
    class LayerFactoryUseCase : ILayerFactory
    {
        private readonly ILayerRegistry _layerRegistry;
        private readonly IAddinLoader _addinLoader;

        public LayerFactoryUseCase(ILayerRegistry layerRegistry)
        {
            _layerRegistry = layerRegistry;
            _addinLoader = new AddinLoader();
        }

        public ILayer GetLayer<T>(Uri layerUri) where T : ILayer
        {
            var typeExtensionNode = _addinLoader.LoadLayer(layerUri, typeof(T));
            var constructors = typeExtensionNode.GetType().GetConstructors();

            // check if there is an empty constructor
            if (constructors.Any(c => c.GetParameters().Length == 0))
            {
                return (T) typeExtensionNode.CreateInstance();
            }

            // take first constructor, resolve dependencies from LayerRegistry and instanciate Layer
            var currentConstructor = constructors[0];
            var neededParameters = currentConstructor.GetParameters();

            var actualParameters = new object[neededParameters.Length];

            var i = 0;
            foreach (var parameterInfo in neededParameters)
            {
                actualParameters[i] = _layerRegistry.GetLayerInstance(parameterInfo.ParameterType);
                i++;
            }
            var result = (T)currentConstructor.Invoke(actualParameters);

            _layerRegistry.RegisterLayer(result);

            return result;
        }
    }
}

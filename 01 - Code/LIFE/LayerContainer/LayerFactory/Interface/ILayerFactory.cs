using System;
using LayerAPI.Interfaces;

namespace LayerFactory.Interface {
    public interface ILayerFactory {
        /// <summary>
        ///     1. Downloads the layer's dll from layerUri
        ///     2. Loads it into the internal AddinRegistry
        ///     3. Resolves dependencies via ILayerRegistry
        ///     4  Instantiates the loaded Layer
        ///     5. Registers the new layer via ILayerRegistry
        ///     6. Returns the instance of ILayer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="layerUri"></param>
        /// <returns></returns>
        ILayer GetLayer<T>(Uri layerUri) where T : ILayer;
    }
}
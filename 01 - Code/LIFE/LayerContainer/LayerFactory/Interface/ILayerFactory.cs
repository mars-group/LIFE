using System;
using LayerAPI.Interfaces;
using LCConnector.TransportTypes.ModelStructure;

namespace LayerFactory.Interface {
    public interface ILayerFactory {
        /// <summary>
        /// Retreives a layer from internal structures
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="binaryData"></param>
        /// <returns></returns>
        ILayer GetLayer(string layerName);

        void LoadModelContent(ModelContent content);
    }
}
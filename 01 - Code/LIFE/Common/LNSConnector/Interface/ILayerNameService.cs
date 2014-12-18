using System;
using Hik.Communication.ScsServices.Service;
using LNSConnector.TransportTypes;

namespace LNSConnector.Interface
{
    /// <summary>
    /// This service allows to resolve connectiviy information by a valid layer name.
    /// </summary>
    [ScsService(Version = "0.1")]
    public interface ILayerNameService
    {
        /// <summary>
        /// Resolves a layer by its type.
        /// </summary>
        /// <param name="layerType"></param>
        /// <returns>A TLayerNameServiceEntry object containing all relevant information, null if nothing found.</returns>
        TLayerNameServiceEntry ResolveLayer(Type layerType);

        /// <summary>
        /// Registers a layer of type <param name="layerType"/> with its entry.
        /// </summary>
        /// <param name="layerType"></param>
        /// <param name="layerNameServiceEntry"></param>
        void RegisterLayer(Type layerType, TLayerNameServiceEntry layerNameServiceEntry);
    }
}

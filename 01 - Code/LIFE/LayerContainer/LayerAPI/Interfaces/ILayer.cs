using System;
using LIFE.LayerContainer.LayerAPI.DataTypes;

namespace LIFE.LayerContainer.LayerAPI.Interfaces
{
    /// <summary>
    /// Base Interface for all layers. DO NOT IMPLEMENT THIS!
    /// Instead use ISteppedLayer or IEventDrivenLayer depending on your requirements.
    /// </summary>
    interface ILayer
    {
        /// <summary>
        /// Initializes the layer with layerInitData.
        /// Use this instead of the constructor, as it is
        /// guaranteed to be called in the correct load order.
        /// <pre>This layer was successfully added to its container.</pre> 
        /// <post> This layer is in a state which allows
        /// it to start the simulation.</post>
        /// <param name="layerInitData">A datatype holding the
        /// information of how to initialize a layer.</param>
        /// </summary>
        /// <returns>True if init finished successfully, false otherwise</returns>
        Boolean InitLayer(LayerInitData layerInitData);

        /// <summary>
        /// The unique ID of this layer as a GUID.
        /// </summary>
        /// <returns>GUID representing the unique ID
        /// of this layer</returns>
        Guid GetID();

    }
}

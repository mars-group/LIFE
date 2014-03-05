using System;
using LIFE.LayerContainer.LayerAPI.DataTypes;

namespace LIFE.LayerContainer.LayerAPI.Interfaces
{
    /// <summary>
    /// This interface must be implemented by anyone who
    /// wants to create a new Layer runnable in a LayerContainer.
    /// The interface IServices may be used inside of that
    /// implementation to access the provided services
    /// from the LayerContainer.
    /// </summary>
    interface ILayer
    {
        /// <summary>
        /// Initializes the layer with layerInitData.
        /// Use this instead of the constructor, as it is
        /// guaranteed to be called in the correct load order.
        /// <pre>This layer was successfully added to</pre> 
        /// its container.
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
